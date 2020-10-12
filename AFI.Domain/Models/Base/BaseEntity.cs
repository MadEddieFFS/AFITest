using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace AFI.Domain.Models.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastEditedByUserId { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime? EditedUTC { get; set; }
    }
}
