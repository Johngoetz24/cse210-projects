using System;
using System.Collections.Generic;
using System.IO;


public class Product
{
    private string _name;
    private string _productId;
    private double _pricePerUnit;
    private int _quantity;

    public Product(string name, string productId, double pricePerUnit, int quantity)
    {
        _name = name;
        _productId = productId;
        _pricePerUnit = pricePerUnit;
        _quantity = quantity;
    }

    public string GetName() => _name;
    public string GetProductId() => _productId;
    public double GetPrice() => _pricePerUnit;
    public int GetQuantity() => _quantity;

    public double GetTotalCost() => _pricePerUnit * _quantity;
}


public class Address
{
    private string _street;
    private string _city;
    private string _state;
    private string _country;

    public Address(string street, string city, string state, string country)
    {
        _street = street;
        _city = city;
        _state = state;
        _country = country;
    }

    public string GetStreet() => _street;
    public string GetCity() => _city;
    public string GetState() => _state;
    public string GetCountry() => _country;

    public bool IsInUSA() => _country.ToLower().Contains("usa") || _country.ToLower().Contains("united states");

    public string GetFullAddress() => $"{_street}\n{_city}, {_state}\n{_country}";
}

public class Customer
{
    private string _name;
    private Address _address;

    public Customer(string name, Address address)
    {
        _name = name;
        _address = address;
    }

    public string GetName() => _name;
    public Address GetAddress() => _address;
    public bool LivesInUSA() => _address.IsInUSA();
}
public class Order
{
    private List<Product> _products = new List<Product>();
    private Customer _customer;

    public Order(Customer customer)
    {
        _customer = customer;
    }

    public void AddProduct(Product product) => _products.Add(product);

    public List<Product> GetProducts() => _products;
    public Customer GetCustomer() => _customer;

    public double CalculateTotalCost()
    {
        double total = 0;
        foreach (Product p in _products)
            total += p.GetTotalCost();

        total += _customer.LivesInUSA() ? 5 : 35;
        return total;
    }

    public string GetPackingLabel()
    {
        string label = "Packing Label:\n";
        foreach (Product p in _products)
            label += $"{p.GetName()} (ID: {p.GetProductId()})\n";
        return label;
    }

    public string GetShippingLabel()
    {
        return $"Shipping Label:\n{_customer.GetName()}\n{_customer.GetAddress().GetFullAddress()}";
    }
}

class Program
{
    static void Main()
    {
        List<Order> orders = new List<Order>();

        while (true)
        {
            Console.WriteLine("\n1. Load Orders from File");
            Console.WriteLine("2. Create Order Manually");
            Console.WriteLine("3. Generate Random Orders");
            Console.WriteLine("4. Display Orders");
            Console.WriteLine("5. Save Orders to File");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": LoadFromFile(orders); break;
                case "2": orders.Add(CreateOrderFromUser()); break;
                case "3": orders.AddRange(GenerateRandomOrders()); break;
                case "4": DisplayOrders(orders); break;
                case "5": SaveToFile(orders); break;
                case "6": return;
            }
        }
    }
    static void LoadFromFile(List<Order> orders)
    {
        Console.Write("Enter file name: ");
        string file = Console.ReadLine();

        if (!File.Exists(file))
        {
            Console.WriteLine("File not found.");
            return;
        }

        string[] lines = File.ReadAllLines(file);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            Address addr = new Address(parts[1], parts[2], parts[3], parts[4]);
            Customer cust = new Customer(parts[0], addr);
            Order order = new Order(cust);

            string[] products = parts[5].Split(';');
            foreach (string p in products)
            {
                string[] data = p.Split(',');
                order.AddProduct(new Product(data[0], data[3], double.Parse(data[1]), int.Parse(data[2])));
            }

            orders.Add(order);
        }

        Console.WriteLine("Orders loaded successfully.");
    }

    static void SaveToFile(List<Order> orders)
    {
        Console.Write("Enter file name to save: ");
        string file = Console.ReadLine();

        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (Order order in orders)
            {
                Customer c = order.GetCustomer();
                Address a = c.GetAddress();

                string line = $"{c.GetName()}|{a.GetStreet()}|{a.GetCity()}|{a.GetState()}|{a.GetCountry()}|";

                List<string> productParts = new List<string>();
                foreach (Product p in order.GetProducts())
                {
                    productParts.Add($"{p.GetName()},{p.GetPrice()},{p.GetQuantity()},{p.GetProductId()}");
                }

                line += string.Join(";", productParts);
                writer.WriteLine(line);
            }
        }

        Console.WriteLine("Orders saved successfully.");
    }

    static Order CreateOrderFromUser()
    {
        Console.Write("Customer name: ");
        string name = Console.ReadLine();

        Console.Write("Street: "); string street = Console.ReadLine();
        Console.Write("City: "); string city = Console.ReadLine();
        Console.Write("State: "); string state = Console.ReadLine();
        Console.Write("Country: "); string country = Console.ReadLine();

        Address addr = new Address(street, city, state, country);
        Customer cust = new Customer(name, addr);
        Order order = new Order(cust);

        while (true)
        {
            Console.Write("Product name (or 'done'): ");
            string pname = Console.ReadLine();
            if (pname.ToLower() == "done") break;

            Console.Write("Product ID: "); string id = Console.ReadLine();
            Console.Write("Price: "); double price = double.Parse(Console.ReadLine());
            Console.Write("Quantity: "); int qty = int.Parse(Console.ReadLine());

            order.AddProduct(new Product(pname, id, price, qty));
        }

        return order;
    }

    static List<Order> GenerateRandomOrders()
    {
        Random rand = new Random();
        List<Order> orders = new List<Order>();

        string[] names = { "John", "Woody", "Allen" };
        string[] countries = { "USA", "Canada", "UK" };

        for (int i = 0; i < 2; i++)
        {
            Address addr = new Address("Banana St", "Westfield", "KY", countries[rand.Next(countries.Length)]);
            Customer cust = new Customer(names[rand.Next(names.Length)], addr);
            Order order = new Order(cust);

            int productCount = rand.Next(2, 4);
            for (int j = 0; j < productCount; j++)
            {
                order.AddProduct(new Product(
                    "Item" + rand.Next(100),
                    "ID" + rand.Next(1000),
                    rand.Next(10, 200),
                    rand.Next(1, 5)));
            }

            orders.Add(order);
        }

        Console.WriteLine("Random orders generated.");
        return orders;
    }

    static void DisplayOrders(List<Order> orders)
    {
        foreach (Order order in orders)
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine(order.GetPackingLabel());
            Console.WriteLine(order.GetShippingLabel());
            Console.WriteLine($"Total Cost: ${order.CalculateTotalCost()}");
        }
    }
}
