using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApplicationCSST.Data;

namespace WebApplicationCSST.Repo
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;

        Task<int> CommitAsync();
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
