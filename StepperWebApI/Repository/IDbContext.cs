using System.Data.Entity;

namespace Demo.Repository
{
    public interface IDbContext 
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
        void Update<TEntity>(TEntity entity);
    }
}