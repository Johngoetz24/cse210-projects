using System;
using System.Runtime.Versioning;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("What is the grade received: ");
        string userInput = Console.ReadLine();
        int number = int.Parse(userInput);
        string letter = "";
        if (number >= 90)
        {
            letter = "A";
        }
        else if (number >= 80)
        {
            letter = "B";
        }
        else if (number >= 70)
        {
            letter = "C";
        }
        else if (number >= 60)
        {
            letter = "D";
        }
        else
        {
            letter = "F";
        }
        Console.WriteLine($"The letter grade is: {letter}");
        if (number >= 70)
        {
            Console.WriteLine("Congratulations!");
        }
        else
        {
            Console.WriteLine("Better luck next time.");
        }
    }
}