using System.Linq.Expressions;

namespace android_backend.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class
    {
        TKey Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TKey id);

        TEntity FindById(TKey id);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        List<TEntity> FindAll();
    }
}