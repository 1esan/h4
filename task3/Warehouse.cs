using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Warehouse
    {
        public int Size { get; set; } 
        public Queue<Order> Orders { get; set; } 
    }
}
