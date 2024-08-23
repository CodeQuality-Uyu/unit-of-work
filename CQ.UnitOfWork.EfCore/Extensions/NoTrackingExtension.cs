using Microsoft.EntityFrameworkCore;

namespace CQ.UnitOfWork.EfCore.Extensions;
public static class NoTrackingExtension
{
    public static IQueryable<T> TrackElements<T>(
        this IQueryable<T> query,
        bool track)
        where T : class
    {
        return track
            ? query
            : query.AsNoTracking();
    }
}
