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