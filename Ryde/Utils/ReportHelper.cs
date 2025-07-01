using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Utils
{
    /// <summary>
    /// Helper class for generating reports and analytics
    /// Makes data visualization beautiful and insightful!
    /// </summary>
    public static class ReportHelper
    {
        public static void GenerateSystemReport(List<User> users, List<Ride> rides)
        {
            Console.Clear();
            ConsoleHelper.DisplayHeader("RYDE SYSTEM REPORT");

            // User Statistics
            DisplayUserStatistics(users);

            // Ride Statistics  
            DisplayRideStatistics(rides);

            // Revenue Statistics
            DisplayRevenueStatistics(rides);

            // Performance Statistics
            DisplayPerformanceStatistics(users, rides);

            ConsoleHelper.PauseForUser();
        }

        private static void DisplayUserStatistics(List<User> users)
        {
            var drivers = users.OfType<Driver>().ToList();
            var passengers = users.OfType<Passenger>().ToList();
            var availableDrivers = drivers.Where(d => d.IsAvailable).Count();
            var activeUsers = users.Where(u => u.IsActive).Count();

            ConsoleHelper.DisplayInfo("USER STATISTICS");
            Console.WriteLine("==================");
            Console.WriteLine($"Total Users: {users.Count}");
            Console.WriteLine($"├─ Drivers: {drivers.Count} ({availableDrivers} available)");
            Console.WriteLine($"├─ Passengers: {passengers.Count}");
            Console.WriteLine($"└─ Active Users: {activeUsers}");

            if (drivers.Any())
            {
                var driversWithRatings = drivers.Where(d => d.ReceivedRatings.Any()).ToList();
                if (driversWithRatings.Any())
                {
                    var avgDriverRating = driversWithRatings.Average(d => d.GetAverageRating());
                    Console.WriteLine($"Average Driver Rating: {avgDriverRating:F1}/5.0");
                }
                else
                {
                    Console.WriteLine("Average Driver Rating: No ratings yet");
                }
            }

            Console.WriteLine();
        }

        private static void DisplayRideStatistics(List<Ride> rides)
        {
            var completedRides = rides.Where(r => r.Status == RideStatus.Completed).ToList();
            var activeRides = rides.Where(r => r.Status == RideStatus.InProgress).ToList();
            var requestedRides = rides.Where(r => r.Status == RideStatus.Requested).ToList();
            var cancelledRides = rides.Where(r => r.Status == RideStatus.Cancelled).ToList();

            ConsoleHelper.DisplayInfo("RIDE STATISTICS");
            Console.WriteLine("==================");
            Console.WriteLine($"Total Rides: {rides.Count}");
            Console.WriteLine($"├─ Completed: {completedRides.Count}");
            Console.WriteLine($"├─ In Progress: {activeRides.Count}");
            Console.WriteLine($"├─ Requested: {requestedRides.Count}");
            Console.WriteLine($"└─ Cancelled: {cancelledRides.Count}");

            if (completedRides.Any())
            {
                var avgDistance = completedRides.Average(r => r.DistanceKm);
                var totalDistance = completedRides.Sum(r => r.DistanceKm);
                var avgRideTime = CalculateAverageRideTime(completedRides);

                Console.WriteLine($"Average Distance: {avgDistance:F1} km");
                Console.WriteLine($"Total Distance: {totalDistance:F1} km");
                Console.WriteLine($"Average Ride Duration: {avgRideTime:F0} minutes");
            }

            Console.WriteLine();
        }

        private static void DisplayRevenueStatistics(List<Ride> rides)
        {
            var completedRides = rides.Where(r => r.Status == RideStatus.Completed).ToList();

            ConsoleHelper.DisplayInfo("REVENUE STATISTICS");
            Console.WriteLine("=====================");

            if (!completedRides.Any())
            {
                ConsoleHelper.DisplayWarning("No completed rides yet - no revenue generated.");
                Console.WriteLine();
                return;
            }

            var totalRevenue = completedRides.Sum(r => r.Fare);
            var avgFarePerRide = completedRides.Average(r => r.Fare);
            var highestFare = completedRides.Max(r => r.Fare);
            var lowestFare = completedRides.Min(r => r.Fare);

            Console.WriteLine($"Total Revenue: ${totalRevenue:F2}");
            Console.WriteLine($"Average Fare: ${avgFarePerRide:F2}");
            Console.WriteLine($"Highest Fare: ${highestFare:F2}");
            Console.WriteLine($"Lowest Fare: ${lowestFare:F2}");

            // Revenue by time period
            DisplayRevenueByPeriod(completedRides);

            Console.WriteLine();
        }

        private static void DisplayRevenueByPeriod(List<Ride> completedRides)
        {
            var today = DateTime.Today;
            var thisWeek = today.AddDays(-(int)today.DayOfWeek);
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var todayRevenue = completedRides
                .Where(r => r.CompletedAt?.Date == today)
                .Sum(r => r.Fare);

            var weekRevenue = completedRides
                .Where(r => r.CompletedAt >= thisWeek)
                .Sum(r => r.Fare);

            var monthRevenue = completedRides
                .Where(r => r.CompletedAt >= thisMonth)
                .Sum(r => r.Fare);

            Console.WriteLine($"├─ Today: ${todayRevenue:F2}");
            Console.WriteLine($"├─ This Week: ${weekRevenue:F2}");
            Console.WriteLine($"└─ This Month: ${monthRevenue:F2}");
        }

        private static void DisplayPerformanceStatistics(List<User> users, List<Ride> rides)
        {
            ConsoleHelper.DisplayInfo("PERFORMANCE STATISTICS");
            Console.WriteLine("=========================");

            // Driver Performance
            DisplayDriverPerformance(users.OfType<Driver>().ToList());

            // Passenger Activity
            DisplayPassengerActivity(users.OfType<Passenger>().ToList());

            // Popular Routes
            DisplayPopularRoutes(rides);

            // System Efficiency
            DisplaySystemEfficiency(rides);

            Console.WriteLine();
        }

        private static void DisplayDriverPerformance(List<Driver> drivers)
        {
            if (!drivers.Any())
            {
                Console.WriteLine("No drivers registered yet.");
                return;
            }

            Console.WriteLine("\n🚖 TOP PERFORMING DRIVERS:");
            Console.WriteLine("──────────────────────────");

            var topDrivers = drivers
                .Where(d => d.CompletedRides.Any())
                .OrderByDescending(d => d.TotalEarnings)
                .Take(5)
                .ToList();

            if (topDrivers.Any())
            {
                for (int i = 0; i < topDrivers.Count; i++)
                {
                    var driver = topDrivers[i];
                    var rating = driver.GetAverageRating();
                    Console.WriteLine($"{i + 1}. {driver.Username} - ${driver.TotalEarnings:F2} | {driver.CompletedRides.Count} rides | {rating:F1}⭐");
                }
            }
            else
            {
                Console.WriteLine("No completed rides by drivers yet.");
            }
        }

        private static void DisplayPassengerActivity(List<Passenger> passengers)
        {
            if (!passengers.Any())
            {
                Console.WriteLine("No passengers registered yet.");
                return;
            }

            Console.WriteLine("\n👥 MOST ACTIVE PASSENGERS:");
            Console.WriteLine("─────────────────────────");

            var activePassengers = passengers
                .Where(p => p.RideHistory.Any())
                .OrderByDescending(p => p.RideHistory.Count)
                .Take(5)
                .ToList();

            if (activePassengers.Any())
            {
                for (int i = 0; i < activePassengers.Count; i++)
                {
                    var passenger = activePassengers[i];
                    var totalSpent = passenger.RideHistory.Sum(r => r.Fare);
                    Console.WriteLine($"{i + 1}. {passenger.Username} - {passenger.RideHistory.Count} rides | ${totalSpent:F2} spent");
                }
            }
            else
            {
                Console.WriteLine("No ride history available for passengers.");
            }
        }

        private static void DisplayPopularRoutes(List<Ride> rides)
        {
            var completedRides = rides.Where(r => r.Status == RideStatus.Completed).ToList();

            if (!completedRides.Any())
                return;

            Console.WriteLine("\n🗺️  POPULAR ROUTES:");
            Console.WriteLine("──────────────────");

            var routeStats = completedRides
                .GroupBy(r => $"{r.PickupLocation} → {r.DropOffLocation}")
                .Select(g => new { Route = g.Key, Count = g.Count(), AvgFare = g.Average(r => r.Fare) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            for (int i = 0; i < routeStats.Count; i++)
            {
                var route = routeStats[i];
                Console.WriteLine($"{i + 1}. {route.Route} ({route.Count} rides, avg ${route.AvgFare:F2})");
            }
        }

        private static void DisplaySystemEfficiency(List<Ride> rides)
        {
            if (!rides.Any())
                return;

            Console.WriteLine("\n⚡ SYSTEM EFFICIENCY:");
            Console.WriteLine("────────────────────");

            var totalRides = rides.Count;
            var completedRides = rides.Count(r => r.Status == RideStatus.Completed);
            var cancelledRides = rides.Count(r => r.Status == RideStatus.Cancelled);

            var completionRate = totalRides > 0 ? (completedRides * 100.0 / totalRides) : 0;
            var cancellationRate = totalRides > 0 ? (cancelledRides * 100.0 / totalRides) : 0;

            Console.WriteLine($"Completion Rate: {completionRate:F1}% ({completedRides}/{totalRides})");
            Console.WriteLine($"Cancellation Rate: {cancellationRate:F1}% ({cancelledRides}/{totalRides})");

            // Average response time (time from request to acceptance)
            var acceptedRides = rides.Where(r => r.AcceptedAt.HasValue).ToList();
            if (acceptedRides.Any())
            {
                var avgResponseTime = acceptedRides
                    .Average(r => (r.AcceptedAt.Value - r.RequestedAt).TotalMinutes);
                Console.WriteLine($"Average Response Time: {avgResponseTime:F1} minutes");
            }
        }

        private static double CalculateAverageRideTime(List<Ride> completedRides)
        {
            var ridesWithTimes = completedRides
                .Where(r => r.AcceptedAt.HasValue && r.CompletedAt.HasValue)
                .ToList();

            if (!ridesWithTimes.Any())
                return 0;

            return ridesWithTimes
                .Average(r => (r.CompletedAt.Value - r.AcceptedAt.Value).TotalMinutes);
        }

        public static void GenerateDriverReport(Driver driver)
        {
            Console.Clear();
            ConsoleHelper.DisplayHeader($"DRIVER REPORT - {driver.Username.ToUpper()}");

            Console.WriteLine("📋 DRIVER OVERVIEW");
            Console.WriteLine("==================");
            Console.WriteLine($"Name: {driver.Username}");
            Console.WriteLine($"License: {driver.LicenseNumber}");
            Console.WriteLine($"Vehicle: {driver.VehicleInfo}");
            Console.WriteLine($"Status: {(driver.IsAvailable ? "🟢 Available" : "🔴 Busy")}");
            Console.WriteLine($"Member Since: {driver.CreatedAt:yyyy-MM-dd}");

            var rating = driver.GetAverageRating();
            Console.WriteLine($"Rating: {(rating > 0 ? $"{rating:F1}/5.0 ⭐" : "No ratings yet")}");

            Console.WriteLine("\n💰 EARNINGS SUMMARY");
            Console.WriteLine("===================");
            Console.WriteLine($"Total Earnings: ${driver.TotalEarnings:F2}");
            Console.WriteLine($"Completed Rides: {driver.CompletedRides.Count}");

            if (driver.CompletedRides.Any())
            {
                var avgEarningsPerRide = driver.TotalEarnings / driver.CompletedRides.Count;
                Console.WriteLine($"Average per Ride: ${avgEarningsPerRide:F2}");
            }

            if (driver.CompletedRides.Any())
            {
                Console.WriteLine("\n🚗 RECENT RIDES");
                Console.WriteLine("===============");

                var recentRides = driver.CompletedRides
                    .OrderByDescending(r => r.CompletedAt)
                    .Take(5)
                    .ToList();

                foreach (var ride in recentRides)
                {
                    Console.WriteLine($"#{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} | ${ride.Fare:F2} | {ride.CompletedAt:MM/dd HH:mm}");
                }
            }

            ConsoleHelper.PauseForUser();
        }

        public static void GeneratePassengerReport(Passenger passenger)
        {
            Console.Clear();
            ConsoleHelper.DisplayHeader($"PASSENGER REPORT - {passenger.Username.ToUpper()}");

            Console.WriteLine("📋 PASSENGER OVERVIEW");
            Console.WriteLine("=====================");
            Console.WriteLine($"Name: {passenger.Username}");
            Console.WriteLine($"Email: {passenger.Email}");
            Console.WriteLine($"Phone: {passenger.PhoneNumber}");
            Console.WriteLine($"Member Since: {passenger.CreatedAt:yyyy-MM-dd}");
            Console.WriteLine($"Payment Method: {passenger.PreferredPaymentMethod}");

            Console.WriteLine("\n💳 WALLET & SPENDING");
            Console.WriteLine("====================");
            Console.WriteLine($"Current Balance: ${passenger.WalletBalance:F2}");
            Console.WriteLine($"Total Rides: {passenger.RideHistory.Count}");

            if (passenger.RideHistory.Any())
            {
                var totalSpent = passenger.RideHistory.Sum(r => r.Fare);
                var avgSpentPerRide = totalSpent / passenger.RideHistory.Count;
                Console.WriteLine($"Total Spent: ${totalSpent:F2}");
                Console.WriteLine($"Average per Ride: ${avgSpentPerRide:F2}");
            }

            if (passenger.RideHistory.Any())
            {
                Console.WriteLine("\n🚗 RECENT RIDES");
                Console.WriteLine("===============");

                var recentRides = passenger.RideHistory
                    .OrderByDescending(r => r.RequestedAt)
                    .Take(5)
                    .ToList();

                foreach (var ride in recentRides)
                {
                    var status = ride.Status.ToString();
                    Console.WriteLine($"#{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} | ${ride.Fare:F2} | {status}");
                }
            }

            ConsoleHelper.PauseForUser();
        }
    }
}
