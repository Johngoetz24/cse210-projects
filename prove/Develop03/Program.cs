using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;




/// "This program is a scripture memorizer that allows users to practice memorizing scriptures by hiding random words from a displayed scripture.
/// The main components of the program include:
/// - Word: Represents a single word in the scripture, with functionality to hide itself and return its display text.
/// - Reference: Represents the reference of a scripture, including the book, chapter, and verse(s).
/// - Scripture: Represents a scripture, containing a reference and a list of words
/// - ScriptureLibrary: Manages a collection of scriptures, allowing for loading from a file and retrieving a random scripture.
/// The program runs in a loop, allowing the user to choose a difficulty level (easy or hard) which determines how many words are hidden each time. 
/// (This part of the summary was mostly generated through VS Code's AI assistant, how I revised portions of it.)
/// 
/// 
/// To add to the program, I added the ability to load scriptures from a file, which allows for much more flexibility and variety in the scriptures that can be practiced. 
/// There is a fail safe incase the file is not able to be loaded, and I created the option to choose between an easy and hard difficulty, 
/// which hides a different number of words each time.


namespace ScriptureMemorizer
{
    public class Word
    {
        private string _text;
        private bool _isHidden;

        public Word(string text)
        {
            _text = text;
            _isHidden = false;
        }

        public string GetDisplayText() => _isHidden ? "____" : _text;

        public void Hide() => _isHidden = true;

        public bool IsHidden() => _isHidden;
    }

    public class Reference
    {
        public string Book { get; private set; }
        public int Chapter { get; private set; }
        public int StartVerse { get; private set; }
        public int EndVerse { get; private set; }

        public Reference(string book, int chapter, int verse)
        {
            Book = book;
            Chapter = chapter;
            StartVerse = verse;
            EndVerse = verse;
        }

        public Reference(string book, int chapter, int startVerse, int endVerse)
        {
            Book = book;
            Chapter = chapter;
            StartVerse = startVerse;
            EndVerse = endVerse;
        }

        public override string ToString() =>
            StartVerse == EndVerse ? $"{Book} {Chapter}:{StartVerse}" : $"{Book} {Chapter}:{StartVerse}-{EndVerse}";
    }

    public class Scripture
    {
        private Reference _reference;
        private List<Word> _words;
        private Random _random;

        public Scripture(Reference reference, string text)
        {
            _reference = reference;
            _words = text.Split(' ').Select(w => new Word(w)).ToList();
            _random = new Random();
        }

        public void Display()
        {
            Console.Clear();
            Console.WriteLine(_reference);
            Console.WriteLine(string.Join(" ", _words.Select(w => w.GetDisplayText())));
        }

        public void HideRandomWords(int count)
        {
            var visibleWords = _words.Where(w => !w.IsHidden()).ToList();
            if (visibleWords.Count == 0) return;

            for (int i = 0; i < count && visibleWords.Count > 0; i++)
            {
                int index = _random.Next(visibleWords.Count);
                visibleWords[index].Hide();
                visibleWords.RemoveAt(index);
            }
        }

        public bool AllWordsHidden() => _words.All(w => w.IsHidden());
    }

    public class ScriptureLibrary
    {
        private List<Scripture> _scriptures;
        private Random _random = new Random();

        public ScriptureLibrary()
        {
            _scriptures = new List<Scripture>()
            {
                new Scripture(new Reference("John", 3, 16),
                    "For God so loved the world that he gave his one and only Son."),

                new Scripture(new Reference("Proverbs", 3, 5, 6),
                    "Trust in the Lord with all thine heart and lean not unto thine own understanding."),

                new Scripture(new Reference("Psalm", 23, 1),
                    "The Lord is my shepherd I shall not want.")
            };
        }

        public ScriptureLibrary(string filePath) : this()  
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File '{filePath}' not found.");

                string[] lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 4) continue; 

                    string book = parts[0].Trim();
                    int chapter = int.Parse(parts[1].Trim());
                    string versePart = parts[2].Trim();
                    string text = parts[3].Trim();

                    Reference reference;
                    if (versePart.Contains("-"))
                    {
                        string[] verses = versePart.Split('-');
                        reference = new Reference(book, chapter,
                            int.Parse(verses[0]),
                            int.Parse(verses[1]));
                    }
                    else
                    {
                        reference = new Reference(book, chapter,
                            int.Parse(versePart));
                    }

                    _scriptures.Add(new Scripture(reference, text));
                }

                Console.WriteLine($"Loaded scriptures from '{filePath}' successfully.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load file: {ex.Message}");
                Console.WriteLine("Using fallback scriptures.\n");
            }
        }

        public Scripture GetRandomScripture()
        {
            return _scriptures[_random.Next(_scriptures.Count)];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            ScriptureLibrary library = new ScriptureLibrary("scripture.txt");
            Scripture scripture = library.GetRandomScripture();

            Console.WriteLine("Choose difficulty: easy or hard");
            string difficulty = (Console.ReadLine() ?? "").ToLower();
            int wordsToHide = (difficulty == "hard") ? 6 : 3;

            scripture.Display();

            while (!scripture.AllWordsHidden())
            {
                Console.WriteLine("\nPress Enter to continue or type 'quit' to exit:");
                string input = Console.ReadLine();
                if (input != null && input.ToLower() == "quit") break;

                scripture.HideRandomWords(wordsToHide);
                scripture.Display();
            }

            Console.WriteLine("\nAll words hidden. Program ending.");
        }
    }
}


