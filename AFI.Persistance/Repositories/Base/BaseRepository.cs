using AFI.Domain.Filters;
using AFI.Domain.Models.Base;
using AFI.Domain.Repositories.Base;
using AFI.Persistance.Contexts;
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
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AFIContext _context;
        private DbSet<T> _entities;
        string _ErrorMessage = string.Empty;
        public BaseRepository(AFIContext context)
        {
            this._context = context;
            this._entities = context.Set<T>();
        }

        public virtual async Task<T> FindByIdAsync(Guid id)
        {
            return await this._entities.SingleOrDefaultAsync(entity => entity.Id == id);
        }
        public async Task Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            await this._entities.AddAsync(entity);
            await this._context.SaveChangesAsync();
        }

        // Would normal add many more
        // Add
        // List
        // Remove
        // Update etc
    }
}