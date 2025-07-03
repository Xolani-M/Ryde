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
            "Kempton Park",
            "Soweto",
            "Boksburg",
            "Edenvale",
            "Bryanston",
            "Melrose Arch",
            "Maboneng",
            "Parkhurst"
        };

        // Popular locations in Johannesburg area with coordinates
        public static readonly Dictionary<string, (double Latitude, double Longitude)> LocationCoordinates =
            new Dictionary<string, (double, double)>
            {
                { "Downtown", (-26.2041, 28.0473) },
                { "Sandton", (-26.1076, 28.0567) },
                { "Rosebank", (-26.1483, 28.0436) },
                { "Midrand", (-25.9992, 28.1260) },
                { "Pretoria", (-25.7479, 28.2293) },
                { "Airport", (-26.1337, 28.2420) },
                { "Centurion", (-25.8600, 28.1890) },
                { "Fourways", (-26.0246, 28.0134) },
                { "Randburg", (-26.0941, 28.0286) },
                { "Bedfordview", (-26.1844, 28.1402) },
                { "Germiston", (-26.2170, 28.1706) },
                { "Kempton Park", (-26.1000, 28.2293) },
                { "Soweto", (-26.2485, 27.8540) },
                { "Boksburg", (-26.2125, 28.2547) },
                { "Edenvale", (-26.1404, 28.1536) },
                { "Bryanston", (-26.0559, 28.0306) },
                { "Melrose Arch", (-26.1342, 28.0682) },
                { "Maboneng", (-26.2032, 28.0636) },
                { "Parkhurst", (-26.1422, 28.0187) }
            };

        /// <summary>
        /// Checks if a location is supported (case-insensitive).
        /// </summary>
        /// <param name="location">The location name to check.</param>
        /// <returns>True if the location is supported; otherwise, false.</returns>
        public static bool IsLocationSupported(string location)
        {
            return SupportedLocations.Any(l =>
                l.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets up to 5 location suggestions that start with the given partial input.
        /// </summary>
        /// <param name="partialLocation">The partial location name entered by the user.</param>
        /// <returns>A list of up to 5 suggested location names.</returns>
        public static List<string> GetLocationSuggestions(string partialLocation)
        {
            if (string.IsNullOrWhiteSpace(partialLocation))
                return SupportedLocations.Take(5).ToList();

            return SupportedLocations
                .Where(l => l.StartsWith(partialLocation, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Displays all supported locations in a formatted list.
        /// </summary>
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

        /// <summary>
        /// Prompts the user to enter a location, providing suggestions and validation.
        /// </summary>
        /// <param name="prompt">The prompt message to display to the user.</param>
        /// <returns>The validated location name selected by the user.</returns>
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
                    string? choice = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(choice) && int.TryParse(choice, out int index) &&
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

        /// <summary>
        /// Estimates travel time in minutes between two locations, with random variation.
        /// </summary>
        /// <param name="fromLocation">The starting location.</param>
        /// <param name="toLocation">The destination location.</param>
        /// <returns>Estimated travel time in minutes.</returns>
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

        /// <summary>
        /// Gets the estimated arrival time as a formatted string.
        /// </summary>
        /// <param name="fromLocation">The starting location.</param>
        /// <param name="toLocation">The destination location.</param>
        /// <returns>Estimated arrival time as a string.</returns>
        public static string GetEstimatedArrivalTime(string fromLocation, string toLocation)
        {
            var travelTime = CalculateEstimatedTravelTime(fromLocation, toLocation);
            var arrivalTime = DateTime.Now.AddMinutes(travelTime);

            return $"{arrivalTime:HH:mm} (in {travelTime:F0} minutes)";
        }

        /// <summary>
        /// Calculates the distance (in km) between two supported locations.
        /// For demo purposes, returns a random value or a fixed mapping.
        /// </summary>
        /// <param name="fromLocation">The starting location.</param>
        /// <param name="toLocation">The destination location.</param>
        /// <returns>The distance in kilometers.</returns>
        public static double CalculateDistance(string fromLocation, string toLocation)
        {
            // Simple demo: if locations are the same, distance is 1km
            if (string.Equals(fromLocation, toLocation, StringComparison.OrdinalIgnoreCase))
                return 1.0;

            // Otherwise, assign a pseudo-random but deterministic distance based on string hash
            int hash = Math.Abs((fromLocation + toLocation).GetHashCode());
            // Distance between 5km and 30km
            return 5.0 + (hash % 26);
        }

        /// <summary>
        /// Gets the latitude and longitude for a supported location name.
        /// </summary>
        /// <param name="locationName">The name of the location.</param>
        /// <returns>The coordinates as (latitude, longitude), or null if not found.</returns>
        public static (double Latitude, double Longitude)? GetCoordinates(string locationName)
        {
            if (LocationCoordinates.TryGetValue(locationName, out var coords))
                return coords;
            return null;
        }

        /// <summary>
        /// Calculates the distance in kilometers between two coordinates using the Haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>The distance in kilometers.</returns>
        public static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in km
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        /// <summary>
        /// Returns the list of supported locations.
        /// </summary>
        public static List<string> GetSupportedLocations()
        {
            return SupportedLocations.ToList();
        }

        /// <summary>
        /// Gets the coordinates for a supported location name.
        /// </summary>
        public static (double Latitude, double Longitude) GetCoordinatesForLocation(string locationName)
        {
            if (LocationCoordinates.TryGetValue(locationName, out var coords))
                return coords;
            throw new ArgumentException($"Location '{locationName}' is not supported.");
        }
    }
}