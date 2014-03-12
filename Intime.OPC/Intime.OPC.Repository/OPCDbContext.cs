using Intime.OPC.Domain;
using System.Data.Entity;

namespace Intime.OPC.Repository
{
    public class OPCDbContext : DbContext
    {
        public OPCDbContext()
            : base("OPC_DBConnectionString")
        {
        }

        public DbSet<AuthUser> OPC_AuthUser { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthUser>().ToTable("OPC_AuthUser");
            base.OnModelCreating(modelBuilder);
        }
    }
}
