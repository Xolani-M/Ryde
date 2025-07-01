using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// Helper class for console operations and user input
    /// Makes our console interface beautiful and user-friendly!
    /// </summary>
    public static class ConsoleHelper
    {
        public static void DisplayWelcomeMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║                    🚗 WELCOME TO RYDE 🚗                    ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║                Your Premium Ride-Sharing Service            ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void DisplayHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n{'='} {title.ToUpper()} {'='}");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void DisplayMenu(string title, List<string> options)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n📋 {title}");
            Console.WriteLine(new string('-', title.Length + 4));
            Console.ResetColor();

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {options[i]}");
            }
            Console.WriteLine();
        }

        public static int GetMenuChoice(int maxOptions)
        {
            while (true)
            {
                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 1 && choice <= maxOptions)
                {
                    return choice;
                }

                DisplayError("Invalid choice. Please try again.");
            }
        }

        public static string GetStringInput(string prompt, bool required = true)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string input = Console.ReadLine()?.Trim();

                if (!required || !string.IsNullOrEmpty(input))
                {
                    return input ?? "";
                }

                DisplayError("This field is required. Please enter a value.");
            }
        }

        public static decimal GetDecimalInput(string prompt, decimal min = 0, decimal max = decimal.MaxValue)
        {
            while (true)
            {
                Console.Write($"{prompt}: R");
                if (decimal.TryParse(Console.ReadLine(), out decimal value) &&
                    value >= min && value <= max)
                {
                    return value;
                }

                DisplayError($"Please enter a valid amount between R{min:F2} and R{max:F2}");
            }
        }

        public static int GetIntInput(string prompt, int min = 1, int max = 5)
        {
            while (true)
            {
                Console.Write($"{prompt} ({min}-{max}): ");
                if (int.TryParse(Console.ReadLine(), out int value) &&
                    value >= min && value <= max)
                {
                    return value;
                }

                DisplayError($"Please enter a number between {min} and {max}");
            }
        }

        public static void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }

        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {message}");
            Console.ResetColor();
        }

        public static void DisplayWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  {message}");
            Console.ResetColor();
        }

        public static void DisplayInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ️  {message}");
            Console.ResetColor();
        }

        public static bool GetYesNoInput(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (y/n): ");
                string input = Console.ReadLine()?.Trim().ToLower();

                if (input == "y" || input == "yes")
                    return true;
                if (input == "n" || input == "no")
                    return false;

                DisplayError("Please enter 'y' for yes or 'n' for no.");
            }
        }

        public static void PauseForUser(string message = "Press any key to continue...")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        public static void DisplayTable<T>(List<T> items, string title, Func<T, string[]> getRowData, string[] headers)
        {
            if (!items.Any())
            {
                DisplayInfo($"No {title.ToLower()} found.");
                return;
            }

            Console.WriteLine($"\n📊 {title}");
            Console.WriteLine(new string('=', title.Length + 4));

            // Calculate column widths
            var columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = Math.Max(headers[i].Length,
                    items.Max(item => getRowData(item)[i]?.Length ?? 0));
            }

            // Print headers
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write($"{headers[i].PadRight(columnWidths[i] + 2)}");
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', columnWidths.Sum() + headers.Length * 2));
            Console.ResetColor();

            // Print rows
            foreach (var item in items)
            {
                var rowData = getRowData(item);
                for (int i = 0; i < rowData.Length; i++)
                {
                    Console.Write($"{rowData[i]?.PadRight(columnWidths[i] + 2) ?? "".PadRight(columnWidths[i] + 2)}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
