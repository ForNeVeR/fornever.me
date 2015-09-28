using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace EvilPlanner.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<EvilPlannerContext>
    {
        public static void EnableAutoMigration()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EvilPlannerContext, Configuration>());
        }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}