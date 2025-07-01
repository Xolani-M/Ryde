using System;
using System.Collections.Generic;
using Ryde.Interfaces;
using Ryde;


namespace Ryde
{
    public class Driver : User, IRideable
    {
        public bool IsAvailable { get; set; }
        public string LicenseNumber { get; set; }
        public string VehicleInfo { get; set; }
        public decimal TotalEarnings { get; set; }
        public List<Ride> CompletedRides { get; set; }
        public Location CurrentLocation { get; set; }
public class UserRepository : IUserRepository
{
    private List<User> _users;

    public UserRepository()
    {
        _users = new List<User>();
    }

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public User GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public void UpdateUser(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users[_users.IndexOf(existingUser)] = user;
        }
    }

    public bool DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            return _users.Remove(user);
        }
        return false;
    }
}

public class RideRepository : IRideRepository
{
    private List<Ride> _rides;

    public RideRepository()
    {
        _rides = new List<Ride>();
    }

    public void AddRide(Ride ride)
    {
        _rides.Add(ride);
    }

    public Ride GetRideById(int id)
    {
        return _rides.FirstOrDefault(r => r.Id == id);
    }

    public List<Ride> GetRidesByPassengerId(int passengerId)
    {
        return _rides.Where(r => r.PassengerId == passengerId).ToList();
    }

    public List<Ride> GetRidesByDriverId(int driverId)
    {
        return _rides.Where(r => r.DriverId == driverId).ToList();
    }

    public List<Ride> GetAvailableRides()
    {
        return _rides.Where(r => r.Status == RideStatus.Requested).ToList();
    }

    public void UpdateRide(Ride ride)
    {
        var existingRide = _rides.FirstOrDefault(r => r.Id == ride.Id);
        if (existingRide != null)
        {
            _rides[_rides.IndexOf(existingRide)] = ride;
        }
    }
}
        public Driver() : base()
        {
            IsAvailable = true;
            TotalEarnings = 0;
            CompletedRides = new List<Ride>();
            CurrentLocation = new Location(0, 0); // Default location
        }


        // Implementing IRideable interface (differently than Passenger!)
        public void RequestRide(string pickupLocation, string dropOffLocation)
        {
            Console.WriteLine($"Driver {Username} cannot request rides - they provide them!");
        }

        public void CancelRide(int rideId)
        {
            Console.WriteLine($"Driver {Username} cancelled ride #{rideId}");
        }

        public void ViewRideHistory()
        {
            Console.WriteLine($"\n=== Rides Completed by {Username} ===");
            if (CompletedRides.Count == 0)
            {
                Console.WriteLine("No rides completed yet.");
                return;
            }

            foreach (var ride in CompletedRides)
            {
                Console.WriteLine($"Ride #{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} - Earned: ${ride.Fare:F2}");
            }
        }


        // Driver-specific methods
        public void AcceptRide(Ride ride)
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"Driver {Username} is not available to accept rides.");
                return;
            }

            Console.WriteLine($"Driver {Username} accepted ride #{ride.Id}");
            IsAvailable = false;
            ride.Status = RideStatus.InProgress;
            ride.DriverId = this.Id;
        }

        public void CompleteRide(Ride ride)
        {
            ride.Status = RideStatus.Completed;
            ride.CompletedAt = DateTime.Now;
            TotalEarnings += ride.Fare;
            CompletedRides.Add(ride);
            IsAvailable = true;

            Console.WriteLine($"Ride #{ride.Id} completed! Earned: ${ride.Fare:F2}");
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            Console.WriteLine($"Driver {Username} is now {(isAvailable ? "available" : "unavailable")}");
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"License: {LicenseNumber}");
            Console.WriteLine($"Vehicle: {VehicleInfo}");
            Console.WriteLine($"Status: {(IsAvailable ? "Available" : "Busy")}");
            Console.WriteLine($"Total Earnings: ${TotalEarnings:F2}");
            Console.WriteLine($"Completed Rides: {CompletedRides.Count}");
        }
    }
}
