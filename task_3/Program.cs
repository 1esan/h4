using System.Text.Json;

public class Baker
{
    public int Id { get; set; }
    public int Experience { get; set; }
    public bool isFree { get; set; }
    public Order CurrentOrder { get; set; }
}
public class Order
{
    public int Id { get; set; } 
    public int Time { get; set; } 
    public bool IsDone { get; set; }
    public bool IsFree { get; set; } 
}
public class Courier
{
    public int Id { get; set; }
    public bool isFree { get; set; }
    public int Capacity { get; set; }
    public List<Order> CurrentOrders { get; set; }
}
public class Warehouse
{
    public int Size { get; set; }
    public Queue<Order> Orders { get; set; }
}
public class Workers
{
    public int N { get; set; } 
    public int M { get; set; } 
    public int T { get; set; } 
    public List<Baker> Bakers { get; set; }
    public List<Courier> Couriers { get; set; } 
}
public class AutomationSystem
{
    private Queue<Order> orders; 
    private Warehouse warehouse; 
    private Workers workers; 
    private int maxOrderTime; 
    private int totalOrders; 
    private int freeOrders;
    private int deliveredOrders;
    private int totalOrderTime;
    private int totalDeliveryTime; 
    private object lockObject; 

    
    public AutomationSystem(string fileName, int maxOrderTime)
    {
        
        string json = File.ReadAllText(fileName);
        workers = JsonSerializer.Deserialize<Workers>(json);

        // Инициализируем очередь заказов, склад и статистику
        orders = new Queue<Order>();
        warehouse = new Warehouse { Size = workers.T, Orders = new Queue<Order>() };
        this.maxOrderTime = maxOrderTime;
        totalOrders = 0;
        freeOrders = 0;
        deliveredOrders = 0;
        totalOrderTime = 0;
        totalDeliveryTime = 0;
        lockObject = new object();
    }