using System.Data.Entity;
using System.Data.Entity.Migrations;
using EvilPlanner.Data.Entities;

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

        protected override void Seed(EvilPlannerContext context)
        {
            context.Quotations.AddOrUpdate(
                q => q.Text,
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "After I captures the hero's super-weapon, I will not immediately disband my legions and relax my guard because I believe whoever holds the weapon is unstoppable.  After all, the hero held the weapon and I took it from him."
                },
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "All bumbling conjurers, clumsy squires, no - talent bards, and cowardly thieves in the land will be preemptively executed; all annoying and / or humorously clever robots and androids will be destroyed; and it shall be declared a capital crime to be the \"town drunk\". The hero will certainly give up and abandon his quest if he has no handy source of comic relief."
                },
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "All deathtraps will have only one way in or out, with any way out leading to an even more cunning deathtrap that works faster."
                },
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "All guards (and other workers) will be entitled to three weeks paid vacation a year after one full year's employment, will be covered (after that same period) by comprehensive medical and dental insurance (paid by me, the Evil Overlord) for themselves and spouse/companion and all dependents.  They will have regularly scheduled pay-raises for every five years with which they remain in my employ as well as annual, merit-based bonuses.  Stock options and retirement plans will be made available after five years of employment along with favorably termed loans for home improvement, education and debt consolidation.  Any employee disabled in my service will receive a lifetime pension.  Every year, my organization will make a few sizable college scholarships available for the most qualified of the dependents of my employees.  Upon leaving my employ they will be constrained from working for any competitor or adversary for a period of not less than five years.  All dismissals (as opposed to termination on their part) will be accompanied by a payment of one month's salary as termination pay and an excellent recommendation(regardless of cause for dismissal).Good will is more valuable than terror on the part of my employees."
                },
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "All midwives will be banned from the realm.  All babies will be delivered at state - approved hospitals.Orphans will be placed in foster - homes, not abandoned in the woods to be raised by creatures of the wild."
                },
                new Quotation
                {
                    Source = "The Evil Overlord List",
                    SourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html",
                    Text =
                        "All my computer systems will have uninterruptable power supplies.All my circuitry will use breakers or fuses of the appropriate tolerances."
                });
        }
    }
}