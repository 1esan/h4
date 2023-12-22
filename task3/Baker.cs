using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Baker
    {
        public int Id { get; set; }
        public int Experience { get; set; }
        public bool isBusy { get; set; }
        public Order CurrentOrder { get; set; } 
    }
}
