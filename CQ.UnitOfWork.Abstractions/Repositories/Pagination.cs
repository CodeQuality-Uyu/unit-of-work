
namespace CQ.UnitOfWork.Abstractions.Repositories;
public record Pagination<TItem>(
    List<TItem> Items,
    long TotalItems,
    long TotalPages,
    long Page = 0,
    long PageSize = 0)
{
    public long Count { get; init; } = Items.Count;

    public bool HasNext { get; init; } = Page != TotalPages;

    public bool HasPrevious { get; init; } = Page != 1;
}
