using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

/// <summary>
/// "This program is a simple journal application that allows users to write daily entries based on prompts, save them to a SQLite database, and search through past entries. 
/// The main components of the program include:
/// - Entry: Represents a single journal entry with a date, prompt, and response.
/// - Journal: Manages a collection of entries, handles database interactions, and provides methods to add, display, save, load, and search entries.
/// - PromptGenerator: Provides a list of prompts and a method to retrieve a random prompt for the user to respond to.
/// The program runs in a loop, presenting a menu to the user for different actions such as writing a new entry, displaying the journal, saving to the database, loading from the database, searching entries, 
/// and quitting the application. Each action is handled through user input and corresponding methods in the Journal class. 
/// The database operations ensure that journal entries are persisted across sessions, allowing users to maintain a record of their thoughts and reflections over time.
/// Overall, this program serves as a practical example of how to structure a simple application using classes and how to interact with a database in C#.
/// Note: The code assumes that the Microsoft.Data.Sqlite package is installed and available in the project to handle SQLite database operations." (This part of the summary was mostly generated through VS Code's AI assistant, how I revised portions of it.)
/// 
/// For the extra creativity I decided to implement a search function that allows users to search through their journal entries for specific keywords. This in combination with the journal being saved to a local database (if you have SQLite database downloaded)
/// helps to create a very immersive journal experience. While I needed help to get the database working, the ability to search throught past entries was a easier feature I was able to implement.
/// </summary>



class Program
{
    public class Entry
    {
        public string Date;
        public string Prompt;
        public string Response;

        public void Display()
        {
            Console.WriteLine($"Date: {Date}");
            Console.WriteLine($"Prompt: {Prompt}");
            Console.WriteLine($"Response: {Response}");
            Console.WriteLine();
        }
    } 

public class Journal
{
    private string connectionString = "Data Source=journal.db";
    public List<Entry> Entries = new List<Entry>();


    public void AddEntry(Entry entry)
    {
        Entries.Add(entry);
    }

    public void DisplayAll()
    {
        foreach (Entry entry in Entries)
        {
            entry.Display();
        }
    }
    public void InitializeDatabase()
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"CREATE TABLE IF NOT EXISTS Entries (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Prompt TEXT,
            Response TEXT
        );";

        command.ExecuteNonQuery();
    }
}

    public void SaveToDatabase()
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        // Optional: clear old entries to avoid duplicates
        var clearCommand = connection.CreateCommand();
        clearCommand.CommandText = "DELETE FROM Entries;";
        clearCommand.ExecuteNonQuery();

        foreach (Entry entry in Entries)
        {
            var command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO Entries (Date, Prompt, Response)
              VALUES ($date, $prompt, $response);";

            command.Parameters.AddWithValue("$date", entry.Date);
            command.Parameters.AddWithValue("$prompt", entry.Prompt);
            command.Parameters.AddWithValue("$response", entry.Response);

            command.ExecuteNonQuery();
        }
    }

    Console.WriteLine("Journal saved to database!");


}

   public void LoadFromDatabase()
{
    try
    {
        Entries.Clear();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Date, Prompt, Response FROM Entries;";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Entry entry = new Entry();
                    entry.Date = reader.GetString(0);
                    entry.Prompt = reader.GetString(1);
                    entry.Response = reader.GetString(2);

                    Entries.Add(entry);
                }
            }
        }

        Console.WriteLine($"Loaded {Entries.Count} entries from database!");
    }
    catch (Exception)
    {
        Console.WriteLine("Error loading database.");
    }
}
    public void SearchEntries(string keyword)
{
    bool found = false;

    foreach (Entry entry in Entries)
    {
        if (entry.Prompt.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            entry.Response.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            entry.Display();
            found = true;
        }
    }

    if (!found)
    {
        Console.WriteLine("No matching entries found.");
    }
} 


}


public class PromptGenerator
{
    public List<string> Prompts = new List<string>()
    {
        "Who was the most interesting person I interacted with today?",
        "What was the best part of my day?",
        "How did I see the hand of the Lord in my life today?",
        "What was the strongest emotion I felt today?",
        "If I had one thing I could do over today, what would it be?"
    };

    Random random = new Random();

    public string GetRandomPrompt()
    {
        int index = random.Next(Prompts.Count);
        return Prompts[index];
    }
}

static void Main(string[] args)
    {
        Journal journal = new Journal();
        journal.InitializeDatabase();
        PromptGenerator promptGen = new PromptGenerator();
        

        while (true)
        {
            Console.WriteLine("Journal Menu");
            Console.WriteLine("1. Write a new entry");
            Console.WriteLine("2. Display journal");
            Console.WriteLine("3. Save journal");
            Console.WriteLine("4. Load journal");
            Console.WriteLine("5. Search entries");
            Console.WriteLine("6. Quit");

            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();
                        if (choice == "1")
            {
                string prompt = promptGen.GetRandomPrompt();
                Console.WriteLine(prompt);

                Console.Write("> ");
                string response = Console.ReadLine();

                Entry entry = new Entry();

                entry.Date = DateTime.Now.ToShortDateString();
                entry.Prompt = prompt;
                entry.Response = response;

                journal.AddEntry(entry);
            }
                        else if (choice == "2")
            {
                journal.DisplayAll();
            }

            else if (choice == "3")
            {
                journal.SaveToDatabase();

            }
            else if (choice == "4")
            {
                journal.LoadFromDatabase();
            }
            else if (choice == "5")
            {
             Console.Write("Enter keyword to search: ");
            string keyword = Console.ReadLine();
            journal.SearchEntries(keyword);
            }
            else if (choice == "6")
         {
             break;
            }
        }    
         
    }

}

