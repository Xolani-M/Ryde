using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Ryde;


namespace Ryde
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Display welcome message
            ConsoleHelper.DisplayWelcomeMessage();

            // Main menu loop
            while (true)
            {
                var options = new List<string> { "Register as Passenger", "Register as Driver", "Login", "Admin Login", "View System Report", "Exit" };
                ConsoleHelper.DisplayMenu("Main Menu", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    RegisterPassenger();
                }
                else if (choice == 2)
                {
                    RegisterDriver();
                }
                else if (choice == 3)
                {
                    Login();
                }
                else if (choice == 4)
                {
                    AdminLogin();
                }
                else if (choice == 5)
                {
                    // Use persistent helpers for reporting
                    var users = Utils.UserStorageHelper.LoadUsers();
                    var rides = Utils.RideStorageHelper.LoadRides();
                    ReportHelper.GenerateSystemReport(users, rides);
                }
                else if (choice == 6)
                {
                    ConsoleHelper.DisplayInfo("Thank you for using Ryde!");
                    break;
                }
            }
        }

        static void RegisterPassenger()
        {
            ConsoleHelper.DisplayHeader("Register as Passenger");
            string username;
            while (true)
            {
                username = ConsoleHelper.GetStringInput("Enter username");
                if (string.IsNullOrWhiteSpace(username))
                {
                    ConsoleHelper.DisplayError("Username cannot be empty.");
                }
                else if (Utils.UserStorageHelper.GetUserByUsername(username) != null)
                {
                    ConsoleHelper.DisplayError("Username already exists. Please choose another.");
                }
                else
                {
                    break;
                }
            }
            string password;
            while (true)
            {
                password = ConsoleHelper.GetStringInput("Enter password (min 6 characters)");
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ConsoleHelper.DisplayError("Password must be at least 6 characters long.");
                }
                else
                {
                    break;
                }
            }
            string email;
            while (true)
            {
                email = ConsoleHelper.GetStringInput("Enter email");
                if (!Utils.ValidationHelper.IsValidEmail(email))
                {
                    ConsoleHelper.DisplayError("Invalid email format.");
                }
                else
                {
                    break;
                }
            }
            string phone;
            while (true)
            {
                phone = ConsoleHelper.GetStringInput("Enter phone number");
                if (!Utils.ValidationHelper.IsValidPhoneNumber(phone))
                {
                    ConsoleHelper.DisplayError("Invalid phone number format.");
                }
                else
                {
                    break;
                }
            }
            var passenger = new Passenger { Username = username, Password = password, Email = email, PhoneNumber = phone };
            Utils.UserStorageHelper.AddUser(passenger);
            ConsoleHelper.DisplaySuccess($"Passenger {username} registered!");
            ConsoleHelper.PauseForUser();
        }

        static void RegisterDriver()
        {
            ConsoleHelper.DisplayHeader("Register as Driver");
            string username;
            while (true)
            {
                username = ConsoleHelper.GetStringInput("Enter username");
                if (string.IsNullOrWhiteSpace(username))
                {
                    ConsoleHelper.DisplayError("Username cannot be empty.");
                }
                else if (Utils.UserStorageHelper.GetUserByUsername(username) != null)
                {
                    ConsoleHelper.DisplayError("Username already exists. Please choose another.");
                }
                else
                {
                    break;
                }
            }
            string password;
            while (true)
            {
                password = ConsoleHelper.GetStringInput("Enter password (min 6 characters)");
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ConsoleHelper.DisplayError("Password must be at least 6 characters long.");
                }
                else
                {
                    break;
                }
            }
            string email;
            while (true)
            {
                email = ConsoleHelper.GetStringInput("Enter email");
                if (!Utils.ValidationHelper.IsValidEmail(email))
                {
                    ConsoleHelper.DisplayError("Invalid email format.");
                }
                else
                {
                    break;
                }
            }
            string phone;
            while (true)
            {
                phone = ConsoleHelper.GetStringInput("Enter phone number");
                if (!Utils.ValidationHelper.IsValidPhoneNumber(phone))
                {
                    ConsoleHelper.DisplayError("Invalid phone number format.");
                }
                else
                {
                    break;
                }
            }
            string license = ConsoleHelper.GetStringInput("Enter license number");
            string vehicle = ConsoleHelper.GetStringInput("Enter vehicle info");
            var driver = new Driver { Username = username, Password = password, Email = email, PhoneNumber = phone, LicenseNumber = license, VehicleInfo = vehicle, IsAvailable = true };
            Utils.UserStorageHelper.AddUser(driver);
            ConsoleHelper.DisplaySuccess($"Driver {username} registered!");
            ConsoleHelper.PauseForUser();
        }

        static void Login()
        {
            ConsoleHelper.DisplayHeader("Login");
            string username = ConsoleHelper.GetStringInput("Enter username");
            string password = ConsoleHelper.GetStringInput("Enter password");
            var user = Utils.UserStorageHelper.GetUserByUsername(username);
            if (user == null || user.Password != password)
            {
                ConsoleHelper.DisplayError("Invalid username or password.");
                ConsoleHelper.PauseForUser();
                return;
            }
            if (user is Passenger passenger)
            {
                PassengerMenu(passenger);
            }
            else if (user is Driver driver)
            {
                DriverMenu(driver);
            }
            else
            {
                ConsoleHelper.DisplayError("Unknown user type.");
                ConsoleHelper.PauseForUser();
            }
        }

        static void AdminLogin()
        {
            ConsoleHelper.DisplayHeader("Admin Login");
            string username = ConsoleHelper.GetStringInput("Enter admin username");
            string password = ConsoleHelper.GetStringInput("Enter admin password");
            // For demo, hardcode admin credentials
            if (username == "admin" && password == "admin123")
            {
                AdminMenu();
            }
            else
            {
                ConsoleHelper.DisplayError("Invalid admin credentials.");
                ConsoleHelper.PauseForUser();
            }
        }

        static void AdminMenu()
        {
            while (true)
            {
                var options = new List<string> { "View Reports", "Flag Low-Rated Drivers", "Logout" };
                ConsoleHelper.DisplayMenu("Admin Menu", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    var users = Utils.UserStorageHelper.LoadUsers();
                    var rides = Utils.RideStorageHelper.LoadRides();
                    var ratings = Utils.RatingStorageHelper.LoadRatings();
                    int totalRides = rides.Count(r => r.Status == RideStatus.Completed);
                    decimal totalEarnings = rides.Where(r => r.Status == RideStatus.Completed).Sum(r => r.Fare);
                    double avgRating = ratings.Any() ? ratings.Average(r => r.Stars) : 0;
                    ConsoleHelper.DisplayInfo($"Total Rides Completed: {totalRides}");
                    ConsoleHelper.DisplayInfo($"Total Earnings (All Drivers): R{totalEarnings:F2}");
                    ConsoleHelper.DisplayInfo($"Average Driver Rating: {avgRating:F2}");
                }
                else if (choice == 2)
                {
                    var drivers = Utils.UserStorageHelper.LoadUsers().OfType<Driver>().ToList();
                    var ratings = Utils.RatingStorageHelper.LoadRatings();
                    double threshold = 3.0;
                    var flagged = drivers.Where(d => {
                        var drRatings = ratings.Where(r => r.ToUserId == d.Id).ToList();
                        return drRatings.Count > 0 && drRatings.Average(r => r.Stars) < threshold;
                    }).ToList();
                    if (flagged.Count == 0)
                        ConsoleHelper.DisplayInfo("No low-rated drivers to flag.");
                    else
                    {
                        ConsoleHelper.DisplayInfo($"Drivers flagged for review (rating below {threshold}):");
                        foreach (var d in flagged)
                        {
                            var drRatings = ratings.Where(r => r.ToUserId == d.Id).ToList();
                            double avg = drRatings.Average(r => r.Stars);
                            ConsoleHelper.DisplayInfo($"Driver: {d.Username}, Avg Rating: {avg:F2}");
                        }
                    }
                }
                else if (choice == 3)
                {
                    break;
                }
                ConsoleHelper.PauseForUser();
            }
        }

        static void PassengerMenu(Passenger passenger)
        {
            while (true)
            {
                var options = new List<string> { "Request a Ride", "View Wallet Balance", "Add Funds to Wallet", "View Ride History", "Rate a Driver", "Logout" };
                ConsoleHelper.DisplayMenu($"Passenger Menu - {passenger.Username}", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    string pickup = ConsoleHelper.GetStringInput("Enter pickup location");
                    string dropoff = ConsoleHelper.GetStringInput("Enter drop-off location");
                    double distance = Utils.LocationHelper.CalculateDistance(pickup, dropoff);
                    decimal fare = (decimal)(distance * 10);
                    ConsoleHelper.DisplayInfo($"Estimated fare: R{fare:F2} for {distance:F1} km");
                    if (passenger.WalletBalance < fare)
                    {
                        ConsoleHelper.DisplayError("Insufficient wallet balance for this ride.");
                    }
                    else
                    {
                        // Create a pending ride (no driver assigned yet)
                        var ride = new Ride
                        {
                            Id = new Random().Next(100000, 999999),
                            PassengerId = passenger.Id,
                            DriverId = null,
                            PickupLocation = pickup,
                            DropOffLocation = dropoff,
                            Fare = fare,
                            Status = RideStatus.Requested,
                            RequestedAt = DateTime.Now,
                            DistanceKm = distance
                        };
                        Utils.RideStorageHelper.SaveRides(Utils.RideStorageHelper.LoadRides().Append(ride).ToList());
                        ConsoleHelper.DisplaySuccess($"Ride requested! Waiting for a driver to accept.");
                    }
                }
                else if (choice == 2)
                {
                    ConsoleHelper.DisplayInfo($"Wallet Balance: R{passenger.WalletBalance:F2}");
                }
                else if (choice == 3)
                {
                    decimal amount = ConsoleHelper.GetDecimalInput("Enter amount to add", 1);
                    passenger.WalletBalance += amount;
                    Utils.UserStorageHelper.UpdateUser(passenger);
                    ConsoleHelper.DisplaySuccess($"Added R{amount:F2} to wallet. New balance: R{passenger.WalletBalance:F2}");
                }
                else if (choice == 4)
                {
                    // Always reload rides to reflect latest status
                    var rides = Utils.RideStorageHelper.LoadRides().Where(r => r.PassengerId == passenger.Id).OrderByDescending(r => r.RequestedAt).ToList();
                    if (rides.Count == 0)
                        ConsoleHelper.DisplayInfo("No rides taken yet.");
                    else
                    {
                        foreach (var ride in rides)
                        {
                            string statusMsg = $"Ride #{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} - R{ride.Fare:F2} [{ride.Status}]";
                            if (ride.Status == RideStatus.InProgress && ride.DriverId != null)
                            {
                                var driver = Utils.UserStorageHelper.GetUserById(ride.DriverId.Value) as Driver;
                                statusMsg += $"\n  🚗 Your ride is in progress with driver: {driver?.Username ?? "Unknown"}";
                            }
                            else if (ride.Status == RideStatus.Requested)
                            {
                                statusMsg += "\n  ⏳ Waiting for a driver to accept.";
                            }
                            else if (ride.Status == RideStatus.Completed && ride.DriverId != null)
                            {
                                var driver = Utils.UserStorageHelper.GetUserById(ride.DriverId.Value) as Driver;
                                statusMsg += $"\n  ✅ Completed by driver: {driver?.Username ?? "Unknown"}";
                            }
                            ConsoleHelper.DisplayInfo(statusMsg);
                        }
                        // Show ratings given by this passenger
                        var ratings = Utils.RatingStorageHelper.LoadRatings().Where(r => r.FromUserId == passenger.Id).ToList();
                        if (ratings.Count > 0)
                        {
                            ConsoleHelper.DisplayInfo("\n=== Ratings Given ===");
                            foreach (var rating in ratings)
                            {
                                var driver = Utils.UserStorageHelper.GetUserById(rating.ToUserId) as Driver;
                                ConsoleHelper.DisplayInfo($"Driver: {driver?.Username}, Stars: {rating.Stars}, Comment: {rating.Comment}");
                            }
                        }
                    }
                }
                else if (choice == 5)
                {
                    var drivers = Utils.UserStorageHelper.LoadUsers().OfType<Driver>().ToList();
                    if (drivers.Count == 0)
                        ConsoleHelper.DisplayInfo("No drivers available to rate.");
                    else
                    {
                        ConsoleHelper.DisplayMenu("Select Driver to Rate", drivers.Select(d => d.Username).ToList());
                        int driverChoice = ConsoleHelper.GetMenuChoice(drivers.Count);
                        var driver = drivers[driverChoice - 1];
                        int stars = ConsoleHelper.GetIntInput("Enter rating (1-5)", 1, 5);
                        string comment = ConsoleHelper.GetStringInput("Enter comment (optional)", false);
                        var rating = new Rating
                        {
                            FromUserId = passenger.Id,
                            ToUserId = driver.Id,
                            Stars = stars,
                            Comment = comment,
                            CreatedAt = DateTime.Now
                        };
                        Utils.RatingStorageHelper.AddRating(rating);
                        ConsoleHelper.DisplaySuccess($"Rated driver {driver.Username} with {stars} stars.");
                        // Update driver's average rating in persistent storage (optional, for analytics or admin)
                        var allRatings = Utils.RatingStorageHelper.LoadRatings().Where(r => r.ToUserId == driver.Id).ToList();
                        if (allRatings.Count > 0)
                        {
                            double avg = allRatings.Average(r => r.Stars);
                            ConsoleHelper.DisplayInfo($"{driver.Username}'s new average rating: {avg:F2} stars");
                        }
                    }
                }
                else if (choice == 6)
                {
                    break;
                }
                ConsoleHelper.PauseForUser();
            }
        }

        static void DriverMenu(Driver driver)
        {
            while (true)
            {
                var options = new List<string> { "View Available Ride Requests", "Accept a Ride", "Complete a Ride", "View Earnings", "Set Availability", "Logout" };
                ConsoleHelper.DisplayMenu($"Driver Menu - {driver.Username}", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    // Only show rides near this driver and if driver is available
                    if (!driver.IsAvailable)
                    {
                        ConsoleHelper.DisplayInfo("You are currently unavailable. Set yourself as available to view ride requests.");
                    }
                    else
                    {
                        var pendingRides = Utils.RideStorageHelper.LoadRides()
                            .Where(r => r.Status == RideStatus.Requested && r.DriverId == null)
                            .Where(r => Utils.LocationHelper.CalculateDistance(driver.CurrentLocation?.ToString() ?? "", r.PickupLocation) <= 10)
                            .ToList();
                        if (pendingRides.Count == 0)
                            ConsoleHelper.DisplayInfo("No pending ride requests near you.");
                        else
                            foreach (var ride in pendingRides)
                            {
                                var passenger = Utils.UserStorageHelper.GetUserById(ride.PassengerId);
                                ConsoleHelper.DisplayInfo($"Ride ID: {ride.Id}, Passenger: {passenger?.Username}, From: {ride.PickupLocation}, To: {ride.DropOffLocation}, Fare: R{ride.Fare:F2}");
                            }
                    }
                }
                else if (choice == 2)
                {
                    if (!driver.IsAvailable)
                    {
                        ConsoleHelper.DisplayInfo("You are currently unavailable. Set yourself as available to accept rides.");
                    }
                    else
                    {
                        Console.Write("Enter Ride ID to accept: ");
                        var input = Console.ReadLine();
                        if (int.TryParse(input, out int rideId))
                        {
                            var rides = Utils.RideStorageHelper.LoadRides();
                            var ride = rides.FirstOrDefault(r => r.Id == rideId && r.Status == RideStatus.Requested && r.DriverId == null);
                            if (ride == null)
                            {
                                ConsoleHelper.DisplayError("Ride not found or already accepted.");
                            }
                            else if (Utils.LocationHelper.CalculateDistance(driver.CurrentLocation?.ToString() ?? "", ride.PickupLocation) > 10)
                            {
                                ConsoleHelper.DisplayError("This ride is too far from your current location.");
                            }
                            else
                            {
                                ride.DriverId = driver.Id;
                                ride.Status = RideStatus.InProgress;
                                ride.AcceptedAt = DateTime.Now;
                                Utils.RideStorageHelper.SaveRides(rides);
                                ConsoleHelper.DisplaySuccess($"Accepted ride ID: {ride.Id}");
                            }
                        }
                        else
                        {
                            ConsoleHelper.DisplayError("Invalid Ride ID.");
                        }
                    }
                }
                else if (choice == 3)
                {
                    var rides = Utils.RideStorageHelper.LoadRides().Where(r => r.Status == RideStatus.InProgress && r.DriverId == driver.Id).ToList();
                    if (rides.Count == 0)
                        ConsoleHelper.DisplayInfo("No rides in progress.");
                    else
                    {
                        var ride = rides.First();
                        ride.Status = RideStatus.Completed;
                        ride.CompletedAt = DateTime.Now;
                        driver.IsAvailable = true;
                        Utils.RideStorageHelper.SaveRides(Utils.RideStorageHelper.LoadRides());
                        Utils.UserStorageHelper.UpdateUser(driver);
                        ConsoleHelper.DisplaySuccess($"Completed ride ID: {ride.Id}");
                    }
                }
                else if (choice == 4)
                {
                    var rides = Utils.RideStorageHelper.LoadRides().Where(r => r.DriverId == driver.Id).ToList();
                    decimal totalEarnings = rides.Where(r => r.Status == RideStatus.Completed).Sum(r => r.Fare);
                    ConsoleHelper.DisplayInfo($"Total Earnings: R{totalEarnings:F2}");
                    // Show ratings received by this driver
                    var ratings = Utils.RatingStorageHelper.LoadRatings().Where(r => r.ToUserId == driver.Id).ToList();
                    if (ratings.Count > 0)
                    {
                        ConsoleHelper.DisplayInfo("\n=== Ratings Received ===");
                        foreach (var rating in ratings)
                        {
                            var passenger = Utils.UserStorageHelper.GetUserById(rating.FromUserId) as Passenger;
                            ConsoleHelper.DisplayInfo($"Passenger: {passenger?.Username}, Stars: {rating.Stars}, Comment: {rating.Comment}");
                        }
                        double avg = ratings.Average(r => r.Stars);
                        ConsoleHelper.DisplayInfo($"Average Rating: {avg:F2} stars");
                    }
                }
                else if (choice == 5)
                {
                    // Set availability
                    ConsoleHelper.DisplayInfo($"Current status: {(driver.IsAvailable ? "Available" : "Unavailable")}");
                    ConsoleHelper.DisplayMenu("Set Availability", new List<string> { "Available", "Unavailable" });
                    int availChoice = ConsoleHelper.GetMenuChoice(2);
                    driver.IsAvailable = availChoice == 1;
                    Utils.UserStorageHelper.UpdateUser(driver);
                    ConsoleHelper.DisplaySuccess($"Availability updated: {(driver.IsAvailable ? "Available" : "Unavailable")}");
                }
                else if (choice == 6)
                {
                    break;
                }
                ConsoleHelper.PauseForUser();
            }
        }
    }
}
