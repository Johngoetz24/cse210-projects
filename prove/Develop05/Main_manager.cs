

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
        if (!int.TryParse(lines[0], out _score))
{
    Console.WriteLine("Invalid score. Setting to 0.");
    _score = 0;
}

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