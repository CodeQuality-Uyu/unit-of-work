
namespace CQ.UnitOfWork.Abstractions.Repositories;
public record Pagination<TItem>(
    List<TItem> Items,
    long TotalCount,
    long TotalPages,
    long Page,
    long PageSize)
{
    public long TotalItems { get; init; } = Items.Count;

    public bool HasNext { get; init; } = Page >= 1 && Page < TotalPages;

    public bool HasPrevious { get; init; } = Page > 1 && Page <= TotalPages;
}
