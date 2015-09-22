namespace EvilPlanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Quotations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Quotations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Text = c.String(),
                        Source = c.String(),
                        SourceUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Quotations");
        }
    }
}
