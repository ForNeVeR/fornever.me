using System.Data.Entity.Migrations;

namespace EvilPlanner.Data.Migrations
{
    public partial class FixQuoteLists : DbMigration
    {
        public override void Up()
        {
            Sql(@"update Quotations set Text = N'I will not resort to android duplicates to safeguard myself from capture by my enemies because: (a) What I can construct others can emulate. If my minions are familiar with the use of androids they may make the mistake of letting the wrong one past their guard.' where Id = 87
update Quotations set Text = N'I will not resort to android duplicates to safeguard myself from capture by my enemies because: (b) My enemies can capture and reprogram one for the same effect.' where Id = 306
insert into Quotations (Text, Source, SourceUrl)
values (N'I will not resort to android duplicates to safeguard myself from capture by my enemies because: (c) Any android can at any time decide that humans are inferior and commence extermination. Handing a killer android an already-assembled international conspiracy is considered ""bad form"".', N'The Evil Overlord List', N'http://legendspbem.angelfire.com/eviloverlordlist.html')");
        }

        public override void Down()
        {
            Sql(@"update Quotations set Text = N'I will not resort to android duplicates to safeguard myself from capture by my enemies because:' where Id in (87, 306)
delete dq
from DailyQuotes dq
join Quotations q on q.Id = dq.Quotation_Id
where q.Text = N'I will not resort to android duplicates to safeguard myself from capture by my enemies because: (c) Any android can at any time decide that humans are inferior and commence extermination. Handing a killer android an already-assembled international conspiracy is considered ""bad form"".'
delete from Quotations where Text = N'I will not resort to android duplicates to safeguard myself from capture by my enemies because: (c) Any android can at any time decide that humans are inferior and commence extermination. Handing a killer android an already-assembled international conspiracy is considered ""bad form"".'
");
        }
    }
}
