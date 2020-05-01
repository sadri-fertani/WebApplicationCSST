using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplicationCSST.Data;

namespace WebApplicationCSST.Repo
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IUnitOfWork<ApplicationDbContext> _unitOfWork;

        public Repository(IUnitOfWork<ApplicationDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<T> GetAsync(long id)
        {
            var query = _unitOfWork.Context.Set<T>().AsQueryable();
            IncludeChildren(ref query);

            return await query.FirstOrDefaultAsync<T>(x => x.Id == id);
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            var query = _unitOfWork.Context.Set<T>().AsQueryable();
            IncludeChildren(ref query);

            return await query.ToArrayAsync<T>();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _unitOfWork.Context.Set<T>().AsQueryable();
            IncludeChildren(ref query);

            return await query.Where(predicate).ToArrayAsync<T>();
        }

        public void Add(T entity)
        {
            _unitOfWork.Context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            T existing = _unitOfWork.Context.Set<T>().Find(entity.Id);
            if (existing != null) _unitOfWork.Context.Set<T>().Remove(existing);
        }

        public void Update(T entity)
        {
            T existing = _unitOfWork.Context.Set<T>().Find(entity.Id);

            if (existing != null)
            {
                _unitOfWork.Context.Entry(existing).State = EntityState.Modified;
                _unitOfWork.Context.Entry(existing).CurrentValues.SetValues(entity);
            }
        }

        private void IncludeChildren(ref IQueryable<T> query)
        {
            foreach (var property in _unitOfWork.Context.Model.FindEntityType(typeof(T)).GetNavigations())
                query = query.Include(property.Name);
        }
    }
}
