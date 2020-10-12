using AFI.Domain.Filters;
using AFI.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AFI.Domain.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<List<T>> ListAsync(BaseFilter<T> filter);

        Task<T> AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T> FindByIdAsync(Guid id);

        T Update(T entity);

    }
}
