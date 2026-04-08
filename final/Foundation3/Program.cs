using System;
using System.Collections.Generic;
using System.IO;


public class Address
{
    private string _street, _city, _state, _country;

    public Address(string street, string city, string state, string country)
    {
        _street = street;
        _city = city;
        _state = state;
        _country = country;
    }

    public string GetFullAddress()
    {
        return $"{_street}, {_city}, {_state}, {_country}";
    }

    public string ToFileString()
    {
        return $"{_street}|{_city}|{_state}|{_country}";
    }

    public static Address FromString(string data)
    {
        string[] parts = data.Split('|');
        return new Address(parts[0], parts[1], parts[2], parts[3]);
    }
}


public class Event
{
    private string _title, _description, _date, _time;
    private Address _address;

    public Event(string title, string desc, string date, string time, Address address)
    {
        _title = title;
        _description = desc;
        _date = date;
        _time = time;
        _address = address;
    }

    public virtual string GetTypeName() => "Event";

    public string GetStandardDetails()
    {
        return $"{_title}\n{_description}\nDate: {_date} Time: {_time}\n{_address.GetFullAddress()}";
    }

    public virtual string GetFullDetails()
    {
        return GetStandardDetails();
    }

    public virtual string GetShortDescription()
    {
        return $"{GetTypeName()} - {_title} on {_date}";
    }

    public virtual string ToFileString()
    {
        return $"{GetTypeName()}~{_title}~{_description}~{_date}~{_time}~{_address.ToFileString()}";
    }

    protected string Title => _title;
    protected string Date => _date;
}

public class Lecture : Event
{
    private string _speaker;
    private int _capacity;

    public Lecture(string title, string desc, string date, string time, Address addr, string speaker, int capacity)
        : base(title, desc, date, time, addr)
    {
        _speaker = speaker;
        _capacity = capacity;
    }

    public override string GetTypeName() => "Lecture";

    public override string GetFullDetails()
    {
        return $"{GetStandardDetails()}\nType: Lecture\nSpeaker: {_speaker}\nCapacity: {_capacity}";
    }

    public override string ToFileString()
    {
        return $"{base.ToFileString()}~{_speaker}~{_capacity}";
    }
}

public class Reception : Event
{
    private string _email;

    public Reception(string title, string desc, string date, string time, Address addr, string email)
        : base(title, desc, date, time, addr)
    {
        _email = email;
    }

    public override string GetTypeName() => "Reception";

    public override string GetFullDetails()
    {
        return $"{GetStandardDetails()}\nType: Reception\nRSVP: {_email}";
    }

    public override string ToFileString()
    {
        return $"{base.ToFileString()}~{_email}";
    }
}

public class Outdoor : Event
{
    private string _weather;

    public Outdoor(string title, string desc, string date, string time, Address addr, string weather)
        : base(title, desc, date, time, addr)
    {
        _weather = weather;
    }

    public override string GetTypeName() => "Outdoor";

    public override string GetFullDetails()
    {
        return $"{GetStandardDetails()}\nType: Outdoor\nWeather: {_weather}";
    }

    public override string ToFileString()
    {
        return $"{base.ToFileString()}~{_weather}";
    }
}

