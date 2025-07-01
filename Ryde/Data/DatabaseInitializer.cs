using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Ryde;

namespace Ryde.Data
{
    /// Initializes our "database" with sample data for testing
    /// In a real app, this would set up actual database 
    public class DatabaseInitializer
    {
        private readonly UserRepository _userRepository;
        private readonly RideRepository _rideRepository;

        public DatabaseInitializer(UserRepository userRepository, RideRepository rideRepository)
        {
            _userRepository = userRepository;
            _rideRepository = rideRepository;
        }

        public void InitializeWithSampleData()
        {
            try
            {
                Console.WriteLine("🚀 Initializing Ryde system...");

                // Create some sample completed rides for demonstration
                CreateSampleCompletedRides();

                Console.WriteLine("✅ System initialized successfully!");
                Console.WriteLine($"   👥 Users: {_userRepository.GetAllUsers().Count}");
                Console.WriteLine($"   🚗 Available Drivers: {_userRepository.GetAvailableDrivers().Count}");
                Console.WriteLine("   📍 Supported Locations: Downtown, Sandton, Rosebank, Midrand, Pretoria, Airport, Centurion");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing system: {ex.Message}");
                throw;
            }
        }

        private void CreateSampleCompletedRides()
        {
            // Get some users for sample rides
            var drivers = _userRepository.GetAllDrivers();
            var passengers = _userRepository.GetAllPassengers();

            if (drivers.Count >= 2 && passengers.Count >= 2)
            {
                // Create a few completed rides for statistics
                var ride1 = new Ride
                {
                    Id = 1001,
                    PassengerId = passengers[0].Id,
                    DriverId = drivers[0].Id,
                    PickupLocation = "Downtown",
                    DropOffLocation = "Sandton",
                    DistanceKm = 15.2,
                    Status = RideStatus.Completed,
                    RequestedAt = DateTime.Now.AddHours(-2),
                    AcceptedAt = DateTime.Now.AddHours(-2).AddMinutes(3),
                    CompletedAt = DateTime.Now.AddHours(-1).AddMinutes(-30)
                };
                ride1.CalculateFare();

                var ride2 = new Ride
                {
                    Id = 1002,
                    PassengerId = passengers[1].Id,
                    DriverId = drivers[1].Id,
                    PickupLocation = "Rosebank",
                    DropOffLocation = "Airport",
                    DistanceKm = 22.8,
                    Status = RideStatus.Completed,
                    RequestedAt = DateTime.Now.AddHours(-4),
                    AcceptedAt = DateTime.Now.AddHours(-4).AddMinutes(2),
                    CompletedAt = DateTime.Now.AddHours(-3).AddMinutes(-15)
                };
                ride2.CalculateFare();

                // Add to repository
                _rideRepository.AddRide(ride1);
                _rideRepository.AddRide(ride2);

                // Update user ride histories
                passengers[0].RideHistory.Add(ride1);
                passengers[1].RideHistory.Add(ride2);
                drivers[0].CompletedRides.Add(ride1);
                drivers[1].CompletedRides.Add(ride2);
                drivers[0].TotalEarnings += ride1.Fare;
                drivers[1].TotalEarnings += ride2.Fare;

                Console.WriteLine($"📊 Created {2} sample completed rides");
            }
        }
    }
}
