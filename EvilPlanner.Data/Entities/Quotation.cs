using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilPlanner.Data.Entities
{
    public class Quotation
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public string SourceUrl { get; set; }
    }
}
