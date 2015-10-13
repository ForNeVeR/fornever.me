using System.Data.Entity.Migrations;

namespace EvilPlanner.Data.Migrations
{
    public partial class UniqueDailyQuotes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DailyQuotes", new[] { "Date" });
            CreateIndex("dbo.DailyQuotes", "Date", unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.DailyQuotes", new[] { "Date" });
            CreateIndex("dbo.DailyQuotes", "Date");
        }
    }
}
