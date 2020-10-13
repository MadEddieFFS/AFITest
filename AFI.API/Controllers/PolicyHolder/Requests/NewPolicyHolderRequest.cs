﻿using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AFI.API.Controllers.PolicyHolder.Requests
{
    [SwaggerSchema(Required = new[] { "Forename", "Surname", "ReferenceNumber" })]
    public class NewPolicyHolderRequest
    {
        [Required]
        [SwaggerSchema("New policyholder forename")]
        public string Forename { get; set; }
        [SwaggerSchema("New policyholder surname")]
        [Required]
        public string Surname { get; set; }
        [SwaggerSchema("New policyholder Reference Number")]
        [Required]
        public string ReferenceNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EMail { get; set; }


        public bool IsValid
        {
            get
            {
                // Forename is required - must be > 3 and < 50 chars
                if (string.IsNullOrEmpty(this.Forename) || this.Forename.Length < 3 || this.Forename.Length > 50)
                {
                    return false;
                }

                // Surname is required - must be > 3 and < 50 chars
                if (string.IsNullOrEmpty(this.Surname) || this.Surname.Length < 3 || this.Surname.Length > 50)
                {
                    return false;
                }

                //*** Reference number is required and must match reg ex
                // Test with
                // XX - 444444
                // 933444555
                // X - 3334444
                // XX - 22223
                // XX - 22222A
                // XX - 2122222

                var ReferenceRegexPattern = @"/[A-Z]{2}-[[0-9]{6}$";
                if (string.IsNullOrEmpty(this.ReferenceNumber) || !Regex.IsMatch(this.ReferenceNumber, ReferenceRegexPattern))
                {
                    return false;
                }

                bool ValidEMail = false;
                var EMailRegexPattern = @"/\w{4,}@\w{2,}(.com|.co.uk)$";
                // Is EMail address valid?
                // Reg Ex tests
                // 1234@test.com
                // 1234@test.co.uk
                // 123@test.com
                // 123@test.co.uk
                // 1234@a.com
                // 1234@a.co.uk
                // 1234@12.co.uk1
                // 1234@12.com1
                if (string.IsNullOrEmpty(this.EMail) || !Regex.IsMatch(this.EMail, EMailRegexPattern))
                {
                    ValidEMail = false;
                }

                bool ValidDOB = false;
                // New registering user must be 18
                if (!this.DateOfBirth.HasValue || this.DateOfBirth.Value.AddYears(-18) < DateTime.Now.AddYears(-18))
                {
                    ValidDOB = false;
                }

                // Documentation suggests that EITHER the policy holders DOB OR the poliy Holders EMail is part of the registration requirement
                // By that rational it is NOT BOTH
                // Assume to take EMail first if supplied

                // Check for at least 1 being valid
                if (!ValidEMail && !ValidDOB)
                {
                    // Neither Valid
                    return false;
                }

                if (ValidEMail)
                {
                    // Strip DOB from object
                    this.DateOfBirth = null;
                }
                else
                {
                    // Assuming Valid DOB as previous IF statement would not have allowed us this far
                    // Strip EMail in case it was invalid
                    this.EMail = null;
                }

                return true;
            }
        }
    }
}
