using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Baker
    {
        public int Id { get; set; } // Номер пекаря
        public int Experience { get; set; } // Опыт работы в минутах
        public bool IsBusy { get; set; } // Состояние занятости (занят или нет)

        public Baker(int id, int experience)
        {
            Id = id;
            Experience = experience;
            IsBusy = false;
        }

        // Метод, который берет заказ в исполнение и готовит пиццу
        public async Task Bake(Order order, Warehouse warehouse)
        {
            IsBusy = true; // Пекарь занят
            Console.WriteLine($"[{order.Id}], [взят в исполнение пекарем {Id}]");
            await Task.Delay(order.Time * Experience); // Имитация процесса готовки
            Console.WriteLine($"[{order.Id}], [готов к передаче на склад]");
            await warehouse.Store(order); // Передача пиццы на склад
            IsBusy = false; // Пекарь свободен
        }
    }
}
