using task3;

class Program
{
    
    const int MAX_ORDERS = 100; 
    const int MAX_TIME = 30; 
    const int MIN_BAKER_EXPERIENCE = 1; 
    const int MAX_BAKER_EXPERIENCE = 5; 
    const int MIN_COURIER_CAPACITY = 1; 
    const int MAX_COURIER_CAPACITY = 5; 
    const int MIN_ORDER_TIME = 5; 
    const int MAX_ORDER_TIME = 15; 
    const int MIN_DELIVERY_TIME = 10; 
    const int MAX_DELIVERY_TIME = 20; 

    
    static Random random = new Random(); 
    static Queue<Order> orderQueue = new Queue<Order>(); 
    static Warehouse warehouse; 
    static Workers workers; 
    static SemaphoreSlim orderSemaphore = new SemaphoreSlim(1); 
    static SemaphoreSlim warehouseSemaphore = new SemaphoreSlim(1); 
    static SemaphoreSlim consoleSemaphore = new SemaphoreSlim(1); 
    static bool isWorking = true; 

    static void Main(string[] args)
    {
        
        workers = new Workers();
        workers.N = random.Next(1, 10); 
        workers.M = random.Next(1, 10); 
        workers.T = random.Next(10, 50); 
        workers.Bakers = new List<Baker>(); 
        workers.Couriers = new List<Courier>(); 

        
        for (int i = 0; i < workers.N; i++)
        {
            Baker baker = new Baker();
            baker.Id = i + 1; 
            baker.Experience = random.Next(MIN_BAKER_EXPERIENCE, MAX_BAKER_EXPERIENCE + 1); 
            baker.isBusy = false; 
            baker.CurrentOrder = null; 
            workers.Bakers.Add(baker); 
        }

        
        for (int i = 0; i < workers.M; i++)
        {
            Courier courier = new Courier();
            courier.Id = i + 1; 
            courier.Capacity = random.Next(MIN_COURIER_CAPACITY, MAX_COURIER_CAPACITY + 1); 
            courier.IsBusy = false; 
            courier.CurrentOrders = new List<Order>(); 
            workers.Couriers.Add(courier); 
        }

        
        warehouse = new Warehouse();
        warehouse.Size = workers.T; 
        warehouse.Orders = new Queue<Order>(); 

        Console.WriteLine("Параметры работников пиццерии:");
        Console.WriteLine($"Количество пекарей: {workers.N}");
        Console.WriteLine($"Количество курьеров: {workers.M}");
        Console.WriteLine($"Размер склада: {workers.T}");
        Console.WriteLine();

        
        Thread orderThread = new Thread(GenerateOrders);
        Thread[] bakerThreads = new Thread[workers.N];
        Thread[] courierThreads = new Thread[workers.M];

        for (int i = 0; i < workers.N; i++)
        {
            bakerThreads[i] = new Thread(BakePizza);
            bakerThreads[i].Start(i);
        }
        for (int i = 0; i < workers.M; i++)
        {
            courierThreads[i] = new Thread(DeliverPizza);
            courierThreads[i].Start(i);
        }
        orderThread.Start();

        
        Console.WriteLine("Нажмите любую клавишу, чтобы завершить работу пиццерии.");
        Console.ReadKey();
        isWorking = false; 

        
        orderThread.Join();
        for (int i = 0; i < workers.N; i++)
        {
            bakerThreads[i].Join();
        }
        for (int i = 0; i < workers.M; i++)
        {
            courierThreads[i].Join();
        }

        
        AnalyzeOrders();
    }

    static void GenerateOrders()
    {
        
        int orderCount = random.Next(1, MAX_ORDERS + 1);
        consoleSemaphore.Wait(); 
        Console.WriteLine($"Количество заказов: {orderCount}");
        Console.WriteLine();
        consoleSemaphore.Release(); 

        
        for (int i = 0; i < orderCount; i++)
        {
            
            if (!isWorking) break;

            
            Order order = new Order();
            order.Id = i + 1; 
            order.Time = random.Next(MIN_ORDER_TIME, MAX_ORDER_TIME + 1); 
            order.IsDone = false; 
            order.IsFree = false; 

            
            orderSemaphore.Wait(); 
            orderQueue.Enqueue(order); 
            orderSemaphore.Release(); 

            
            consoleSemaphore.Wait(); 
            Console.WriteLine($"[{order.Id}], [поступил]");
            consoleSemaphore.Release(); 
        }
    }
    
