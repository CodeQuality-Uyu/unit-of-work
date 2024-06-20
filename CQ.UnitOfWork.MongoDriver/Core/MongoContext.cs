using CQ.UnitOfWork.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CQ.UnitOfWork.MongoDriver.Core;
public class MongoContext(IMongoDatabase _mongoDatabase) :
    IDatabaseContext
{
    private readonly List<Action> _actions = [];

    private readonly List<Func<Task>> _actionsTask = [];

    private readonly Dictionary<Type, string> collections = [];

    public MongoContext AddCollection<TEntity>(string collectionName)
    {
        collections.Add(typeof(TEntity), collectionName);

        return this;
    }

    public bool Ping(string? collection = null)
    {
        try
        {
            var result = _mongoDatabase.RunCommand<BsonDocument>(new BsonDocument($"{collection ?? "ping"}", 1));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IMongoCollection<TEntity> GetEntityCollection<TEntity>()
    {
        var collectionName = GetCollectionName<TEntity>();

        return _mongoDatabase.GetCollection<TEntity>(collectionName);
    }

    public bool HasCollectionRegistered<TEntity>()
    {
        return collections.ContainsKey(typeof(TEntity));
    }

    public string GetCollectionName<TEntity>()
    {
        collections.TryGetValue(typeof(TEntity), out string? collectionName);

        return collectionName ?? $"{typeof(TEntity).Name}s";
    }

    public IMongoCollection<BsonDocument> GetGenericCollection<TEntity>()
    {
        var collectionName = GetCollectionName<TEntity>();

        return _mongoDatabase.GetCollection<BsonDocument>(collectionName);
    }

    public void AddActionAsync(Func<Task> action)
    {
        _actionsTask.Add(action);
    }

    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

    public Task SaveChangesAsync()
    {
        _actions.ForEach(action =>
        {
            action();
        });

        Parallel.ForEach(_actionsTask, async action =>
        {
            await action().ConfigureAwait(false);
        });

        return Task.CompletedTask;
    }
}
