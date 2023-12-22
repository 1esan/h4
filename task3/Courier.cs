using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Courier
    {
        public int Id { get; set; } // Номер курьера
        public int Capacity { get; set; } // Вместимость багажника в пиццах
        public bool IsBusy { get; set; } // Состояние занятости (занят или нет)

        public Courier(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            IsBusy = false;
        }

        // Метод, который берет пиццы со склада и доставляет их заказчику
        public async Task Deliver(Warehouse warehouse)
        {
            IsBusy = true; // Курьер занят
            var pizzas = await warehouse.Take(Capacity); // Взятие пицц со склада
            foreach (var pizza in pizzas)
            {
                Console.WriteLine($"[{pizza.Id}], [взят в доставку курьером {Id}]");
                await Task.Delay(pizza.Time * 2); // Имитация процесса доставки
                pizza.IsDone = true; // Заказ выполнен
                Console.WriteLine($"[{pizza.Id}], [доставлен заказчику]");
                if (pizza.IsFree) // Если пицца бесплатная
                {
                    Console.WriteLine($"[{pizza.Id}], [бесплатная]");
                }
            }
            IsBusy = false; // Курьер свободен
        }
    }
}
