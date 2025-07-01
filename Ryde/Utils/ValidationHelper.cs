using System.Text.RegularExpressions;

namespace Utils
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Simple regex for demonstration
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            // Accepts numbers, spaces, dashes, and optional + at start
            return Regex.IsMatch(phone, @"^\+?[0-9\-\s]{7,15}$");
        }
    }
}
