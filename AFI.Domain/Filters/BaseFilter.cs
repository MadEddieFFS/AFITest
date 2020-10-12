using AFI.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFI.Domain.Filters
{
  public abstract class BaseFilter<T> where T : BaseEntity
    {
        public abstract IQueryable<T> Apply(DbSet<T> dbSet);
    }
}
