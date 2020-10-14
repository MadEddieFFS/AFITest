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
        Task<T> FindByIdAsync(int id);
        Task Insert(T entity);
    }
}
