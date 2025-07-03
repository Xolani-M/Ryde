using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Ryde
{
    public class Ride
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int? DriverId { get; set; } // Nullable - might not have driver yet
        public string PickupLocation { get; set; } = string.Empty;
        public string DropOffLocation { get; set; } = string.Empty;
        public decimal Fare { get; set; }
        public RideStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double DistanceKm { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }


        public Ride()
        {
            RequestedAt = DateTime.Now;
            Status = RideStatus.Requested;
        }

        public void CalculateFare()
        {
            // Simple fare calculation: base fare + distance-based fare
            decimal baseFare = 5.00m;
            decimal perKmRate = 2.50m;

            Fare = baseFare + ((decimal)DistanceKm * perKmRate);
        }

    }
}
