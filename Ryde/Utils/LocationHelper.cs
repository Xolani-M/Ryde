using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Utils
{
    /// <summary>
    /// Helper class for location-related operations
    /// Makes working with locations easy and efficient!
    /// </summary>
    public static class LocationHelper
    {
        // Popular locations in Johannesburg area
        public static readonly List<string> SupportedLocations = new List<string>
        {
            "Downtown",
            "Sandton",
            "Rosebank",
            "Midrand",
            "Pretoria",
            "Airport",
            "Centurion",
            "Fourways",
            "Randburg",
            "Bedfordview",
            "Germiston",
            "Kempton Park"
        };

        public static bool IsLocationSupported(string location)
        {
            return SupportedLocations.Any(l =>
                l.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> GetLocationSuggestions(string partialLocation)
        {
            if (string.IsNullOrWhiteSpace(partialLocation))
                return SupportedLocations.Take(5).ToList();

            return SupportedLocations
                .Where(l => l.StartsWith(partialLocation, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();
        }

        public static void DisplaySupportedLocations()
        {
            Console.WriteLine("\n📍 Supported Locations:");
            Console.WriteLine("========================");

            for (int i = 0; i < SupportedLocations.Count; i++)
            {
                if (i % 3 == 0 && i > 0)
                    Console.WriteLine();

                Console.Write($"{SupportedLocations[i],-15}");
            }
            Console.WriteLine("\n");
        }

        public static string GetLocationFromUser(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt}");
                DisplaySupportedLocations();

                string location = ConsoleHelper.GetStringInput("Enter location");

                if (IsLocationSupported(location))
                {
                    return SupportedLocations.First(l =>
                        l.Equals(location, StringComparison.OrdinalIgnoreCase));
                }

                var suggestions = GetLocationSuggestions(location);
                if (suggestions.Any())
                {
                    Console.WriteLine($"\nDid you mean one of these?");
                    for (int i = 0; i < suggestions.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {suggestions[i]}");
                    }

                    Console.Write("Select a number or enter a new location: ");
                    string choice = Console.ReadLine();

                    if (int.TryParse(choice, out int index) &&
                        index >= 1 && index <= suggestions.Count)
                    {
                        return suggestions[index - 1];
                    }
                }
                else
                {
                    ConsoleHelper.DisplayError($"Location '{location}' is not supported.");
                    Console.WriteLine("Please choose from the supported locations above.");
                }
            }
        }

        public static double CalculateEstimatedTravelTime(string fromLocation, string toLocation)
        {
            // Simple travel time estimation based on location
            // In a real app, you'd use mapping APIs like Google Maps

            var baseTime = 15.0; // Base time in minutes
            var random = new Random();

            // Add some variation based on locations
            if (fromLocation == "Airport" || toLocation == "Airport")
                baseTime += 10; // Airport takes longer

            if (fromLocation == "Pretoria" || toLocation == "Pretoria")
                baseTime += 15; // Pretoria is further

            // Add random traffic factor (5-20 minutes)
            baseTime += random.NextDouble() * 15 + 5;

            return Math.Round(baseTime, 1);
        }

        public static string GetEstimatedArrivalTime(string fromLocation, string toLocation)
        {
            var travelTime = CalculateEstimatedTravelTime(fromLocation, toLocation);
            var arrivalTime = DateTime.Now.AddMinutes(travelTime);

            return $"{arrivalTime:HH:mm} (in {travelTime:F0} minutes)";
        }
    }
}