using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryde
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
       

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Calculate distance between two locations (simplified)
        public double DistanceTo(Location other)
        {
            // Simplified distance calculation (in real apps, use proper geolocation formulas)
            double latDiff = Math.Abs(this.Latitude - other.Latitude);
            double lonDiff = Math.Abs(this.Longitude - other.Longitude);

            return Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff) * 111; // Rough km conversion
        }
    }
}
