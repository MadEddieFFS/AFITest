using AFI.Domain.Models.PolicyHolders;
using AFI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFI.Test.TestHelpers
{
    public class InMemoryDbContextFactory
    {
        public AFIContext GetAFIContext()
        {
            var options = new DbContextOptionsBuilder<AFIContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAFIDatabase")
            .Options;

            var dbContext = new AFIContext(options);

            return dbContext;
        }
    }
}
