using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;
using Ryde.Interfaces;

namespace Ryde.Services
{
    /// Handles all ride-related business logic
    public class RideService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly PaymentService _paymentService;
        private readonly LocationService _locationService;

        public RideService(IUserRepository userRepository, IRideRepository rideRepository,
            PaymentService paymentService, LocationService locationService)
        {
            _userRepository = userRepository;
            _rideRepository = rideRepository;
            _paymentService = paymentService;
            _locationService = locationService;
        }

        public Ride RequestRide(int passengerId, string pickupLocation, string dropOffLocation)
        {
            try
            {   // Get the passenger from the repository
                var passenger = _userRepository.GetUserById(passengerId) as Passenger;

                if (passenger == null)
                { 
                    throw new InvalidOperationException("Passenger not found.");
                }

                // Create the ride
                var ride = new Ride
                {
                    Id = GenerateRideId(),
                    PassengerId = passengerId,
                    PickupLocation = pickupLocation,
                    DropOffLocation = dropOffLocation,
                    DistanceKm = _locationService.CalculateDistance(pickupLocation, dropOffLocation)
                };

                ride.CalculateFare();

                //Check passenger can afford the ride
                if (!_paymentService.CanAfford(passenger, ride.Fare))
                {
                    throw new InvalidOperationException($"Insufficient funds. Required: R{ride.Fare:F2}, Available: R{passenger.GetBalance():F2}");
                }

                //Save the ride
                _rideRepository.AddRide(ride);

                Console.WriteLine($"✅ Ride requested successfully!");
                Console.WriteLine($"   Pickup: {pickupLocation}");
                Console.WriteLine($"   Drop-off: {dropOffLocation}");
                Console.WriteLine($"   Distance: {ride.DistanceKm:F1} km");
                Console.WriteLine($"   Estimated Fare: R{ride.Fare:F2}");
                Console.WriteLine($"   Ride ID: #{ride.Id}");

                return ride;

            }

            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error requesting ride: {ex.Message}");
                throw;
            }
        }


        //Find available drivers near a pickup location

        public List<Driver> FindNearbyDrivers(string pickUpLocation, double maxDistanceKm = 10.0)
        {
            try
            {
                // Get all users and filter for available drivers
                var nearbyDrivers = _userRepository.GetAllUsers()
                    .OfType<Driver>()
                    .Where(driver => driver.IsAvailable)
                    .Where(driver => driver.IsActive)
                    .Where(driver => _locationService.IsWithinDistance(driver.CurrentLocation, pickUpLocation, maxDistanceKm))
                    .OrderBy(driver => _locationService.GetDistanceToLocation(driver.CurrentLocation, pickUpLocation))
                    .ThenByDescending(driver => driver.GetAverageRating())
                    .ToList();

                Console.WriteLine($"🔍 Found {nearbyDrivers.Count} available drivers within {maxDistanceKm} km");

                return nearbyDrivers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error finding drivers: {ex.Message}");
                return new List<Driver>();
            }
        }

        //Match a ride with the best available driver
        public bool MatchRideWithDriver(int rideId)
        {
            try
            { 
                var ride = _rideRepository.GetRideById(rideId);
                if (ride == null)
                {
                    throw new InvalidOperationException("Ride not found.");
                }

                if (ride.Status != RideStatus.Requested)
                { 
                    throw new InvalidOperationException("Ride is not in a valid state for matching.");
                }

                // Find nearby drivers
                var availableDrivers = FindNearbyDrivers(ride.PickupLocation);
                if (!availableDrivers.Any())
                {
                    Console.WriteLine("❌ No available drivers found for this ride");
                    return false;
                }

                //Get best driver first in sorted list
                var bestDriver = availableDrivers.First();

                bestDriver.AcceptRide(ride);
                _rideRepository.UpdateRide(ride);
                _userRepository.UpdateUser(bestDriver);

                Console.WriteLine($"🎯 Ride #{rideId} matched with driver {bestDriver.Username}!");
                Console.WriteLine($"   Driver rating: {bestDriver.GetAverageRating():F1}/5.0");
                Console.WriteLine($"   Vehicle: {bestDriver.VehicleInfo}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error matching ride: {ex.Message}");
                return false;
            }

        }


        //Complete a ride and process payment

        public bool CompleteRide(int rideId, int driverId)
        {
            try
            { 
                var ride = _rideRepository.GetRideById(rideId);
                var driver = _userRepository.GetUserById(driverId) as Driver;
                var passenger = _userRepository.GetUserById(ride.PassengerId) as Passenger;

                if (ride == null || driver == null || passenger == null)
                {
                    throw new InvalidOperationException("Invalid ride, driver, or passenger.");
                }

                if (ride.Status != RideStatus.InProgress)
                {
                    throw new InvalidOperationException("Ride is not in progress.");
                }

                //Process payment
                if (_paymentService.ProccessRidePayment(passenger, driver, ride.Fare))
                { 
                    driver.CompleteRide(ride);
                    passenger.RideHistory.Add(ride);

                    // Update repos
                    _rideRepository.UpdateRide(ride);
                    _userRepository.UpdateUser(driver);
                    _userRepository.UpdateUser(passenger);

                    Console.WriteLine($"🎉 Ride #{rideId} completed successfully!");
                    Console.WriteLine($"   Fare: R{ride.Fare:F2}");
                    Console.WriteLine($"   Driver earned: R{ride.Fare:F2}");

                    return true;
                }
                else
                {
                    Console.WriteLine("❌ Payment failed - ride could not be completed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error completing ride: {ex.Message}");
                return false;
            }
        }


        //Get ride statistics

        public void DisplayRideStatistics()
        {
            try
            {
                var allRides = _rideRepository.GetAvailableRides();
                if (!allRides.Any())
                {
                    Console.WriteLine("📊 No rides found in the system.");
                    return;
                }

                var completedRides = allRides.Where(r => r.Status == RideStatus.Completed);
                var totalRides = completedRides.Count();
                var totalRevenue = completedRides.Sum(r => r.Fare);
                var averageFare = completedRides.Average(r => r.Fare);
                var longestRide = completedRides.Max(r => r.DistanceKm);
                var shortestRide = completedRides.Min(r => r.DistanceKm);

                //Most active passenger

                var mostActivePassenger = completedRides.GroupBy(r => r.PassengerId)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (mostActivePassenger != null)
                { 
                    var passenger = _userRepository.GetUserById(mostActivePassenger.Key) as Passenger;
                    Console.WriteLine($"Most active passenger: {passenger.Username} ({mostActivePassenger.Count()} rides)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error displaying ride statistics: {ex.Message}");
            }
        }


        private int GenerateRideId()
        {
            return new Random().Next(1000, 9999);
        }

    }
}

