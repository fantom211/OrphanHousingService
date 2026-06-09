using System.Linq.Expressions;

namespace OrphanHousingService.Services.Helpers
{
    public static class ReportQueryHelper
    {
        public static IQueryable<T> FilterByDate<T>(
            IQueryable<T> query,
            DateTime? from,
            DateTime? to,
            Expression<Func<T, DateTime>> dateSelector)
        {
            if (!from.HasValue && !to.HasValue)
                return query;

            if (from.HasValue)
            {
                var start = from.Value.Date;
                query = query.Where(
                    Expression.Lambda<Func<T, bool>>(
                        Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(start)),
                        dateSelector.Parameters));
            }

            if (to.HasValue)
            {
                var end = to.Value.Date;
                query = query.Where(
                    Expression.Lambda<Func<T, bool>>(
                        Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(end)),
                        dateSelector.Parameters));
            }

            return query;
        }
    }
}
