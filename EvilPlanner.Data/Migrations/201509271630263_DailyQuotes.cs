namespace EvilPlanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyQuotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyQuotes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Quotation_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quotations", t => t.Quotation_Id)
                .Index(t => t.Date)
                .Index(t => t.Quotation_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyQuotes", "Quotation_Id", "dbo.Quotations");
            DropIndex("dbo.DailyQuotes", new[] { "Quotation_Id" });
            DropIndex("dbo.DailyQuotes", new[] { "Date" });
            DropTable("dbo.DailyQuotes");
        }
    }
}
