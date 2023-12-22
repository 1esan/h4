using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Order
    {
        public int Id { get; set; } // Номер заказа
        public int Time { get; set; } // Время выполнения заказа в минутах
        public bool IsDone { get; set; } // Состояние заказа (выполнен или нет)
        public bool IsFree { get; set; } // Состояние оплаты (бесплатный или нет)

        public Order(int id, int time)
        {
            Id = id;
            Time = time;
            IsDone = false;
            IsFree = false;
        }
    }
}
