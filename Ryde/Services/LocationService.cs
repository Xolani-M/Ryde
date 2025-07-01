using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Ryde.Services
{

    // Handles location-based operations
    public class LocationService
    {
        // Simple location data for demo purposes
        private readonly Dictionary<string, Location> _locations = new Dictionary<string, Location>()
        {

            { "Downtown", new Location(-26.2041, 28.0473) },
            { "Sandton", new Location(-26.1076, 28.0567) },
            { "Rosebank", new Location(-26.1483, 28.0436) },
            { "Midrand", new Location(-25.9853, 28.1294) },
            { "Pretoria", new Location(-25.7479, 28.2293) },
            { "Airport", new Location(-26.1392, 28.2460) },
            { "Centurion", new Location(-25.8601, 28.1881) }
        };
    

    public double CalculateDistance(string fromLocation, string toLocation)
        {
            if (_locations.ContainsKey(fromLocation) && _locations.ContainsKey(toLocation))
            {
                return _locations[fromLocation].DistanceTo(_locations[toLocation]);
            }

            return new Random().NextDouble() * 20 + 5; // Return a random distance if locations not found
        }

        public bool IsWithinDistance(Location driverLocation, string pickUpLocation, double maxDistanceKm)
        {
            if (_locations.ContainsKey(pickUpLocation))
            { 
                double distance = driverLocation.DistanceTo(_locations[pickUpLocation]);
                return distance <= maxDistanceKm;
            }

            return true; // If location not found for demo purposes, assume within distance
        }

        public double GetDistanceToLocation(Location driverLocation, string targetLocation)
        {
            if (_locations.ContainsKey(targetLocation))
            {
                return driverLocation.DistanceTo(_locations[targetLocation]);
            }

            return new Random().NextDouble() * 10; // Return a random distance for demo
        }

        public List<string> GetAvailableLocations()
        {
            return new List<string>(_locations.Keys);
        }
    }
}
