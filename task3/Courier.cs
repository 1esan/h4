using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Courier
    {
        public int Id { get; set; } 
        public int Capacity { get; set; } 
        public bool IsBusy { get; set; } 
        public List<Order> CurrentOrders { get; set; } 
    }
}
