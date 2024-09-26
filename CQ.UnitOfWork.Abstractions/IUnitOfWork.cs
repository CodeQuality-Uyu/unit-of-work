namespace CQ.UnitOfWork.Abstractions;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}