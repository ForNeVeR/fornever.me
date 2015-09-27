using System.Data.Entity;
using EvilPlanner.Data.Entities;

namespace EvilPlanner.Data
{
    public class EvilPlannerContext : DbContext
    {
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<DailyQuote> DailyQuotes { get; set; }

        public EvilPlannerContext()
        {
        }

        public EvilPlannerContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}
