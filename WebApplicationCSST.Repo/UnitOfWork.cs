using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationCSST.Data;

namespace WebApplicationCSST.Repo
{
    public class UnitOfWork : IUnitOfWork<ApplicationDbContext>
    {
        public ApplicationDbContext Context { get; }

        private readonly Dictionary<Type, object> _repositories;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed;

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        {
            Context = context;
            _disposed = false;
            _repositories = new Dictionary<Type, object>();
            _logger = logger;
        }

        public async Task<int> CommitAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            // Checks if the Dictionary Key contains the Model class
            if (_repositories.Keys.Contains(typeof(T)))
            {
                _logger.LogInformation($"Get old instance of Repository for {typeof(T).Name}");

                // Return the repository for that Model class
                return _repositories[typeof(T)] as IRepository<T>;
            }

            // If the repository for that Model class doesn't exist, create it
            var repository = new Repository<T>(this);

            _logger.LogInformation($"Create a new instance of Repository for {typeof(T).Name}");

            // Add it to the dictionary
            _repositories.Add(typeof(T), repository);

            return repository;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Context.Dispose();
            }
            
            _disposed = true;
        }
    }
}
