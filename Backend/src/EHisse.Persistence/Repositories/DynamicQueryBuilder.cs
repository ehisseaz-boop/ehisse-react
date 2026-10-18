using System.Linq.Expressions;

namespace EHisse.Persistence.Repositories;

public class DynamicQueryBuilder<T>
{
    private Expression<Func<T, bool>>? _predicate;

    public DynamicQueryBuilder()
    {
    }

    public DynamicQueryBuilder<T> And(Expression<Func<T, bool>> expr)
    {
        if (_predicate == null)
        {
            _predicate = expr;
        }
        else
        {
            var invokedExpr = Expression.Invoke(expr, _predicate.Parameters.Cast<Expression>());
            _predicate = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(_predicate.Body, invokedExpr), _predicate.Parameters);
        }
        return this;
    }

    public DynamicQueryBuilder<T> Or(Expression<Func<T, bool>> expr)
    {
        if (_predicate == null)
        {
            _predicate = expr;
        }
        else
        {
            var invokedExpr = Expression.Invoke(expr, _predicate.Parameters.Cast<Expression>());
            _predicate = Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(_predicate.Body, invokedExpr), _predicate.Parameters);
        }
        return this;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        if (_predicate != null)
        {
            query = query.Where(_predicate);
        }
        return query;
    }

    public Expression<Func<T, bool>>? GetPredicate() => _predicate;
}
