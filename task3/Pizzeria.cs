using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace task3
{
    public class Pizzeria
    {
        public List<Baker> Bakers { get; set; } // Список пекарей
        public List<Courier> Couriers { get; set; } // Список курьеров
        public Warehouse Warehouse { get; set; } // Склад готовой продукции
        public Queue<Order> Orders { get; set; } // Очередь заказов
        public int MaxTime { get; set; } // Максимальное время выполнения заказа в минутах
        public int TotalOrders { get; set; } // Общее количество заказов
        public int FreeOrders { get; set; } // Количество бесплатных заказов
        public int LostOrders { get; set; } // Количество потерянных заказов

        public Pizzeria(int n, int m, int t, int maxTime)
        {
            // Инициализация пекарей, курьеров, склада и очереди заказов
            Bakers = new List<Baker>();
            Couriers = new List<Courier>();
            Warehouse = new Warehouse(t);
            Orders = new Queue<Order>();
            MaxTime = maxTime;
            TotalOrders = 0;
            FreeOrders = 0;
            LostOrders = 0;

            // Загрузка параметров работников из файла JSON
            var json = File.ReadAllText("workers.json");
            var workers = JsonSerializer.Deserialize<dynamic>(json);

            // Добавление пекарей в список
            for (int i = 0; i < n; i++)
            {
                var experience = (int)workers.bakersi.experience;
                Bakers.Add(new Baker(i + 1, experience));
            }

            // Добавление курьеров в список
            for (int i = 0; i < m; i++)
            {
                var capacity = (int)workers.couriersi.capacity;
                Couriers.Add(new Courier(i + 1, capacity));
            }
        }
        // Метод, который генерирует случайный заказ и добавляет его в очередь
        public void GenerateOrder()
        {
            var random = new Random();
            var id = ++TotalOrders; // Увеличение номера заказа
            var time = random.Next(5, 15); // Случайное время выполнения заказа
            var order = new Order(id, time); // Создание заказа
            lock (Orders) // Блокировка очереди
            {
                Orders.Enqueue(order); // Добавление заказа в очередь
            }
            Console.WriteLine($"[{order.Id}], [поступил в общую очередь]");
        }

        // Метод, который запускает производственный процесс
        public async Task Run()
        {
            // Запуск таймера, который генерирует заказы каждые 10 секунд
            var timer = new Timer(state =>
            {
                GenerateOrder(); // Генерация заказа
            }, null, 0, 10000);

            // Запуск задач, которые выполняют пекари и курьеры
            var tasks = new List<Task>();
            foreach (var baker in Bakers)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (true) // Пока есть заказы
                    {
                        Order order = null;
                        lock (Orders) // Блокировка очереди
                        {
                            if (Orders.Count > 0) // Если есть заказ в очереди
                            {
                                order = Orders.Dequeue(); // Взятие заказа из очереди
                            }
                        }
                        if (order != null) // Если заказ взят
                        {
                            await baker.Bake(order, Warehouse); // Выполнение заказа пекарем
                        }
                        else // Если заказов нет
                        {
                            break; // Завершение работы
                        }
                    }
                }));
            }
            foreach (var courier in Couriers)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (true) // Пока есть пиццы на складе
                    {
                        if (Warehouse.Orders.Count > 0) // Если есть пицца на складе
                        {
                            await courier.Deliver(Warehouse); // Доставка пиццы курьером
                        }
                        else // Если пицц нет
                        {
                            break; // Завершение работы
                        }
                    }
                }));
            }

            // Ожидание завершения всех задач
            await Task.WhenAll(tasks);

            // Остановка таймера
            timer.Dispose();

            // Анализ выполнения заказов и рекомендации владельцу
            Analyze();
        }

        // Метод, который анализирует выполнение заказов и дает рекомендации владельцу
        public void Analyze()
        {
            // Подсчет количества бесплатных и потерянных заказов
            foreach (var order in Orders)
            {
                if (!order.IsDone) // Если заказ не выполнен
                {
                    LostOrders++; // Увеличение количества потерянных заказов
                }
                else if (order.Time * 2 > MaxTime) // Если заказ выполнен с опозданием
                {
                    FreeOrders++; // Увеличение количества бесплатных заказов
                    order.IsFree = true; // Отметка заказа как бесплатного
                }
            }

            // Вывод статистики по заказам
            Console.WriteLine($"Всего заказов: {TotalOrders}");
            Console.WriteLine($"Бесплатных заказов: {FreeOrders}");
            Console.WriteLine($"Потерянных заказов: {LostOrders}");

            // Вывод рекомендаций владельцу
            Console.WriteLine("Рекомендации владельцу:");
            if (FreeOrders > TotalOrders * 0.1) // Если более 10% заказов бесплатные
            {
                Console.WriteLine("- увеличить максимальное время выполнения заказа");
            }
            if (LostOrders > TotalOrders * 0.1) // Если более 10% заказов потерянные
            {
                Console.WriteLine("- увеличить количество заказов");
            }
            if (Warehouse.Orders.Count > 0) // Если на складе остались пиццы
            {
                Console.WriteLine("- расширить склад");
            }
            if (Bakers.Any(b => !b.IsBusy)) // Если есть свободные пекари
            {
                Console.WriteLine("- уволить пекаря (i)");
            }
            if (Couriers.Any(c => !c.IsBusy)) // Если есть свободные курьеры
            {
                Console.WriteLine("- уволить курьера (i)");
            }
        }
    }
}