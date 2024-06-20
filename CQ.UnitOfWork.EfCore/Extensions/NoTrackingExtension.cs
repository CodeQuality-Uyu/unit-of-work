using Microsoft.EntityFrameworkCore;

namespace CQ.UnitOfWork.EfCore.Extensions;
public static class NoTrackingExtension
{
    public static IQueryable<T> TrackElements<T>(this DbSet<T> elements, bool track) where T : class
    {
        return track ? elements : elements.AsNoTracking();
    }
}
