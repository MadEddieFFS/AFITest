using AFI.Domain.Filters;
using AFI.Domain.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFI.Persistance.Repositories.Base
{
    public abstract class BaseRepository<S,T>
        where S : DbContext
        where T : BaseEntity
    {
        protected readonly DbSet<T> DBSet;
        private readonly S context;

        protected BaseRepository(S context)
        {
            this.context = context;
            DBSet = this.context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            // Collect from HTTP Context
            entity.CreatedByUserId = Guid.Empty;
            return (await DBSet.AddAsync(entity))?.Entity;
        }

        public virtual async Task<T> FindByIdAsync(Guid id)
        {
            return await DBSet.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual async Task<List<T>> ListAsync(BaseFilter<T> filter)
        {
            return await filter.Apply(DBSet).ToListAsync();
        }

        public virtual async Task<List<T>> ListAsync(IEnumerable<Guid> idxs)
        {
            return await DBSet
                .Where(entity => idxs.Contains(entity.Id))
                .ToListAsync();
        }

        public virtual T Remove(T entity)
        {
            return DBSet.Remove(entity)?.Entity;
        }

        public virtual T Update([NotNull] T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Would normally collect a user from the HTTP Context 
            entity.LastEditedByUserId = Guid.Empty;
            entity.EditedUTC = DateTime.UtcNow;

            return DBSet.Update(entity)?.Entity;
        }

    }
}