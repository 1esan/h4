using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    public class Warehouse
    {
        public int Size { get; set; } // Размер склада в пиццах
        public Queue<Order> Orders { get; set; } // Очередь готовых пицц
        public SemaphoreSlim Semaphore { get; set; } // Семафор для синхронизации доступа к складу

        public Warehouse(int size)
        {
            Size = size;
            Orders = new Queue<Order>();
            Semaphore = new SemaphoreSlim(size, size); // Изначально склад пуст
        }

        // Метод, который пытается зарезервировать место на складе и передать туда пиццу
        public async Task Store(Order order)
        {
            await Semaphore.WaitAsync(); // Ожидание свободного места
            lock (Orders) // Блокировка очереди
            {
                Orders.Enqueue(order); // Добавление пиццы в очередь
            }
            Console.WriteLine($"[{order.Id}], [передан на склад]");
        }
        // Метод, который пытается взять одну или несколько пицц со склада
        public async Task<List<Order>> Take(int capacity)
        {
            var pizzas = new List<Order>(); // Список взятых пицц
            while (capacity > 0) // Пока есть место в багажнике
            {
                await Semaphore.WaitAsync(); // Ожидание готовой пиццы
                lock (Orders) // Блокировка очереди
                {
                    var order = Orders.Dequeue(); // Извлечение пиццы из очереди
                    pizzas.Add(order); // Добавление пиццы в список
                }
                capacity--; // Уменьшение места в багажнике
            }
            return pizzas; // Возврат списка пицц
        }
    }
}
