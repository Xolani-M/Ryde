using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Ryde.Services
{

    // Handles all payment operations
    public class PaymentService
    {
        public bool CanAfford(Passenger passenger, decimal amount)
        { 
             return passenger.GetBalance() >= amount;
        }

        public bool ProccessRidePayment(Passenger passenger, Driver driver, decimal fare)
        {
            try
            {
                //Check if passenger has sufficient funds
                if (!passenger.ProcessPayment(fare))
                { 
                    return false; // Payment failed
                }

                //Credit driver's earnings
                driver.TotalEarnings += fare;

                Console.WriteLine($"💰 Payment processed: R{fare:F2}");
                Console.WriteLine($"   Passenger balance: R{passenger.GetBalance():F2}");
                Console.WriteLine($"   Driver earnings: R{driver.TotalEarnings:F2}");

                return true; // Payment successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Payment error: {ex.Message}");
                return false;
            }
        }

        public void ProcessRefund(Passenger passenger, decimal amount)
        { 
            passenger.AddFunds(amount);
            Console.WriteLine($"💸 Refund processed: R{amount:F2}");
        }
    }
}
