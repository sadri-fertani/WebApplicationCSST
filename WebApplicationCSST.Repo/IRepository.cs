using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationCSST.Data;

namespace WebApplicationCSST.Repo
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetAsync(long id);
        Task<IEnumerable<T>> GetAsync();
        IQueryable<T> GetQueryable();
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
