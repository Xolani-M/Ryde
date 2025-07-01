using System;
using System.Collections.Generic;
using System.Linq;
using Ryde;
using Ryde.Interfaces;

namespace Ryde.Data
{
    // In-memory storage for rides
    public class RideRepository : IRideRepository
    {
        private readonly List<Ride> _rides;
        private int _nextId;

        public RideRepository()
        {
            _rides = new List<Ride>();
            _nextId = 1000; // Start ride IDs at 1000
        }

        public void AddRide(Ride ride)
        {
            try
            {
                if (ride.Id == 0)
                {
                    ride.Id = _nextId++;
                }

                _rides.Add(ride);
                Console.WriteLine($"✅ Ride #{ride.Id} added to system");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding ride: {ex.Message}");
                throw;
            }
        }

        public Ride GetRideById(int id)
        {
            try
            {
                // LINQ: Find ride by ID
                return _rides.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting ride by ID {id}: {ex.Message}");
                return null;
            }
        }

        public List<Ride> GetRidesByPassengerId(int passengerId)
        {
            try
            {
                // Get all rides for a specific passenger
                return _rides.Where(r => r.PassengerId == passengerId)
                           .OrderByDescending(r => r.RequestedAt)
                           .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting rides for passenger {passengerId}: {ex.Message}");
                return new List<Ride>();
            }
        }

        public List<Ride> GetRidesByDriverId(int driverId)
        {
            try
            {
                //Get all rides for a specific driver
                return _rides.Where(r => r.DriverId == driverId)
                           .OrderByDescending(r => r.RequestedAt)
                           .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting rides for driver {driverId}: {ex.Message}");
                return new List<Ride>();
            }
        }

        public List<Ride> GetAvailableRides()
        {
            try
            {
                //Get all rides that are available for drivers to accept
                return _rides.Where(r => r.Status == RideStatus.Requested)
                           .OrderBy(r => r.RequestedAt)
                           .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting available rides: {ex.Message}");
                return new List<Ride>();
            }
        }

        public void UpdateRide(Ride ride)
        {
            try
            {
                //Find existing ride
                var existingRide = _rides.FirstOrDefault(r => r.Id == ride.Id);
                if (existingRide == null)
                {
                    throw new InvalidOperationException($"Ride with ID {ride.Id} not found");
                }

                // Update the ride in the list
                var index = _rides.IndexOf(existingRide);
                _rides[index] = ride;

                Console.WriteLine($"✅ Ride #{ride.Id} updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating ride: {ex.Message}");
                throw;
            }
        }

  
        /// Get rides by status using LINQ
        public List<Ride> GetRidesByStatus(RideStatus status)
        {
            return _rides.Where(r => r.Status == status)
                        .OrderByDescending(r => r.RequestedAt)
                        .ToList();
        }

       
        /// Get rides within a date range
        public List<Ride> GetRidesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _rides.Where(r => r.RequestedAt >= startDate && r.RequestedAt <= endDate)
                        .OrderByDescending(r => r.RequestedAt)
                        .ToList();
        }

        /// Get today's rides using LINQ
        public List<Ride> GetTodaysRides()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _rides.Where(r => r.RequestedAt >= today && r.RequestedAt < tomorrow)
                        .OrderByDescending(r => r.RequestedAt)
                        .ToList();
        }

        /// Get high-value rides using
        public List<Ride> GetHighValueRides(decimal minimumFare = 50.0m)
        {
            return _rides.Where(r => r.Fare >= minimumFare)
                        .OrderByDescending(r => r.Fare)
                        .ToList();
        }

        
        /// Get ride statistics using LINQ aggregations
        public void DisplayRideStatistics()
        {
            try
            {
                if (!_rides.Any())
                {
                    Console.WriteLine("📊 No rides found in the system");
                    return;
                }

                var completedRides = _rides.Where(r => r.Status == RideStatus.Completed);
                var inProgressRides = _rides.Where(r => r.Status == RideStatus.InProgress);
                var requestedRides = _rides.Where(r => r.Status == RideStatus.Requested);
                var cancelledRides = _rides.Where(r => r.Status == RideStatus.Cancelled);

                Console.WriteLine("\n📊 === RIDE STATISTICS ===");
                Console.WriteLine($"Total Rides: {_rides.Count}");
                Console.WriteLine($"Completed: {completedRides.Count()}");
                Console.WriteLine($"In Progress: {inProgressRides.Count()}");
                Console.WriteLine($"Requested: {requestedRides.Count()}");
                Console.WriteLine($"Cancelled: {cancelledRides.Count()}");

                if (completedRides.Any())
                {
                    var totalRevenue = completedRides.Sum(r => r.Fare);
                    var averageFare = completedRides.Average(r => r.Fare);
                    var highestFare = completedRides.Max(r => r.Fare);
                    var longestDistance = completedRides.Max(r => r.DistanceKm);

                    Console.WriteLine($"\nRevenue Statistics:");
                    Console.WriteLine($"Total Revenue: ${totalRevenue:F2}");
                    Console.WriteLine($"Average Fare: ${averageFare:F2}");
                    Console.WriteLine($"Highest Fare: ${highestFare:F2}");
                    Console.WriteLine($"Longest Ride: {longestDistance:F1} km");

                    // Most popular routes
                    var popularRoutes = completedRides
                        .GroupBy(r => new { r.PickupLocation, r.DropOffLocation })
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .ToList();

                    if (popularRoutes.Any())
                    {
                        Console.WriteLine($"\nMost Popular Routes:");
                        foreach (var route in popularRoutes)
                        {
                            Console.WriteLine($"  {route.Key.PickupLocation} → {route.Key.DropOffLocation} ({route.Count()} rides)");
                        }
                    }
                }

                // Today's activity
                var todaysRides = GetTodaysRides();
                if (todaysRides.Any())
                {
                    Console.WriteLine($"\nToday's Activity:");
                    Console.WriteLine($"Rides Today: {todaysRides.Count}");
                    Console.WriteLine($"Revenue Today: ${todaysRides.Where(r => r.Status == RideStatus.Completed).Sum(r => r.Fare):F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error displaying ride statistics: {ex.Message}");
            }
        }

       
        /// Clean up old cancelled rides (housekeeping)
        public int CleanupOldCancelledRides(int daysOld = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysOld);
                var oldCancelledRides = _rides
                    .Where(r => r.Status == RideStatus.Cancelled && r.RequestedAt < cutoffDate)
                    .ToList();

                foreach (var ride in oldCancelledRides)
                {
                    _rides.Remove(ride);
                }

                if (oldCancelledRides.Any())
                {
                    Console.WriteLine($"🧹 Cleaned up {oldCancelledRides.Count} old cancelled rides");
                }

                return oldCancelledRides.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error cleaning up rides: {ex.Message}");
                return 0;
            }
        }

        public List<Ride> GetAllRides()
        {
            return _rides;
        }
    }
}