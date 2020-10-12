using AFI.Domain.Models.PolicyHolders;
using AFI.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AFI.Persistance.Contexts
{
    public class AFIContext : DbContext
    {
        private readonly IConfiguration configuration;

        public AFIContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public AFIContext(DbContextOptions<AFIContext> options, IConfiguration configuration)
        : base(options)
        {
            this.configuration = configuration;
        }


        public virtual DbSet<PolicyHolder> PolicyHolder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration["databaseconnectionstring"]);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-1337");


            base.OnModelCreating(modelBuilder);
        }



        #region Entity creation
        private ModelBuilder ConfigurePolicyHolder(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureBaseEntity<PolicyHolder>("PolicyHolder", "app")
                .Entity<PolicyHolder>(entity =>
                {
                    entity.HasIndex(e => e.PolicyId);
                    entity.Property(e => e.PolicyId)
                    .IsRequired();

                    entity.Property(e => e.DateOfBirth)
                    .IsRequired(false);


                    entity.Property(e => e.EMail)
                    .IsRequired(false);

                    entity.Property(e => e.Forename)
                    .IsRequired()
                    .HasMaxLength(50);

                    entity.Property(e => e.PolicyNumber)
                    .IsRequired()
                    .HasMaxLength(9);

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
