
namespace CQ.UnitOfWork.Abstractions;
public interface IDatabaseContext
{
    bool Ping(string? collection = null);

    Task SaveChangesAsync();
}
