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
                // Prompt driver to set current location on login
                ConsoleHelper.DisplayHeader("Set Your Current Location");
                ConsoleHelper.DisplayInfo("Select your current location from the supported locations:");
                var locations = Utils.LocationHelper.GetSupportedLocations();
                ConsoleHelper.DisplayMenu("Supported Locations", locations);
                int locChoice = ConsoleHelper.GetMenuChoice(locations.Count);
                var selectedLocation = locations[locChoice - 1];
                var coords = Utils.LocationHelper.GetCoordinatesForLocation(selectedLocation);
                driver.CurrentLocation = new Location(coords.Latitude, coords.Longitude);
                Utils.UserStorageHelper.UpdateUser(driver);
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
                // Show current active ride at the top of the passenger menu
                // Update passenger menu active ride status display to exclude cancelled/completed rides
                var menuActiveRide = Utils.RideStorageHelper.LoadRides().FirstOrDefault(r => r.PassengerId == passenger.Id &&
                    (r.Status == RideStatus.Requested || r.Status == RideStatus.Accepted || r.Status == RideStatus.EnRouteToPickup || r.Status == RideStatus.ArrivedAtPickup || r.Status == RideStatus.InProgress));
                if (menuActiveRide != null)
                {
                    string driverName = "Unknown";
                    if (menuActiveRide.DriverId != null)
                    {
                        var driver = Utils.UserStorageHelper.GetUserById(menuActiveRide.DriverId.Value) as Driver;
                        driverName = driver?.Username ?? "Unknown";
                    }
                    string statusMsg = $"\n🚕 Active Ride: #{menuActiveRide.Id} | {menuActiveRide.PickupLocation} → {menuActiveRide.DropOffLocation} | Fare: R{menuActiveRide.Fare:F2} | Status: {menuActiveRide.Status}";
                    switch (menuActiveRide.Status)
                    {
                        case RideStatus.Requested:
                            statusMsg += menuActiveRide.DriverId != null ? $"\n  🎉 Driver: {driverName} accepted your ride! Waiting for driver to start heading to you." : "\n  ⏳ Waiting for a driver to accept.";
                            break;
                        case RideStatus.Accepted:
                            statusMsg += $"\n  🎉 Driver: {driverName} accepted your ride! Waiting for driver to start heading to you.";
                            break;
                        case RideStatus.EnRouteToPickup:
                            statusMsg += $"\n  🚗 Driver: {driverName} is on the way to your pickup location!";
                            break;
                        case RideStatus.ArrivedAtPickup:
                            statusMsg += $"\n  📍 Driver: {driverName} has arrived at your pickup location. Please meet your driver!";
                            break;
                        case RideStatus.InProgress:
                            statusMsg += $"\n  ✅ Ride in progress to your destination.";
                            break;
                    }
                    ConsoleHelper.DisplayInfo(statusMsg);
                }
                var options = new List<string> { "Request a Ride", "View Wallet Balance", "Add Funds to Wallet", "View Ride History", "Cancel a Ride", "Rate a Driver", "Logout" };
                ConsoleHelper.DisplayMenu($"Passenger Menu - {passenger.Username}", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    // Prevent multiple active ride requests
                    var activeRide = Utils.RideStorageHelper.LoadRides().FirstOrDefault(r => r.PassengerId == passenger.Id && (r.Status == RideStatus.Requested || r.Status == RideStatus.InProgress));
                    if (activeRide != null)
                    {
                        ConsoleHelper.DisplayError($"You already have an active ride (#{activeRide.Id}) from {activeRide.PickupLocation} to {activeRide.DropOffLocation}. Please wait until it is accepted and completed or cancel it before requesting another ride.");
                        continue;
                    }
                    // Use improved location input with suggestions and validation
                    string pickup = Utils.LocationHelper.GetLocationFromUser("Enter pickup location:");
                    string dropoff = Utils.LocationHelper.GetLocationFromUser("Enter drop-off location:");
                    var pickupCoords = Utils.LocationHelper.GetCoordinates(pickup);
                    if (pickupCoords == null)
                    {
                        ConsoleHelper.DisplayError("Unknown pickup location. Please use a supported location.");
                        continue;
                    }
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
                            PickupLatitude = pickupCoords.Value.Latitude,
                            PickupLongitude = pickupCoords.Value.Longitude,
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
                            string driverName = "Unknown";
                            if (ride.DriverId != null)
                            {
                                var driver = Utils.UserStorageHelper.GetUserById(ride.DriverId.Value) as Driver;
                                driverName = driver?.Username ?? "Unknown";
                            }
                            string statusMsg = $"Ride #{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} - R{ride.Fare:F2} [{ride.Status}]";
                            if (ride.Status == RideStatus.InProgress)
                            {
                                statusMsg += $"\n  🚗 Your ride is in progress! Driver: {driverName} is on the way.";
                            }
                            else if (ride.Status == RideStatus.Requested)
                            {
                                statusMsg += "\n  ⏳ Waiting for a driver to accept.";
                            }
                            else if (ride.Status == RideStatus.Completed)
                            {
                                statusMsg += $"\n  ✅ Completed by driver: {driverName}";
                            }
                            else if (ride.Status == RideStatus.Cancelled)
                            {
                                statusMsg += "\n  ❌ This ride was cancelled.";
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
                    // Cancel a Ride
                    // Only allow cancelling rides that are truly active
                    var allRides = Utils.RideStorageHelper.LoadRides();
                    var rides = allRides.Where(r => r.PassengerId == passenger.Id &&
                        (r.Status == RideStatus.Requested || r.Status == RideStatus.Accepted || r.Status == RideStatus.EnRouteToPickup || r.Status == RideStatus.ArrivedAtPickup || r.Status == RideStatus.InProgress)).ToList();
                    if (rides.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No rides to cancel.");
                    }
                    else
                    {
                        ConsoleHelper.DisplayMenu("Select a ride to cancel", rides.Select(r => $"Ride #{r.Id}: {r.PickupLocation} → {r.DropOffLocation} - {r.Status}").ToList());
                        int cancelChoice = ConsoleHelper.GetMenuChoice(rides.Count);
                        var ride = rides[cancelChoice - 1];
                        // Find the ride in the main list and update it
                        var idx = allRides.FindIndex(r => r.Id == ride.Id);
                        if (idx >= 0) {
                            allRides[idx].Status = RideStatus.Cancelled;
                            // If the ride was never accepted by a driver, set DriverId to null
                            if (allRides[idx].DriverId == null || allRides[idx].DriverId == 0) {
                                allRides[idx].DriverId = null;
                            }
                            Utils.RideStorageHelper.SaveRides(allRides);
                            ConsoleHelper.DisplaySuccess($"Ride #{ride.Id} cancelled.");
                        }
                    }
                    // Reload rides after cancellation to update menu display
                    continue;
                }
                else if (choice == 6)
                {
                    // Only allow rating for completed rides
                    var completedRides = Utils.RideStorageHelper.LoadRides().Where(r => r.PassengerId == passenger.Id && r.Status == RideStatus.Completed && r.DriverId != null).ToList();
                    if (completedRides.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No completed rides to rate.");
                    }
                    else
                    {
                        // Only include non-null drivers
                        var drivers = completedRides
                            .Select(r => r.DriverId != null ? Utils.UserStorageHelper.GetUserById(r.DriverId.Value) as Driver : null)
                            .Where(d => d != null)
                            .Distinct()
                            .ToList();
                        if (drivers.Count == 0)
                        {
                            ConsoleHelper.DisplayInfo("No drivers found for completed rides.");
                        }
                        else
                        {
                            ConsoleHelper.DisplayMenu("Select Driver to Rate", drivers.Select(d => d!.Username).ToList());
                            int driverChoice = ConsoleHelper.GetMenuChoice(drivers.Count);
                            var driver = drivers[driverChoice - 1];
                            if (driver != null)
                            {
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
                    }
                }
                else if (choice == 7)
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
                // Notify driver of any rides cancelled by passengers
                var cancelledRides = Utils.RideStorageHelper.LoadRides().Where(r => r.DriverId == driver.Id && r.Status == RideStatus.Cancelled).ToList();
                if (cancelledRides.Count > 0)
                {
                    foreach (var ride in cancelledRides)
                    {
                        var passenger = Utils.UserStorageHelper.GetUserById(ride.PassengerId);
                        ConsoleHelper.DisplayInfo($"❌ Ride #{ride.Id} from {ride.PickupLocation} to {ride.DropOffLocation} was cancelled by passenger: {passenger?.Username ?? "Unknown"}.");
                    }
                    // Optionally, remove or archive cancelled rides after notification
                }

                // Show driver availability status at the top of the menu
                Console.WriteLine($"\n🚦 Availability: {(driver.IsAvailable ? "Available" : "Unavailable")}");
                var options = new List<string> { "View Available Ride Requests", "Accept a Ride", "Complete a Ride", "Cancel a Ride", "View Earnings", "Set Availability", "Logout" };
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
                            .Where(r =>
                            {
                                // Use coordinates for distance calculation
                                var rideCoords = (r.PickupLatitude, r.PickupLongitude);
                                var driverCoords = (driver.CurrentLocation?.Latitude ?? 0, driver.CurrentLocation?.Longitude ?? 0);
                                return Utils.LocationHelper.CalculateDistanceKm(driverCoords.Item1, driverCoords.Item2, rideCoords.PickupLatitude, rideCoords.PickupLongitude) <= 10;
                            })
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
                    // Prevent driver from accepting multiple rides at once
                    var activeRide = Utils.RideStorageHelper.LoadRides().FirstOrDefault(r => r.DriverId == driver.Id && (r.Status == RideStatus.InProgress || r.Status == RideStatus.EnRouteToPickup || r.Status == RideStatus.ArrivedAtPickup || r.Status == RideStatus.Accepted));
                    if (activeRide != null)
                    {
                        ConsoleHelper.DisplayError($"You already have an active ride (#{activeRide.Id}) from {activeRide.PickupLocation} to {activeRide.DropOffLocation}. Complete or cancel it before accepting another ride.");
                        return;
                    }
                    if (!driver.IsAvailable)
                    {
                        ConsoleHelper.DisplayInfo("You are currently unavailable. Set yourself as available to accept rides.");
                    }
                    else
                    {
                        var pendingRides = Utils.RideStorageHelper.LoadRides()
                            .Where(r => r.Status == RideStatus.Requested && r.DriverId == null)
                            .Where(r =>
                            {
                                // Use coordinates for distance calculation
                                var rideCoords = (r.PickupLatitude, r.PickupLongitude);
                                var driverCoords = (driver.CurrentLocation?.Latitude ?? 0, driver.CurrentLocation?.Longitude ?? 0);
                                return Utils.LocationHelper.CalculateDistanceKm(driverCoords.Item1, driverCoords.Item2, rideCoords.PickupLatitude, rideCoords.PickupLongitude) <= 10;
                            })
                            .ToList();
                        if (pendingRides.Count == 0)
                        {
                            ConsoleHelper.DisplayInfo("No pending ride requests near you.");
                        }
                        else
                        {
                            ConsoleHelper.DisplayMenu("Available Ride Requests", pendingRides.Select((r, i) => $"Passenger: {Utils.UserStorageHelper.GetUserById(r.PassengerId)?.Username}, From: {r.PickupLocation}, To: {r.DropOffLocation}, Fare: R{r.Fare:F2}").ToList());
                            int rideChoice = ConsoleHelper.GetMenuChoice(pendingRides.Count);
                            var ride = pendingRides[rideChoice - 1];
                            // Only allow state transition if no driver is assigned
                            if (ride.DriverId != null)
                            {
                                ConsoleHelper.DisplayError("This ride has already been accepted by another driver.");
                                return;
                            }
                            ride.DriverId = driver.Id;
                            ride.Status = RideStatus.Accepted;
                            ride.AcceptedAt = DateTime.Now;
                            var allRides = Utils.RideStorageHelper.LoadRides();
                            var idx = allRides.FindIndex(r => r.Id == ride.Id);
                            if (idx >= 0) {
                                allRides[idx] = ride;
                                Utils.RideStorageHelper.SaveRides(allRides);
                                ConsoleHelper.DisplaySuccess($"🎉 You have accepted ride ID: {ride.Id}.");
                                Console.WriteLine("Press any key when you start heading to the pickup location...");
                                Console.ReadKey();
                                // Only allow assigned driver to advance state
                                if (ride.DriverId != driver.Id) {
                                    ConsoleHelper.DisplayError("Only the assigned driver can advance this ride.");
                                    return;
                                }
                                ride.Status = RideStatus.EnRouteToPickup;
                                allRides[idx] = ride;
                                Utils.RideStorageHelper.SaveRides(allRides);
                                ConsoleHelper.DisplayInfo($"🚗 You are now en route to the pickup location for passenger: {Utils.UserStorageHelper.GetUserById(ride.PassengerId)?.Username}");
                                Console.WriteLine("Press any key when you have arrived at the pickup location...");
                                Console.ReadKey();
                                if (ride.DriverId != driver.Id) {
                                    ConsoleHelper.DisplayError("Only the assigned driver can advance this ride.");
                                    return;
                                }
                                ride.Status = RideStatus.ArrivedAtPickup;
                                allRides[idx] = ride;
                                Utils.RideStorageHelper.SaveRides(allRides);
                                ConsoleHelper.DisplayInfo($"📍 You have arrived at the pickup location. Wait for the passenger to board.");
                                Console.WriteLine("Press any key when the passenger has boarded and the ride is starting...");
                                Console.ReadKey();
                                if (ride.DriverId != driver.Id) {
                                    ConsoleHelper.DisplayError("Only the assigned driver can advance this ride.");
                                    return;
                                }
                                ride.Status = RideStatus.InProgress;
                                allRides[idx] = ride;
                                Utils.RideStorageHelper.SaveRides(allRides);
                                ConsoleHelper.DisplayInfo($"✅ Ride in progress! Taking passenger to their destination.");
                                Console.WriteLine("Press any key to complete this ride...");
                                Console.ReadKey();
                                if (ride.DriverId != driver.Id) {
                                    ConsoleHelper.DisplayError("Only the assigned driver can complete this ride.");
                                    return;
                                }
                                ride.Status = RideStatus.Completed;
                                ride.CompletedAt = DateTime.Now;
                                allRides[idx] = ride;
                                Utils.RideStorageHelper.SaveRides(allRides);
                                ConsoleHelper.DisplaySuccess($"🏁 Ride #{ride.Id} completed!");
                            } else {
                                ConsoleHelper.DisplayError("Ride not found in storage.");
                            }
                        }
                    }
                }
                else if (choice == 3)
                {
                    // Only allow one in-progress ride at a time for the driver
                    var ride = Utils.RideStorageHelper.LoadRides().FirstOrDefault(r => r.Status == RideStatus.InProgress && r.DriverId == driver.Id);
                    if (ride == null)
                    {
                        ConsoleHelper.DisplayInfo("No rides in progress.");
                    }
                    else
                    {
                        var passenger = Utils.UserStorageHelper.GetUserById(ride.PassengerId);
                        ConsoleHelper.DisplayInfo($"Ride in progress: #{ride.Id} | {ride.PickupLocation} → {ride.DropOffLocation} | Passenger: {passenger?.Username ?? "Unknown"} | Fare: R{ride.Fare:F2}");
                        ConsoleHelper.DisplayInfo("Completing this ride...");
                        ride.Status = RideStatus.Completed;
                        ride.CompletedAt = DateTime.Now;
                        driver.IsAvailable = true;
                        var allRides = Utils.RideStorageHelper.LoadRides();
                        var idx = allRides.FindIndex(r => r.Id == ride.Id);
                        if (idx >= 0) {
                            allRides[idx] = ride;
                            Utils.RideStorageHelper.SaveRides(allRides);
                        }
                        Utils.UserStorageHelper.UpdateUser(driver);
                        ConsoleHelper.DisplaySuccess($"Completed ride ID: {ride.Id}");
                    }
                }
                else if (choice == 4)
                {
                    // Cancel a Ride
                    var rides = Utils.RideStorageHelper.LoadRides().Where(r => r.DriverId == driver.Id && (r.Status == RideStatus.InProgress || r.Status == RideStatus.Requested)).ToList();
                    if (rides.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No rides to cancel.");
                    }
                    else
                    {
                        ConsoleHelper.DisplayMenu("Select a ride to cancel", rides.Select(r => $"Ride #{r.Id}: {r.PickupLocation} → {r.DropOffLocation} - {r.Status}").ToList());
                        int cancelChoice = ConsoleHelper.GetMenuChoice(rides.Count);
                        var ride = rides[cancelChoice - 1];
                        ride.Status = RideStatus.Cancelled;
                        var allRides = Utils.RideStorageHelper.LoadRides();
                        var idx = allRides.FindIndex(r => r.Id == ride.Id);
                        if (idx >= 0) {
                            allRides[idx] = ride;
                            Utils.RideStorageHelper.SaveRides(allRides);
                            ConsoleHelper.DisplaySuccess($"Ride #{ride.Id} cancelled.");
                        }
                    }
                }
                else if (choice == 5)
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
                else if (choice == 6)
                {
                    // Set availability
                    ConsoleHelper.DisplayInfo($"Current status: {(driver.IsAvailable ? "Available" : "Unavailable")}");
                    ConsoleHelper.DisplayMenu("Set Availability", new List<string> { "Available", "Unavailable" });
                    int availChoice = ConsoleHelper.GetMenuChoice(2);
                    driver.IsAvailable = availChoice == 1;
                    Utils.UserStorageHelper.UpdateUser(driver);
                    ConsoleHelper.DisplaySuccess($"Availability updated: {(driver.IsAvailable ? "Available" : "Unavailable")}");
                }
                else if (choice == 7)
                {
                    break;
                }
                ConsoleHelper.PauseForUser();
            }
        }
    }
}
