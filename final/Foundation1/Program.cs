

/* 

Write a program to keep track of YouTube videos and comments left on them. As mentioned this could be part of a larger project to analyze them, 
but for this assignment, you will only need to worry about storing the information about a video and the comments.
Your program should have a class for a Video that has the responsibility to track the title, author, and length (in seconds) of the video. 
Each video also has responsibility to store a list of comments, and should have a method to return the number of comments. A comment should be 
defined by the Comment class which has the responsibility for tracking both the name of the person who made the comment and the text of the comment.
Once you have the classes in place, write a program that creates 3-4 videos, sets the appropriate values, and for each one add a list of 3-4 comments 
(with the commenter's name and text). Put each of these videos in a list. Then, have your program iterate through the list of videos and for each one, 
display the title, author, length, number of comments (from the method) and then list out all of the comments for that video. Repeat this display for each video in the list.
Note: The YouTube example is just to give you a context for creating classes to store information. You will not actually be connecting to YouTube or downloading content in any way.

*/






/// <summary>
/// MAIN ---> Video class --> Comment class
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;

// ===================== COMMENT CLASS =====================
public class Comment
{
    private string _name;
    private string _text;

    public Comment(string name, string text)
    {
        _name = name;
        _text = text;
    }

    public string GetName() => _name;
    public string GetText() => _text;
}

// ===================== VIDEO CLASS =====================
public class Video
{
    private string _title;
    private string _author;
    private int _length;
    private List<Comment> _comments = new List<Comment>();

    public Video(string title, string author, int length)
    {
        _title = title;
        _author = author;
        _length = length;
    }

    public void AddComment(Comment comment) => _comments.Add(comment);
    public int GetCommentCount() => _comments.Count;

    public void DisplayVideoInfo()
    {
        Console.WriteLine($"\nTitle: {_title}");
        Console.WriteLine($"Author: {_author}");
        Console.WriteLine($"Length: {_length} seconds");
        Console.WriteLine($"Number of Comments: {GetCommentCount()}");
        Console.WriteLine("Comments:");

        foreach (var comment in _comments)
        {
            Console.WriteLine($"- {comment.GetName()}: {comment.GetText()}");
        }

        Console.WriteLine("-----------------------------------");
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        Console.WriteLine("Where would you like to get video data?");
        Console.WriteLine("1. Load from file");
        Console.WriteLine("2. Enter manually");
        Console.WriteLine("3. Use sample data");
        Console.Write("Choose an option: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                if (!LoadFromFile("Videos.txt", videos))
                {
                    Console.WriteLine("Falling back to sample data...");
                    CreateSampleData(videos);
                }
                break;

            case "2":
                CreateFromUserInput(videos);
                break;

            case "3":
                CreateSampleData(videos);
                break;

            default:
                Console.WriteLine("Invalid choice. Using sample data.");
                CreateSampleData(videos);
                break;
        }

        DisplayAllVideos(videos);
    }

    static bool LoadFromFile(string filePath, List<Video> videos)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return false;
            }

            Video currentVideo = null;

            foreach (string line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentVideo != null)
                    {
                        videos.Add(currentVideo);
                        currentVideo = null;
                    }
                    continue;
                }

                if (line.StartsWith("Video:"))
                {
                    string[] parts = line.Substring(6).Split('|');

                    if (parts.Length < 3)
                        throw new Exception("Invalid video format.");

                    currentVideo = new Video(
                        parts[0].Trim(),
                        parts[1].Trim(),
                        int.Parse(parts[2].Trim())
                    );
                }
                else if (line.StartsWith("Comment:") && currentVideo != null)
                {
                    string[] parts = line.Substring(8).Split('|');

                    if (parts.Length < 2)
                        throw new Exception("Invalid comment format.");

                    currentVideo.AddComment(
                        new Comment(parts[0].Trim(), parts[1].Trim())
                    );
                }
            }

            if (currentVideo != null)
                videos.Add(currentVideo);

            Console.WriteLine("File loaded successfully!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading file: {e.Message}");
            return false;
        }
    }

    static void CreateFromUserInput(List<Video> videos)
    {
        int videoCount = GetValidInt("How many videos? ");

        for (int i = 0; i < videoCount; i++)
        {
            Console.WriteLine($"\nVideo {i + 1}");

            Console.Write("Title: ");
            string title = Console.ReadLine();

            Console.Write("Author: ");
            string author = Console.ReadLine();

            int length = GetValidInt("Length (seconds): ");

            Video video = new Video(title, author, length);

            int commentCount = GetValidInt("How many comments? ");

            for (int j = 0; j < commentCount; j++)
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();

                Console.Write("Comment: ");
                string text = Console.ReadLine();

                video.AddComment(new Comment(name, text));
            }

            videos.Add(video);
        }
    }

    static void CreateSampleData(List<Video> videos)
    {
        Video v1 = new Video("C# Basics", "CodeMaster", 600);
        v1.AddComment(new Comment("Alice", "Super helpful!"));
        v1.AddComment(new Comment("Bob", "Thanks!"));
        videos.Add(v1);

        Video v2 = new Video("Gaming Highlights", "GameZone", 900);
        v2.AddComment(new Comment("Chris", "That was crazy!"));
        v2.AddComment(new Comment("Dana", "Loved it"));
        videos.Add(v2);

        Video v3 = new Video("Cooking Pasta", "ChefLife", 500);
        v3.AddComment(new Comment("Eli", "Trying this tonight"));
        v3.AddComment(new Comment("Faith", "Looks amazing"));
        videos.Add(v3);
    }
    static void DisplayAllVideos(List<Video> videos)
    {
        Console.WriteLine("\n===== Video List =====");

        foreach (var video in videos)
        {
            video.DisplayVideoInfo();
        }
    }
    static int GetValidInt(string prompt)
    {
        int value;
        Console.Write(prompt);

        while (!int.TryParse(Console.ReadLine(), out value))
        {
            Console.Write("Invalid input. Try again: ");
        }

        return value;
    }
}







