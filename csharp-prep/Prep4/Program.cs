using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {

        Console.Write("Welcome to the number analyzer!\nEnter a series of numbers, and I'll calculate the sum, average, and max for you.\n");

        List<int> numbers = new List<int>();
        
        int userNum = -1;
        while (userNum != 0)
        {
            Console.Write("Enter a number (0 to quit): ");
            
            string userResponse = Console.ReadLine();
            userNum = int.Parse(userResponse);
 

            if (userNum != 0)
            {
                numbers.Add(userNum);
            }
        }

        int sum = 0;
        foreach (int number in numbers)
        {
            sum += number;
        }

        Console.WriteLine($"The sum is: {sum}");

       
        float average = ((float)sum) / numbers.Count;
        Console.WriteLine($"The average is: {average}");
        
        int max = numbers[0];

        foreach (int number in numbers)
        {
            if (number > max)
            {
                max = number;
            }
        }

        Console.WriteLine($"The max is: {max}");
    }
}