    static void BakePizza(object index)
    {
        
        int bakerId = (int)index;

        
        Baker baker = workers.Bakers[bakerId];

        
        while (isWorking)
        {
            
            if (!baker.isBusy)
            {
                
                orderSemaphore.Wait(); 
                if (orderQueue.Count > 0) 
                {
                    
                    baker.CurrentOrder = orderQueue.Dequeue(); 
                    baker.isBusy = true; 
                    orderSemaphore.Release(); 

                    
                    consoleSemaphore.Wait(); 
                    Console.WriteLine($"[{baker.CurrentOrder.Id}], [взят в работу пекарем {baker.Id}]");
                    consoleSemaphore.Release(); 
                }
                else 
                {
                    orderSemaphore.Release(); 
                    continue; 
                }
            }

            
            if (baker.isBusy)
            {
                
                Thread.Sleep((baker.CurrentOrder.Time + baker.Experience) * 1000); 
                warehouseSemaphore.Wait(); 
                if (warehouse.Orders.Count < warehouse.Size)  
                {
                    
                    warehouse.Orders.Enqueue(baker.CurrentOrder); 
                    baker.isBusy = false; 
                    baker.CurrentOrder.IsDone = true; 
                    warehouseSemaphore.Release(); 
                    consoleSemaphore.Wait(); 
                    Console.WriteLine($"[{baker.CurrentOrder.Id}], [передан на склад пекарем {baker.Id}]");
                    consoleSemaphore.Release(); 
                }
                else 
                {
                    warehouseSemaphore.Release();  
                    continue; 
                }
            }
        }
    }
    static void DeliverPizza(object index)
    {
        
        int courierId = (int)index;

        
        Courier courier = workers.Couriers[courierId];

        
        while (isWorking)
        {
            
            if (!courier.IsBusy)
            {
                
                warehouseSemaphore.Wait(); 
                if (warehouse.Orders.Count > 0) 
                {
                    
                    int ordersCount = random.Next(1, Math.Min(courier.Capacity, warehouse.Orders.Count) + 1); 
                    for (int i = 0; i < ordersCount; i++)
                    {
                        
                        Order order = warehouse.Orders.Dequeue(); 
                        courier.CurrentOrders.Add(order); 
                    }
                    courier.IsBusy = true; 
                    warehouseSemaphore.Release(); 

                    
                    consoleSemaphore.Wait(); 
                    foreach (Order order in courier.CurrentOrders) 
                    {
                        Console.WriteLine($"[{order.Id}], [взят в доставку курьером {courier.Id}]");
                    }
                    consoleSemaphore.Release(); 
                }
                else 
                {
                    warehouseSemaphore.Release();
                    continue; 
                }
            }

            
            if (courier.IsBusy)
            {
                
                Thread.Sleep(random.Next(MIN_DELIVERY_TIME, MAX_DELIVERY_TIME + 1) * 1000); 
                consoleSemaphore.Wait(); 
                foreach (Order order in courier.CurrentOrders) 
                {
                    
                    Baker baker = workers.Bakers.Find(b => b.CurrentOrder.Id == order.Id); 
                    if (baker != null) 
                    {
                        if (order.Time + baker.Experience + MIN_DELIVERY_TIME <= MAX_TIME) 
                        {
                            Console.WriteLine($"[{order.Id}], [выполнен]");
                        }
                        else 
                        {
                            Console.WriteLine($"[{order.Id}], [выполнен бесплатно]");
                            order.IsFree = true; 
                        }
                    }
                    else 
                    {
                        Console.WriteLine($"[{order.Id}], [ошибка: не найден пекарь, выполнивший заказ]");
                    }
                }
                consoleSemaphore.Release(); 
                courier.CurrentOrders.Clear();
                courier.IsBusy = false; 
            }
        }
    }
    static void AnalyzeOrders()
    {
        int doneOrders = 0; 
        int freeOrders = 0; 
        foreach (Baker baker in workers.Bakers) 
        {
            if (baker.CurrentOrder != null && baker.CurrentOrder.IsDone) 
            {
                doneOrders++; 
                if (baker.CurrentOrder.IsFree) 
                {
                    freeOrders++; 
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine($"Количество выполненных заказов: {doneOrders}");
        Console.WriteLine($"Количество бесплатных заказов: {freeOrders}");
        double freePercent = (double)freeOrders / doneOrders * 100;
        Console.WriteLine();
        Console.WriteLine("Рекомендации владельцу:");
        if (freePercent > 10) 
        {
            Console.WriteLine("- Увеличить скорость работы пекарей и курьеров, чтобы снизить процент бесплатных заказов.");
        }
        if (orderQueue.Count > 0) 
        {
            Console.WriteLine($"- Увеличить количество пекарей, чтобы справиться с поступающими заказами. В очереди осталось {orderQueue.Count} заказов.");
        }
        if (warehouse.Orders.Count > 0) 
        {
            Console.WriteLine($"- Увеличить количество курьеров, чтобы доставить все заказы со склада. На складе осталось {warehouse.Orders.Count} заказов.");
        }
        if (warehouse.Orders.Count == warehouse.Size) 
        {
            Console.WriteLine("- Расширить склад, чтобы увеличить его вместимость и избежать задержек в производстве пиццы.");
        }
    }
}
 
