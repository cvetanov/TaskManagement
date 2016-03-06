using TaskManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using TaskManagement.API.DataLayer.Models;

namespace TaskManagement.Persistence.Implementations
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private Entities _context;
        public Entities Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
                _entities = _context.Set<TEntity>();
            }
        }
        private DbSet<TEntity> _entities;

        public GenericRepository() { }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public virtual void Remove(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _entities.Attach(entityToDelete);
            }
            _entities.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _entities.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }



        public void Dispose()
        {
            _context.Dispose();
        }
    }
}