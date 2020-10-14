using AFI.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AFI.Domain.Models.PolicyHolders
{
    [Table("PolicyHolder")]
    public class PolicyHolder : BaseEntity
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EMail { get; set; }
    }
}
