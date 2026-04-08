using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        List<Activity> activities = new List<Activity>();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n--- Exercise Tracker ---");
            Console.WriteLine("1. Add Activity");
            Console.WriteLine("2. View Activities");
            Console.WriteLine("3. Save to File");
            Console.WriteLine("4. Load from File");
            Console.WriteLine("5. Generate Random Data");
            Console.WriteLine("6. Quit");
            Console.Write("Choose option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    activities.Add(CreateActivityFromUser());
                    break;

                case "2":
                    foreach (Activity a in activities)
                        Console.WriteLine(a.GetSummary());
                    break;

                case "3":
                    SaveToFile(activities);
                    break;

                case "4":
                    activities = LoadFromFile();
                    break;

                case "5":
                    GenerateRandomData(activities);
                    break;

                case "6":
                    running = false;
                    break;
            }
        }
    }

    static Activity CreateActivityFromUser()
    {
        Console.WriteLine("Choose type: 1=Running, 2=Cycling, 3=Swimming");
        string type = Console.ReadLine();

        Console.Write("Enter minutes: ");
        int minutes = int.Parse(Console.ReadLine());

        DateTime date = DateTime.Now;

        switch (type)
        {
            case "1":
                Console.Write("Enter distance (miles): ");
                double dist = double.Parse(Console.ReadLine());
                return new Running(date, minutes, dist);

            case "2":
                Console.Write("Enter speed (mph): ");
                double speed = double.Parse(Console.ReadLine());
                return new Cycling(date, minutes, speed);

            case "3":
                Console.Write("Enter laps: ");
                int laps = int.Parse(Console.ReadLine());
                return new Swimming(date, minutes, laps);

            default:
                Console.WriteLine("Invalid choice.");
                return CreateActivityFromUser();
        }
    }

    static void SaveToFile(List<Activity> activities)
    {
        using (StreamWriter writer = new StreamWriter("activities.txt"))
        {
            foreach (Activity a in activities)
            {
                if (a is Running r)
                    writer.WriteLine($"Running|{r.GetDate()}|{r.GetMinutes()}|{r.GetDistance()}");

                else if (a is Cycling c)
                    writer.WriteLine($"Cycling|{c.GetDate()}|{c.GetMinutes()}|{c.GetSpeed()}");

                else if (a is Swimming s)
                    writer.WriteLine($"Swimming|{s.GetDate()}|{s.GetMinutes()}|{s.GetLaps()}");
            }
        }

        Console.WriteLine("Saved successfully!");
    }

    static List<Activity> LoadFromFile()
    {
        List<Activity> list = new List<Activity>();

        if (!File.Exists("activities.txt"))
        {
            Console.WriteLine("No file found.");
            return list;
        }

        string[] lines = File.ReadAllLines("activities.txt");

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            string type = parts[0];
            DateTime date = DateTime.Parse(parts[1]);
            int minutes = int.Parse(parts[2]);

            switch (type)
            {
                case "Running":
                    list.Add(new Running(date, minutes, double.Parse(parts[3])));
                    break;

                case "Cycling":
                    list.Add(new Cycling(date, minutes, double.Parse(parts[3])));
                    break;

                case "Swimming":
                    list.Add(new Swimming(date, minutes, int.Parse(parts[3])));
                    break;
            }
        }

        Console.WriteLine("Loaded successfully!");
        return list;
    }

    static void GenerateRandomData(List<Activity> activities)
    {
        Random rand = new Random();

        for (int i = 0; i < 5; i++)
        {
            int type = rand.Next(1, 4);
            int minutes = rand.Next(20, 60);
            DateTime date = DateTime.Now.AddDays(-rand.Next(0, 30));

            switch (type)
            {
                case 1:
                    activities.Add(new Running(date, minutes, rand.Next(1, 6)));
                    break;

                case 2:
                    activities.Add(new Cycling(date, minutes, rand.Next(10, 20)));
                    break;

                case 3:
                    activities.Add(new Swimming(date, minutes, rand.Next(10, 40)));
                    break;
            }
        }

        Console.WriteLine("Random data added!");
    }
    static int GetLaps(Swimming s)
    {

        return (int)(s.GetDistance() / 0.031);
    }
}