using System.Data.Entity.Migrations;

namespace EvilPlanner.Data.Migrations
{
    public partial class QuotationCleanup : DbMigration
    {
        public override void Up()
        {
            Sql(@"delete dq
from DailyQuotes dq
join Quotations q on q.Id = dq.Quotation_Id
where q.Text = N''

delete from Quotations where Text = N'';

with properQuotes as (
    select q.Id FromId, (
        select min(Id)
        from Quotations q2
        where q2.Text = q.Text
    ) ToId
    from Quotations q
)
update dq
set Quotation_Id = pq.ToId
from DailyQuotes dq
join properQuotes pq on pq.FromId = dq.Quotation_Id;

with properQuotes as (
    select q.Id FromId, (
        select min(Id)
        from Quotations q2
        where q2.Text = q.Text
    ) ToId
    from Quotations q
)
delete q
from Quotations q
join properQuotes pq on pq.FromId = q.Id
where q.Id <> pq.ToId

update DailyQuotes
set Quotation_Id = 435
where Quotation_Id = 215

delete from Quotations
where Id = 215
");
        }

        public override void Down()
        {
            // Too complex to be rolled back.
        }
    }
}
