using System;
using System.ComponentModel;

class Program
{
    static void Main(string[] args)
    {
       int Magic_Number = Collect_Random_Number();
       Game_Play(Magic_Number);
    }
    static int Collect_Random_Number()
    {
        Console.Write("Please enter in a Magic Number:");
        string userInput = Console.ReadLine();
        int number = int.Parse(userInput);
        return number;
    }
    static void Game_Play(int Magic_Number)
    {
            int Number_To_Guess = Magic_Number;
            int Guessed_Number = 0;
            while (Guessed_Number != Number_To_Guess){
            Console.Write("What is your guess? ");
            string userInput = Console.ReadLine();
            Guessed_Number = int.Parse(userInput);
            if (Guessed_Number < Magic_Number)
            {
                Console.Write("Higher");
                Console.WriteLine();
            }
            if (Guessed_Number > Magic_Number)
            {
                Console.Write("Lower");
                Console.WriteLine();
            }
        }
        Console.Write("You Guessed it!");
    }

    

}