class Program
{
    static List<Event> events = new List<Event>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1. Create Event");
            Console.WriteLine("2. Display Events");
            Console.WriteLine("3. Save");
            Console.WriteLine("4. Load");
            Console.WriteLine("5. Random Event");
            Console.WriteLine("6. Exit");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": CreateEvent(); break;
                case "2": DisplayEvents(); break;
                case "3": Save(); break;
                case "4": Load(); break;
                case "5": GenerateRandom(); break;
                case "6": return;
            }
        }
    }


    static void CreateEvent()
    {
        Console.Write("Type (1=Lecture,2=Reception,3=Outdoor): ");
        string type = Console.ReadLine();

        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Description: ");
        string desc = Console.ReadLine();

        Console.Write("Date: ");
        string date = Console.ReadLine();

        Console.Write("Time: ");
        string time = Console.ReadLine();

        Address addr = CreateAddress();

        if (type == "1")
        {
            Console.Write("Speaker: ");
            string speaker = Console.ReadLine();

            Console.Write("Capacity: ");
            int cap = int.Parse(Console.ReadLine());

            events.Add(new Lecture(title, desc, date, time, addr, speaker, cap));
        }
        else if (type == "2")
        {
            Console.Write("RSVP Email: ");
            string email = Console.ReadLine();

            events.Add(new Reception(title, desc, date, time, addr, email));
        }
        else
        {
            Console.Write("Weather: ");
            string weather = Console.ReadLine();

            events.Add(new Outdoor(title, desc, date, time, addr, weather));
        }
    }

    static Address CreateAddress()
    {
        Console.Write("Street: ");
        string s = Console.ReadLine();
        Console.Write("City: ");
        string c = Console.ReadLine();
        Console.Write("State: ");
        string st = Console.ReadLine();
        Console.Write("Country: ");
        string co = Console.ReadLine();

        return new Address(s, c, st, co);
    }

    static void DisplayEvents()
    {
        foreach (Event e in events)
        {
            Console.WriteLine("\n--- STANDARD ---");
            Console.WriteLine(e.GetStandardDetails());

            Console.WriteLine("\n--- FULL ---");
            Console.WriteLine(e.GetFullDetails());

            Console.WriteLine("\n--- SHORT ---");
            Console.WriteLine(e.GetShortDescription());
        }
    }

    static void Save()
    {
        using (StreamWriter sw = new StreamWriter("events.txt"))
        {
            foreach (Event e in events)
            {
                sw.WriteLine(e.ToFileString());
            }
        }
        Console.WriteLine("Saved!");
    }

    static void Load()
{
    events.Clear();

    if (!File.Exists("events.txt"))
    {
        Console.WriteLine("No file found.");
        return;
    }

    foreach (string line in File.ReadAllLines("events.txt"))
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        string[] parts = line.Split('~');

        // 🔴 Prevent crashes
        if (parts.Length < 6)
        {
            Console.WriteLine("Skipping bad line: " + line);
            continue;
        }

        try
        {
            string type = parts[0];
            Address addr = Address.FromString(parts[5]);

            if (type == "Lecture" && parts.Length >= 8)
            {
                events.Add(new Lecture(
                    parts[1], parts[2], parts[3], parts[4],
                    addr,
                    parts[6],
                    int.Parse(parts[7])
                ));
            }
            else if (type == "Reception" && parts.Length >= 7)
            {
                events.Add(new Reception(
                    parts[1], parts[2], parts[3], parts[4],
                    addr,
                    parts[6]
                ));
            }
            else if (type == "Outdoor" && parts.Length >= 7)
            {
                events.Add(new Outdoor(
                    parts[1], parts[2], parts[3], parts[4],
                    addr,
                    parts[6]
                ));
            }
            else
            {
                Console.WriteLine("Invalid event format: " + line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading line:");
            Console.WriteLine(line);
            Console.WriteLine(ex.Message);
        }
    }

    Console.WriteLine($"Loaded {events.Count} events!");
}
    static void GenerateRandom()
    {
        Random r = new Random();

        Address addr = new Address("Banana St", "Nendon", "UT", "USA");

        int type = r.Next(3);

        if (type == 0)
            events.Add(new Lecture("Info Lecture", "Ground breaking topics", "May 1", "10AM", addr, "Dr. Smith", r.Next(50, 200)));

        else if (type == 1)
            events.Add(new Reception("Family Reception", "Celebrating Julie", "June 2", "6PM", addr, "email.email@email.com"));

        else
            events.Add(new Outdoor("All About Wheels", "Show all about transportation", "July 3", "2PM", addr, "Sunny"));

        Console.WriteLine("Random event added!");
    }
}