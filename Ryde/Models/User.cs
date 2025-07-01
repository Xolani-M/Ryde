using System;
using System.Web;

namespace Ryde
{
    // Base class for all users in the system
    public abstract class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<Rating> ReceivedRatings { get; set; }
        public string Password { get; set; }

        // Constructor - runs when we create a new User
        public User()
        {
            ReceivedRatings = new List<Rating>();
            CreatedAt = DateTime.Now;
            IsActive = true;
        }


        // Virtual method - can be overridden by child classes
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"User: {Username}, Email: {Email}, Phone: {PhoneNumber}");
        }


        // Calculate average rating
        public double GetAverageRating()
        {
            if (ReceivedRatings.Count == 0) return 0.0;

            double total = 0;
            foreach (var rating in ReceivedRatings)
            {
                total += rating.Stars;
            }
            return total / ReceivedRatings.Count;
        }


    }
}
