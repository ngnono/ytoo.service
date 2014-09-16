using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using com.intime.fashion.data.tmall.Models.Mapping;

namespace com.intime.fashion.data.tmall.Models
{
    public partial class SYS_INFOContext : DbContext
    {
        static SYS_INFOContext()
        {
            Database.SetInitializer<SYS_INFOContext>(null);
        }

        public SYS_INFOContext()
            : base("Name=SYS_INFOContext")
        {
        }

        public DbSet<JDP_TB_ITEM> JDP_TB_ITEM { get; set; }
        public DbSet<JDP_TB_REFUND> JDP_TB_REFUND { get; set; }
        public DbSet<JDP_TB_TRADE> JDP_TB_TRADE { get; set; }
        public DbSet<OrderPushErrorLog> OrderPushErrorLogs { get; set; }
        public DbSet<OrderPushHistory> OrderPushHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new JDP_TB_ITEMMap());
            modelBuilder.Configurations.Add(new JDP_TB_REFUNDMap());
            modelBuilder.Configurations.Add(new JDP_TB_TRADEMap());
            modelBuilder.Configurations.Add(new OrderPushErrorLogMap());
            modelBuilder.Configurations.Add(new OrderPushHistoryMap());
        }
    }
}
