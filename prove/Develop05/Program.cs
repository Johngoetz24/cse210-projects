using System;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// This program implements a goal tracking system with different types of goals (Simple, Eternal, Checklist, Bad Habit).
/// Users can create goals, record events, save/load progress, and visualize their growth with a flower representation. The program uses an abstract base class for goals and derived classes for each goal type,
/// and it manages the goals and score through a GoalManager class. The program also includes functionality to clear all data and reset progress.
/// The flower visualization changes based on the user's score, providing a fun and engaging way to track progress. The program saves and loads goal data to/from a file, allowing users to maintain their progress across sessions.
/// 
/// </summary>
/// 
/// EXCEEDS REQUIREMENTS:
/// - Visual flower growth based on score (GoalManager.DisplayFlower) 
/// - Bad Habit goal type that deducts points (BadHabitGoal)
/// - Confirmation prompt before clearing all data (GoalManager.ClearAll) 
/// - Display of completion timestamps for goals (Goal.GetCompletedAt) 





public abstract class Goal
{
    private string _name;
    private string _description;
    private int _points;
    private DateTime? _completedAt;

    public Goal(string name, string description, int points)
    {
        _name = name;
        _description = description;
        _points = points;
    }

    public string GetName() => _name;
    public string GetDescription() => _description;
    public int GetPoints() => _points;

    public abstract int RecordEvent();
    public abstract bool IsComplete();
    public abstract string GetStatus();
    public abstract string SaveFormat();

    public void SetCompletedAt()
    {
        if (_completedAt == null)
            _completedAt = DateTime.Now;
    }

    public void SetCompletedAt(DateTime? date)
    {
        _completedAt = date;
    }

    public string GetCompletedAt()
    {
        return _completedAt.HasValue ? _completedAt.Value.ToString("g") : "";
    }
}

public class SimpleGoal : Goal
{
    private bool _isComplete;

    public SimpleGoal(string name, string description, int points)
        : base(name, description, points) { }

    public void SetComplete(bool complete) => _isComplete = complete;

    public override int RecordEvent()
    {
        if (!_isComplete)
        {
            _isComplete = true;
            SetCompletedAt();
            return GetPoints();
        }
        return 0;
    }

    public override bool IsComplete() => _isComplete;

    public override string GetStatus()
    {
        return _isComplete
            ? $"[X] {GetName()} - {GetDescription()} (Completed: {GetCompletedAt()})"
            : $"[ ] {GetName()} - {GetDescription()}";
    }

    public override string SaveFormat()
    {
        return $"Simple|{GetName()}|{GetDescription()}|{GetPoints()}|{_isComplete}|{GetCompletedAt()}";
    }
}

public class EternalGoal : Goal
{
    public EternalGoal(string name, string description, int points)
        : base(name, description, points) { }

    public override int RecordEvent() => GetPoints();
    public override bool IsComplete() => false;

    public override string GetStatus()
    {
        return $"[∞] {GetName()} - {GetDescription()}";
    }

    public override string SaveFormat()
    {
        return $"Eternal|{GetName()}|{GetDescription()}|{GetPoints()}";
    }
}

public class ChecklistGoal : Goal
{
    private int _target;
    private int _current;
    private int _bonus;

    public ChecklistGoal(string name, string description, int points, int target, int bonus)
        : base(name, description, points)
    {
        _target = target;
        _bonus = bonus;
    }

    public void SetProgress(int current) => _current = current;

    public override int RecordEvent()
    {
        if (_current < _target)
        {
            _current++;
            if (_current == _target)
            {
                SetCompletedAt();
                return GetPoints() + _bonus;
            }
            return GetPoints();
        }
        return 0;
    }

    public override bool IsComplete() => _current >= _target;

    public override string GetStatus()
    {
        return IsComplete()
            ? $"[X] {GetName()} - {GetDescription()} ({_current}/{_target}) Completed: {GetCompletedAt()}"
            : $"[ ] {GetName()} - {GetDescription()} ({_current}/{_target})";
    }

    public override string SaveFormat()
    {
        return $"Checklist|{GetName()}|{GetDescription()}|{GetPoints()}|{_current}|{_target}|{_bonus}|{GetCompletedAt()}";
    }
}

public class BadHabitGoal : Goal
{
    private int _count = 0;

    public BadHabitGoal(string name, string description, int points)
        : base(name, description, points) { }

    public void SetCount(int count) => _count = count;

    public override int RecordEvent()
    {
        _count++;
        return -GetPoints();
    }

    public override bool IsComplete() => false;

    public override string GetStatus()
    {
        return $"[!] {GetName()} - {GetDescription()} (Slipped {_count} times)";
    }

    public override string SaveFormat()
    {
        return $"BadHabit|{GetName()}|{GetDescription()}|{GetPoints()}|{_count}";
    }
}

public class GoalManager
{
    private List<Goal> _goals = new List<Goal>();
    private int _score = 0;

    public int Count() => _goals.Count;

    public void AddGoal(Goal goal) => _goals.Add(goal);

    public void DisplayGoals()
    {
        for (int i = 0; i < _goals.Count; i++)
            Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
    }

    public void RecordEvent(int index)
    {
        int points = _goals[index].RecordEvent();
        _score += points;

        Console.WriteLine(points >= 0
            ? $"You earned {points} points!"
            : $"You lost {-points} points!");
    }

    public void DisplayScore()
    {
        Console.WriteLine($"\nScore: {_score}");
    }

