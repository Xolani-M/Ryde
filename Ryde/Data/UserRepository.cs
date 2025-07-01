using System;
using System.Collections.Generic;
using System.Linq;
using Ryde;
using Ryde.Interfaces;

namespace Ryde.Data
{
   
    /// In-memory storage for use
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;
        private int _nextId;

        public UserRepository()
        {
            _users = new List<User>();
            _nextId = 1;
            SeedTestData(); // Add some sample data for testing
        }

        public void AddUser(User user)
        {
            try
            {
                user.Id = _nextId++;
                _users.Add(user);
                Console.WriteLine($"✅ User {user.Username} added successfully (ID: {user.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding user: {ex.Message}");
                throw;
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                //Find user by ID
                return _users.FirstOrDefault(u => u.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting user by ID {id}: {ex.Message}");
                return null;
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                // LINQ: Find user by username (case-insensitive)
                return _users.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting user by username {username}: {ex.Message}");
                return null;
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                // Return a copy to prevent external modification
                return new List<User>(_users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting all users: {ex.Message}");
                return new List<User>();
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                // LINQ: Find existing user
                var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {user.Id} not found");
                }

                // Update the user in the list
                var index = _users.IndexOf(existingUser);
                _users[index] = user;

                Console.WriteLine($"✅ User {user.Username} updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating user: {ex.Message}");
                throw;
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                // LINQ: Find and remove user
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {id} not found");
                    return false;
                }

                _users.Remove(user);
                Console.WriteLine($"✅ User {user.Username} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all drivers using LINQ
        /// </summary>
        public List<Driver> GetAllDrivers()
        {
            return _users.OfType<Driver>().ToList();
        }

        /// <summary>
        /// Get all passengers using LINQ
        /// </summary>
        public List<Passenger> GetAllPassengers()
        {
            return _users.OfType<Passenger>().ToList();
        }

        /// <summary>
        /// Get available drivers using LINQ
        /// </summary>
        public List<Driver> GetAvailableDrivers()
        {
            return _users.OfType<Driver>()
                        .Where(d => d.IsAvailable && d.IsActive)
                        .ToList();
        }

        /// <summary>
        /// Find users by email domain using LINQ
        /// </summary>
        public List<User> GetUsersByEmailDomain(string domain)
        {
            return _users.Where(u => u.Email.EndsWith($"@{domain}"))
                        .ToList();
        }

        /// <summary>
        /// Get top-rated drivers using LINQ
        /// </summary>
        public List<Driver> GetTopRatedDrivers(int count = 5)
        {
            return _users.OfType<Driver>()
                        .Where(d => d.ReceivedRatings.Any()) // Only drivers with ratings
                        .OrderByDescending(d => d.GetAverageRating())
                        .ThenByDescending(d => d.ReceivedRatings.Count)
                        .Take(count)
                        .ToList();
        }

        private void SeedTestData()
        {
            // Create some sample drivers
            var driver1 = new Driver
            {
                Username = "john_driver",
                Email = "john@ryde.com",
                PhoneNumber = "+27-11-123-4567",
                LicenseNumber = "GP-001-2023",
                VehicleInfo = "Toyota Corolla - White (CA 123 GP)",
                CurrentLocation = new Location(-26.2041, 28.0473), // Downtown
                IsAvailable = true
            };

            var driver2 = new Driver
            {
                Username = "sarah_wheels",
                Email = "sarah@ryde.com",
                PhoneNumber = "+27-11-234-5678",
                LicenseNumber = "GP-002-2023",
                VehicleInfo = "Honda Civic - Blue (CA 456 GP)",
                CurrentLocation = new Location(-26.1076, 28.0567), // Sandton
                IsAvailable = true
            };

            var driver3 = new Driver
            {
                Username = "mike_transport",
                Email = "mike@ryde.com",
                PhoneNumber = "+27-11-345-6789",
                LicenseNumber = "GP-003-2023",
                VehicleInfo = "Volkswagen Polo - Silver (CA 789 GP)",
                CurrentLocation = new Location(-26.1483, 28.0436), // Rosebank
                IsAvailable = false // Currently busy
            };

            // Create some sample passengers
            var passenger1 = new Passenger
            {
                Username = "alice_rider",
                Email = "alice@gmail.com",
                PhoneNumber = "+27-82-123-4567",
                WalletBalance = 250.00m
            };

            var passenger2 = new Passenger
            {
                Username = "bob_commuter",
                Email = "bob@yahoo.com",
                PhoneNumber = "+27-83-234-5678",
                WalletBalance = 150.00m
            };

            // Add some ratings to make it realistic
            driver1.ReceivedRatings.Add(new Rating { FromUserId = 100, ToUserId = 1, Stars = 5, Comment = "Excellent driver!" });
            driver1.ReceivedRatings.Add(new Rating { FromUserId = 101, ToUserId = 1, Stars = 4, Comment = "Good service" });
            driver1.ReceivedRatings.Add(new Rating { FromUserId = 102, ToUserId = 1, Stars = 5, Comment = "Very professional" });

            driver2.ReceivedRatings.Add(new Rating { FromUserId = 100, ToUserId = 2, Stars = 4, Comment = "Nice car and friendly" });
            driver2.ReceivedRatings.Add(new Rating { FromUserId = 103, ToUserId = 2, Stars = 5, Comment = "Perfect ride!" });

            driver3.ReceivedRatings.Add(new Rating { FromUserId = 104, ToUserId = 3, Stars = 3, Comment = "Okay driver" });
            driver3.ReceivedRatings.Add(new Rating { FromUserId = 105, ToUserId = 3, Stars = 2, Comment = "Late pickup" });

            // Add users to repository
            AddUser(driver1);
            AddUser(driver2);
            AddUser(driver3);
            AddUser(passenger1);
            AddUser(passenger2);

            Console.WriteLine("🌱 Test data seeded successfully!");
        }

        /// <summary>
        /// Display user statistics using LINQ
        /// </summary>
        public void DisplayUserStatistics()
        {
            try
            {
                var totalUsers = _users.Count;
                var totalDrivers = _users.OfType<Driver>().Count();
                var totalPassengers = _users.OfType<Passenger>().Count();
                var availableDrivers = _users.OfType<Driver>().Count(d => d.IsAvailable);
                var activeUsers = _users.Count(u => u.IsActive);

                Console.WriteLine("\n👥 === USER STATISTICS ===");
                Console.WriteLine($"Total Users: {totalUsers}");
                Console.WriteLine($"Drivers: {totalDrivers} ({availableDrivers} available)");
                Console.WriteLine($"Passengers: {totalPassengers}");
                Console.WriteLine($"Active Users: {activeUsers}");

                if (totalDrivers > 0)
                {
                    var avgDriverRating = _users.OfType<Driver>()
                        .Where(d => d.ReceivedRatings.Any())
                        .Average(d => d.GetAverageRating());

                    Console.WriteLine($"Average Driver Rating: {avgDriverRating:F1}/5.0");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error displaying statistics: {ex.Message}");
            }
        }
    }
}