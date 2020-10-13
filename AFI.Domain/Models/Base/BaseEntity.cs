using AFI.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace AFI.Domain.Models.Base
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastEditedByUserId { get; set; }
        public DateTime CreatedUTC { get; set; } = DateTime.UtcNow;
        public DateTime? EditedUTC { get; set; }
    }
}
