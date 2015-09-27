using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvilPlanner.Data.Entities
{
    public class DailyQuote
    {
        public long Id { get; set; }

        [Index]
        public DateTime Date { get; set; }

        public Quotation Quotation { get; set; }
    }
}
