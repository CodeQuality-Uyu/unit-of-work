
namespace CQ.UnitOfWork.Abstractions.Repositories;
public record class Pagination<TItem>(
    List<TItem> Items,
    long TotalItems,
    long TotalPages);
