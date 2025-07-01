using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde.Interfaces;

namespace Ryde
{
    public class Passenger : User, IRideable, IPayable
    {
        public decimal WalletBalance { get; set; }
        public List<Ride> RideHistory { get; set; }
        public string PreferredPaymentMethod { get; set; }

        public Passenger() : base()
        {
            WalletBalance = 100.00m; // Starting balance
            RideHistory = new List<Ride>();
            PreferredPaymentMethod = "Wallet";
            RideHistory = new List<Ride>();
        }

        // Implementing IRideable interface
        public void RequestRide(string pickupLocation, string dropOffLocation)
        {
            Console.WriteLine($"{Username} requested a ride from {pickupLocation} to {dropOffLocation}");
        }

        public void CancelRide(int rideId)
        {
            Console.WriteLine($"{Username} cancelled ride #{rideId}");
        }

        public void ViewRideHistory()
        {
            Console.WriteLine($"\n=== Ride History for {Username} ===");
            if (RideHistory.Count == 0)
            {
                Console.WriteLine("No rides taken yet.");
                return;
            }

            foreach (var ride in RideHistory)
            {
                Console.WriteLine($"Ride #{ride.Id}: {ride.PickupLocation} → {ride.DropOffLocation} - ${ride.Fare:F2}");
            }
        }


        // Implementing IPayable interface
        public bool ProcessPayment(decimal amount)
        {
            if (WalletBalance >= amount)
            {
                WalletBalance -= amount;
                Console.WriteLine($"Payment of ${amount:F2} processed. Remaining balance: ${WalletBalance:F2}");
                return true;
            }
            else
            {
                Console.WriteLine($"Insufficient funds. Required: ${amount:F2}, Available: ${WalletBalance:F2}");
                return false;
            }
        }


        public decimal GetBalance()
        {
            return WalletBalance;
        }

        public void AddFunds(decimal amount)
        {
            WalletBalance += amount;
            Console.WriteLine($"${amount:F2} added to wallet. New balance: ${WalletBalance:F2}");
        }

        public void UpdateBalance(decimal amount)
        {
            WalletBalance = amount;
        }

        // Override parent method - this is POLYMORPHISM in action!
        public override void DisplayInfo()
        {
            base.DisplayInfo(); // Call parent method
            Console.WriteLine($"Wallet Balance: ${WalletBalance:F2}");
            Console.WriteLine($"Total Rides: {RideHistory.Count}");
        }


    }

}
