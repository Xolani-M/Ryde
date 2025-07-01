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

            // Initialize repositories
            var userRepository = new Data.UserRepository();
            var rideRepository = new Data.RideRepository();

            // Initialize services
            var paymentService = new Services.PaymentService();
            var locationService = new Services.LocationService();
            var rideService = new Services.RideService(userRepository, rideRepository, paymentService, locationService);
            var ratingService = new Services.RatingService(userRepository);

            // sample data
            var dbInitializer = new Data.DatabaseInitializer(userRepository, rideRepository);
            dbInitializer.InitializeWithSampleData();

            // Main menu loop
            while (true)
            {
                var options = new List<string> { "Register as Passenger", "Register as Driver", "Login", "View System Report", "Exit" };
                ConsoleHelper.DisplayMenu("Main Menu", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    RegisterPassenger(userRepository);
                }
                else if (choice == 2)
                {
                    RegisterDriver(userRepository);
                }
                else if (choice == 3)
                {
                    Login(userRepository, rideRepository, rideService, paymentService, locationService, ratingService);
                }
                else if (choice == 4)
                {
                    ReportHelper.GenerateSystemReport(userRepository.GetAllUsers(), rideRepository.GetAllRides());
                }
                else if (choice == 5)
                {
                    ConsoleHelper.DisplayInfo("Thank you for using Ryde!");
                    break;
                }
            }
        }

        static void RegisterPassenger(Data.UserRepository userRepository)
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
                else if (userRepository.GetUserByUsername(username) != null)
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
                if (!ValidationHelper.IsValidEmail(email))
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
                if (!ValidationHelper.IsValidPhoneNumber(phone))
                {
                    ConsoleHelper.DisplayError("Invalid phone number format.");
                }
                else
                {
                    break;
                }
            }
            var passenger = new Passenger { Username = username, Password = password, Email = email, PhoneNumber = phone };
            userRepository.AddUser(passenger);
            ConsoleHelper.DisplaySuccess($"Passenger {username} registered!");
            ConsoleHelper.PauseForUser();
        }

        static void RegisterDriver(Data.UserRepository userRepository)
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
                else if (userRepository.GetUserByUsername(username) != null)
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
                if (!ValidationHelper.IsValidEmail(email))
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
                if (!ValidationHelper.IsValidPhoneNumber(phone))
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
            userRepository.AddUser(driver);
            ConsoleHelper.DisplaySuccess($"Driver {username} registered!");
            ConsoleHelper.PauseForUser();
        }

        static void Login(Data.UserRepository userRepository, Data.RideRepository rideRepository, Services.RideService rideService, Services.PaymentService paymentService, Services.LocationService locationService, Services.RatingService ratingService)
        {
            ConsoleHelper.DisplayHeader("Login");
            string username = ConsoleHelper.GetStringInput("Enter username");
            string password = ConsoleHelper.GetStringInput("Enter password");
            var user = userRepository.GetUserByUsername(username);
            if (user == null || user.Password != password)
            {
                ConsoleHelper.DisplayError("Invalid username or password.");
                ConsoleHelper.PauseForUser();
                return;
            }
            if (user is Passenger passenger)
            {
                PassengerMenu(passenger, userRepository, rideRepository, rideService, paymentService, locationService, ratingService);
            }
            else if (user is Driver driver)
            {
                DriverMenu(driver, userRepository, rideRepository, rideService);
            }
            else
            {
                ConsoleHelper.DisplayError("Unknown user type.");
                ConsoleHelper.PauseForUser();
            }
        }

        static void PassengerMenu(Passenger passenger, Data.UserRepository userRepository, Data.RideRepository rideRepository, Services.RideService rideService, Services.PaymentService paymentService, Services.LocationService locationService, Services.RatingService ratingService)
        {
            while (true)
            {
                var options = new List<string> { "Request a Ride", "View Wallet Balance", "Add Funds to Wallet", "View Ride History", "Rate a Driver", "Logout" };
                ConsoleHelper.DisplayMenu($"Passenger Menu - {passenger.Username}", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    // Prompt for ride details
                    string pickup = ConsoleHelper.GetStringInput("Enter pickup location");
                    string dropoff = ConsoleHelper.GetStringInput("Enter drop-off location");
                    try
                    {
                        // Find available drivers near pickup location
                        var drivers = userRepository.GetAllUsers().OfType<Driver>().Where(d => d.IsAvailable && locationService.IsWithinDistance(d.CurrentLocation, pickup, 10)).ToList();
                        if (drivers.Count == 0)
                        {
                            ConsoleHelper.DisplayError("No available drivers nearby. Try again later.");
                        }
                        else
                        {
                            // Calculate fare
                            double distance = locationService.CalculateDistance(pickup, dropoff);
                            decimal fare = (decimal)(distance * 10); // Example: R10 per km
                            ConsoleHelper.DisplayInfo($"Estimated fare: R{fare:F2} for {distance:F1} km");
                            if (passenger.WalletBalance < fare)
                            {
                                ConsoleHelper.DisplayError("Insufficient wallet balance for this ride.");
                            }
                            else
                            {
                                // Assign first available driver
                                var driver = drivers.First();
                                var ride = new Ride
                                {
                                    PassengerId = passenger.Id,
                                    DriverId = driver.Id,
                                    PickupLocation = pickup,
                                    DropOffLocation = dropoff,
                                    Fare = fare,
                                    Status = RideStatus.Requested,
                                    RequestedAt = DateTime.Now,
                                    DistanceKm = distance
                                };
                                rideRepository.AddRide(ride);
                                driver.IsAvailable = false;
                                passenger.WalletBalance -= fare;
                                driver.TotalEarnings += fare;
                                ride.Status = RideStatus.Completed;
                                passenger.RideHistory.Add(ride);
                                driver.CompletedRides.Add(ride);
                                ConsoleHelper.DisplaySuccess($"Ride completed! Driver: {driver.Username}, Fare: R{fare:F2}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.DisplayError($"Ride request failed: {ex.Message}");
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
                    ConsoleHelper.DisplaySuccess($"Added R{amount:F2} to wallet. New balance: R{passenger.WalletBalance:F2}");
                }
                else if (choice == 4)
                {
                    passenger.ViewRideHistory();
                }
                else if (choice == 5)
                {
                    // Prompt for driver to rate
                    var drivers = userRepository.GetAllUsers().OfType<Driver>().ToList();
                    if (drivers.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No drivers available to rate.");
                    }
                    else
                    {
                        ConsoleHelper.DisplayMenu("Select Driver to Rate", drivers.Select(d => d.Username).ToList());
                        int driverChoice = ConsoleHelper.GetMenuChoice(drivers.Count);
                        var driver = drivers[driverChoice - 1];
                        int stars = ConsoleHelper.GetIntInput("Enter rating (1-5)", 1, 5);
                        string comment = ConsoleHelper.GetStringInput("Enter comment (optional)", false);
                        try
                        {
                            ratingService.RateUser(passenger.Id, driver.Id, stars, comment);
                            ConsoleHelper.DisplaySuccess($"Rated driver {driver.Username} with {stars} stars.");
                        }
                        catch (Exception ex)
                        {
                            ConsoleHelper.DisplayError($"Rating failed: {ex.Message}");
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

        static void DriverMenu(Driver driver, Data.UserRepository userRepository, Data.RideRepository rideRepository, Services.RideService rideService)
        {
            while (true)
            {
                var options = new List<string> { "View Available Ride Requests", "Accept a Ride", "Complete a Ride", "View Earnings", "Logout" };
                ConsoleHelper.DisplayMenu($"Driver Menu - {driver.Username}", options);
                int choice = ConsoleHelper.GetMenuChoice(options.Count);
                if (choice == 1)
                {
                    // Show all requested rides assigned to this driver
                    var availableRides = rideRepository.GetAllRides().Where(r => r.Status == RideStatus.Requested && r.DriverId == driver.Id).ToList();
                    if (availableRides.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No ride requests assigned to you.");
                    }
                    else
                    {
                        foreach (var ride in availableRides)
                        {
                            var passenger = userRepository.GetUserById(ride.PassengerId);
                            ConsoleHelper.DisplayInfo($"Ride ID: {ride.Id}, Passenger: {passenger?.Username}, From: {ride.PickupLocation}, To: {ride.DropOffLocation}, Fare: R{ride.Fare:F2}");
                        }
                    }
                }
                else if (choice == 2)
                {
                    // Accept a ride (change status to InProgress)
                    var ridesToAccept = rideRepository.GetAllRides().Where(r => r.Status == RideStatus.Requested && r.DriverId == driver.Id).ToList();
                    if (ridesToAccept.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No rides to accept.");
                    }
                    else
                    {
                        var ride = ridesToAccept.First();
                        ride.Status = RideStatus.InProgress;
                        ride.AcceptedAt = DateTime.Now;
                        ConsoleHelper.DisplaySuccess($"Accepted ride ID: {ride.Id}");
                    }
                }
                else if (choice == 3)
                {
                    // Complete a ride (change status to Completed)
                    var ridesInProgress = rideRepository.GetAllRides().Where(r => r.Status == RideStatus.InProgress && r.DriverId == driver.Id).ToList();
                    if (ridesInProgress.Count == 0)
                    {
                        ConsoleHelper.DisplayInfo("No rides in progress.");
                    }
                    else
                    {
                        var ride = ridesInProgress.First();
                        ride.Status = RideStatus.Completed;
                        ride.CompletedAt = DateTime.Now;
                        driver.IsAvailable = true;
                        ConsoleHelper.DisplaySuccess($"Completed ride ID: {ride.Id}");
                    }
                }
                else if (choice == 4)
                {
                    ConsoleHelper.DisplayInfo($"Total Earnings: R{driver.TotalEarnings:F2}");
                }
                else if (choice == 5)
                {
                    break;
                }
                ConsoleHelper.PauseForUser();
            }
        }
    }
}
