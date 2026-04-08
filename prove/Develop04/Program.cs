
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


// This program demonstrates a simple console-based mindfulness application with three activities: Breathing, Reflection, and Listing. Each activity guides the user through a timed session.
// The program also saves session data to a file and allows users to view past sessions. The activities are implemented using an abstract base class and derived classes for each activity type.
//


//I have implemented the ability to save session data to a file and load it back, allowing users to view their past sessions.
// and I implemented a prompt variety system that randomly cycles through a list of prompts for the Reflection and Listing activities, ensuring a different prompt 
// each time until all have been used before repeating.


abstract class Activity
{
    private int _duration;
    private string _name;
    private string _description;

    public Activity(string name, string description)
    {
        _name = name;
        _description = description;
    }

    public void StartMessage()
    {
        Console.Clear();
        Console.WriteLine($"--- {_name} ---");
        Console.WriteLine(_description);

        Console.Write("\nEnter duration (seconds): ");
        _duration = int.Parse(Console.ReadLine());

        Console.WriteLine("\nPrepare to begin...");
        Spinner(3);
    }

    public void EndMessage()
    {
        Console.WriteLine("\nWell done!");
        Spinner(2);

        Console.WriteLine($"\nYou completed the {_name} for {_duration} seconds.");
        Spinner(3);

        // Save session
        Session session = new Session
        {
            ActivityName = _name,
            Duration = _duration,
            Date = DateTime.Now
        };

        SessionManager.SaveSession(session);
    }

    public int GetDuration()
    {
        return _duration;
    }

    protected void Spinner(int seconds)
    {
        string[] spinner = { "|", "/", "-", "\\" };
        DateTime end = DateTime.Now.AddSeconds(seconds);
        int i = 0;

        while (DateTime.Now < end)
        {
            Console.Write(spinner[i]);
            Thread.Sleep(200);
            Console.Write("\b");
            i = (i + 1) % spinner.Length;
        }
    }

    protected void Countdown(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Console.Write(i);
            Thread.Sleep(1000);
            Console.Write("\b \b");
        }
    }

    public abstract void Run();
}

class RandomCycler
{
    private readonly List<string> _items;
    private Queue<string> _queue;
    private Random _rand = new Random();

    public RandomCycler(List<string> items)
    {
        _items = new List<string>(items);
        Shuffle();
    }

    private void Shuffle()
    {
        List<string> temp = new List<string>(_items);

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int j = _rand.Next(i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        _queue = new Queue<string>(temp);
    }

    public string GetNext()
    {
        if (_queue.Count == 0)
        {
            Shuffle();
        }

        return _queue.Dequeue();
    }
}
class BreathingActivity : Activity
{
    public BreathingActivity() : base(
        "Breathing Activity",
        "This activity will help you relax by guiding your breathing."
    ) { }

    public override void Run()
    {
        StartMessage();

        int elapsed = 0;
        int duration = GetDuration();

        while (elapsed < duration)
        {
            Console.Write("\nBreathe in... ");
            Countdown(4);

            Console.Write("\nBreathe out... ");
            Countdown(4);

            elapsed += 8;
        }

        EndMessage();
    }
}

class ReflectionActivity : Activity
{
    private readonly List<string> _prompts = new List<string>()
    {
        "Think of a time when you stood up for someone else.",
        "Think of a time when you did something really difficult.",
        "Think of a time when you helped someone in need.",
        "Think of a time when you did something truly selfless."
    };

    private readonly List<string> _questions = new List<string>()
    {
        "Why was this meaningful?",
        "Have you done this before?",
        "How did you get started?",
        "How did you feel when it was done?",
        "What made this different?",
        "What did you learn?",
        "How can you apply this again?"
    };

    private readonly RandomCycler _promptCycler;
    private readonly RandomCycler _questionCycler;

