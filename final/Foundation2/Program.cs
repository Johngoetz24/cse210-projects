using System;
using System.Collections.Generic;
using System.IO;


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