    public void DisplayFlower()
    {
        int stage = Math.Min(_score / 200, 5);
        Console.WriteLine($"Growth: {_score}/1000");
        
        if (_score < 0) { 
            Console.WriteLine("\n  _ _"); 
            Console.WriteLine(" (x x) <-- Your flower is dying!"); 
            Console.WriteLine("  \\_/"); return; } 
        switch (stage) { 
            case 0: 
            Console.WriteLine("\n  ."); 
            Console.WriteLine("  |"); 
            Console.WriteLine(" / \\ (Seed)"); 
            break; 
            case 1: Console.WriteLine("\n  ."); 
            Console.WriteLine("  |"); 
            Console.WriteLine(" / \\"); 
            Console.WriteLine(" | | (Sprout)"); 
            break; 
            case 2: 
            Console.WriteLine("\n  ."); 
            Console.WriteLine(" \\|/"); 
            Console.WriteLine("  |"); 
            Console.WriteLine(" / \\ (Growing)"); 
            break; 
            case 3: 
            Console.WriteLine("\n \\|/"); 
            Console.WriteLine("  |"); 
            Console.WriteLine(" / \\");
            Console.WriteLine(" | | (Bud)"); 
            break; 
            case 4: 
            Console.WriteLine("\n  \\|/"); 
            Console.WriteLine("-- * --"); 
            Console.WriteLine("   |"); 
            Console.WriteLine("  / \\ (Almost there!)"); 
            break; 
            default: 
            Console.WriteLine("   🌸*🌸  "); 
            Console.WriteLine(" 🌸     🌸 "); 
            Console.WriteLine("🌸  *🌸*  🌸 "); 
            Console.WriteLine(" 🌸     🌸 "); 
            Console.WriteLine("   🌸*🌸  "); 
            Console.WriteLine("    || "); 
            Console.WriteLine("    || \n "); 
            Console.WriteLine(" (Full Flower!)");
            break; 
            } 
            
        
    }

    public void Save(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine(_score);
            foreach (Goal g in _goals)
                writer.WriteLine(g.SaveFormat());
        }
    }

    public void Load(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.");
            return;
        }

        _goals.Clear();
        string[] lines = File.ReadAllLines(filename);
        _score = int.Parse(lines[0]);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split('|');

            switch (parts[0])
            {
                case "Simple":
                    var sg = new SimpleGoal(parts[1], parts[2], int.Parse(parts[3]));
                    sg.SetComplete(bool.Parse(parts[4]));
                    if (parts.Length > 5 && parts[5] != "")
                        sg.SetCompletedAt(DateTime.Parse(parts[5]));
                    _goals.Add(sg);
                    break;

                case "Eternal":
                    _goals.Add(new EternalGoal(parts[1], parts[2], int.Parse(parts[3])));
                    break;

                case "Checklist":
                    var cg = new ChecklistGoal(parts[1], parts[2],
                        int.Parse(parts[3]), int.Parse(parts[5]), int.Parse(parts[6]));
                    cg.SetProgress(int.Parse(parts[4]));
                    if (parts.Length > 7 && parts[7] != "")
                        cg.SetCompletedAt(DateTime.Parse(parts[7]));
                    _goals.Add(cg);
                    break;

                case "BadHabit":
                    var bg = new BadHabitGoal(parts[1], parts[2], int.Parse(parts[3]));
                    if (parts.Length > 4)
                        bg.SetCount(int.Parse(parts[4]));
                    _goals.Add(bg);
                    break;
            }
        }
    }

    public void ClearAll()
    {
        Console.Write("Are you sure you want to delete EVERYTHING? (yes/no): ");
        if (Console.ReadLine().ToLower() == "yes")
        {
            _goals.Clear();
            _score = 0;

            if (File.Exists("Goals.txt"))
                File.Delete("Goals.txt");

            Console.WriteLine("All data cleared.");
        }
    }
}

class Program
{
    static void Main()
    {
        GoalManager manager = new GoalManager();
        bool running = true;

        while (running)
        {
            manager.DisplayScore();
            manager.DisplayFlower();

            Console.WriteLine("\n1. Create Goal\n2. List Goals\n3. Record Event\n4. Save\n5. Load\n6. Clear All\n7. Quit");
            Console.Write("Choose: ");

            switch (Console.ReadLine())
            {
                case "1": CreateGoal(manager); break;
                case "2": manager.DisplayGoals(); break;
                case "3":
                    manager.DisplayGoals();
                    Console.Write("Select #: ");
                    if (int.TryParse(Console.ReadLine(), out int i) && i > 0 && i <= manager.Count())
                        manager.RecordEvent(i - 1);
                    else
                        Console.WriteLine("Invalid choice.");
                    break;
                case "4":
                    manager.Save("Goals.txt");
                    Console.WriteLine("Saved.");
                    break;
                case "5":
                    manager.Load("Goals.txt");
                    Console.WriteLine("Loaded.");
                    break;
                case "6":
                    manager.ClearAll();
                    break;
                case "7":
                    running = false;
                    break;
            }
        }
    }

    static void CreateGoal(GoalManager manager)
    {
        Console.WriteLine("1. Simple\n2. Eternal\n3. Checklist\n4. Bad Habit");
        string type = Console.ReadLine();

        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Description: ");
        string desc = Console.ReadLine();
        Console.Write("Points: ");
        int pts = int.Parse(Console.ReadLine());

        switch (type)
        {
            case "1":
                manager.AddGoal(new SimpleGoal(name, desc, pts));
                break;
            case "2":
                manager.AddGoal(new EternalGoal(name, desc, pts));
                break;
            case "3":
                Console.Write("Target: ");
                int t = int.Parse(Console.ReadLine());
                Console.Write("Bonus: ");
                int b = int.Parse(Console.ReadLine());
                manager.AddGoal(new ChecklistGoal(name, desc, pts, t, b));
                break;
            case "4":
                manager.AddGoal(new BadHabitGoal(name, desc, pts));
                break;
        }
    }
}