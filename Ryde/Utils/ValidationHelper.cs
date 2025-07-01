using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Utils
{
    /// <summary>
    /// Helper class for data validation
    /// Ensures our data is clean and valid!
    /// </summary>
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Simple email validation using regex
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Remove all non-digit characters
            string cleanedNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

            // Check if it starts with + and has 10-15 digits, or just has 10 digits
            return Regex.IsMatch(cleanedNumber, @"^(\+\d{10,15}|\d{10})$");
        }

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            // Username should be 3-20 characters, alphanumeric and underscores only
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");
        }

        public static bool IsValidLicenseNumber(string licenseNumber)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                return false;

            // South African license format: AA-000-0000
            return Regex.IsMatch(licenseNumber, @"^[A-Z]{2}-\d{3}-\d{4}$");
        }

        public static bool IsValidVehicleInfo(string vehicleInfo)
        {
            if (string.IsNullOrWhiteSpace(vehicleInfo))
                return false;

            // Should contain at least make, model, and license plate
            return vehicleInfo.Length >= 10 && vehicleInfo.Contains(" ");
        }

        public static bool IsValidRating(int rating)
        {
            return rating >= 1 && rating <= 5;
        }

        public static bool IsValidAmount(decimal amount, decimal min = 0, decimal max = 10000)
        {
            return amount >= min && amount <= max;
        }

        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // Remove dangerous characters and trim
            return input.Trim()
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("&", "")
                       .Replace("\"", "")
                       .Replace("'", "");
        }

        public static bool IsValidLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return false;

            // Basic location validation - should be at least 2 characters
            return location.Trim().Length >= 2;
        }
    }
}