    public ReflectionActivity() : base(
        "Reflection Activity",
        "Reflect on times you showed strength and resilience."
    )
    {
        _promptCycler = new RandomCycler(_prompts);
        _questionCycler = new RandomCycler(_questions);
    }

    public override void Run()
    {
        StartMessage();

        Console.WriteLine("\n" + _promptCycler.GetNext());
        Console.WriteLine("\nThink about this...");
        Spinner(5);

        DateTime end = DateTime.Now.AddSeconds(GetDuration());

        while (DateTime.Now < end)
        {
            Console.Write("\n" + _questionCycler.GetNext() + " ");
            Spinner(4);
        }

        EndMessage();
    }
}

class ListingActivity : Activity
{
    private readonly List<string> _prompts = new List<string>()
    {
        "Who are people you appreciate?",
        "What are your personal strengths?",
        "Who have you helped this week?",
        "When have you felt the Holy Ghost this month?",
        "Who are your personal heroes?"
    };

    private readonly RandomCycler _promptCycler;

    public ListingActivity() : base(
        "Listing Activity",
        "List as many positive things as you can."
    )
    {
        _promptCycler = new RandomCycler(_prompts);
    }

    public override void Run()
    {
        StartMessage();

        Console.WriteLine("\n" + _promptCycler.GetNext());

        Console.Write("\nYou may begin in: ");
        Countdown(5);

        int count = 0;
        DateTime end = DateTime.Now.AddSeconds(GetDuration());

        while (DateTime.Now < end)
        {
            Console.Write("> ");
            Console.ReadLine();
            count++;
        }

        Console.WriteLine($"\nYou listed {count} items!");

        EndMessage();
    }
}

class Session
{
    public string ActivityName { get; set; }
    public int Duration { get; set; }
    public DateTime Date { get; set; }

    public override string ToString()
    {
        return $"{Date} | {ActivityName} | {Duration} seconds";
    }

    public string ToFileFormat()
    {
        return $"{Date}|{ActivityName}|{Duration}";
    }

    public static Session FromFileFormat(string line)
    {
        string[] parts = line.Split('|');

        return new Session
        {
            Date = DateTime.Parse(parts[0]),
            ActivityName = parts[1],
            Duration = int.Parse(parts[2])
        };
    }
}

static class SessionManager
{
    private static readonly string _fileName = "sessions.txt";

    public static void SaveSession(Session session)
    {
        using (StreamWriter output = new StreamWriter(_fileName, true))
        {
            output.WriteLine(session.ToFileFormat());
        }
    }

    public static List<Session> LoadSessions()
    {
        List<Session> sessions = new List<Session>();

        if (File.Exists(_fileName))
        {
            string[] lines = File.ReadAllLines(_fileName);

            foreach (string line in lines)
            {
                sessions.Add(Session.FromFileFormat(line));
            }
        }

        return sessions;
    }
}

class Program
{
    static void DisplaySessions()
    {
        Console.Clear();
        List<Session> sessions = SessionManager.LoadSessions();

        if (sessions.Count == 0)
        {
            Console.WriteLine("No sessions found.");
        }
        else
        {
            Console.WriteLine("Past Sessions:\n");

            foreach (Session s in sessions)
            {
                Console.WriteLine(s);
            }
        }

        Console.WriteLine("\nPress Enter to return...");
        Console.ReadLine();
    }

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Menu Options:");
            Console.WriteLine("1. Start Breathing Activity");
            Console.WriteLine("2. Start Reflection Activity");
            Console.WriteLine("3. Start Listing Activity");
            Console.WriteLine("4. View Past Sessions");
            Console.WriteLine("5. Quit");

            Console.Write("Select a choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
                new BreathingActivity().Run();
            else if (choice == "2")
                new ReflectionActivity().Run();
            else if (choice == "3")
                new ListingActivity().Run();
            else if (choice == "4")
                DisplaySessions();
            else if (choice == "5")
                break;
        }
    }
}