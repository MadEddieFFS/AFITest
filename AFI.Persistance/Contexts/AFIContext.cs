using AFI.Domain.Models.PolicyHolders;
using AFI.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AFI.Persistance.Contexts
{
    public class AFIContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AFIContext()
        {        }

        public AFIContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AFIContext(DbContextOptions<AFIContext> options)
        : base(options)
        {
        }

        public AFIContext(DbContextOptions<AFIContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }


        public virtual DbSet<PolicyHolder> PolicyHolder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["databaseconnectionstring"]);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-1337");


            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        #region Entity creation
        private ModelBuilder ConfigurePolicyHolder(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureBaseEntity<PolicyHolder>("PolicyHolder", "app")
                .Entity<PolicyHolder>(entity =>
                {
                    entity.Property(e => e.DateOfBirth)
                    .IsRequired(false);


                    entity.Property(e => e.EMail)
                    .IsRequired(false);

                    entity.Property(e => e.Forename)
                    .IsRequired()
                    .HasMaxLength(50);

                    entity.Property(e => e.ReferenceNumber)
                    .IsRequired()
                    .HasMaxLength(9);

                    entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);
                });

            return modelBuilder;
        }
        #endregion
    }
}
