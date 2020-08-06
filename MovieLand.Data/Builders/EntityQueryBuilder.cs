using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MovieLand.Data.Builders
{
    public class EntityQueryBuilder<T> where T : class
    {
        public EntityQueryBuilder() {
            Includes = new List<Expression<Func<T, object>>>();
        }

        public Expression<Func<T, bool>> Filter { get; private set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; private set; }
        public int? Limit { get; private set; }
        public int? Offset { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; private set; }

        public void SetFilter(Expression<Func<T, bool>> filter) {
            Filter = filter;
        }

        public void SetOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy) {
            OrderBy = orderBy;
        }

        public void SetLimit(int limit) {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("Limit", "Limit value must be positive");
            Limit = limit;
        }

        public void SetOffset(int offset) {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("Offset", "Offset value must be greater or equal to zero");
            Offset = offset;
        }

        public void AddInclude(Expression<Func<T, object>> include) {
            Includes.Add(include);
        }
    }
}
