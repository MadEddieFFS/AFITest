﻿using AFI.Domain.Repositories.PolicyHolders;
using AFI.Domain.Models.PolicyHolders;
using AFI.Persistance.Contexts;
using AFI.Persistance.Repositories.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AFI.Domain.Repositories.Base;

namespace AFI.Persistance.Repositories.PolicyHolders
{
    public class PolicyHolderRepository : BaseRepository<PolicyHolder>, IPolicyHolderRepository
    {
        public PolicyHolderRepository(AFIContext context) : base(context) { }

        public Task AddRangeAsync(IEnumerable<PolicyHolder> entities)
        {
            throw new NotImplementedException();
        }
    }
}