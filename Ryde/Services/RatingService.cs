using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;
using Ryde.Interfaces;


namespace Ryde.Services
{

    // Handles user rating operations
    public class RatingService
    {
        private readonly IUserRepository _userRepository;

        public RatingService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RateUser(int fromUserId, int toUserId, int stars, string comment = "")
        {
            try
            {
                if (stars < 1 || stars > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5 stars");
                }

                var fromUser = _userRepository.GetUserById(fromUserId);
                var toUser = _userRepository.GetUserById(toUserId);

                if (fromUser == null || toUser == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                var rating = new Rating
                {
                    Id = new Random().Next(1000, 9999),
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    Stars = stars,
                    Comment = comment
                };

                toUser.ReceivedRatings.Add(rating);
                _userRepository.UpdateUser(toUser);

                Console.WriteLine($"⭐ Rating submitted: {stars}/5 stars for {toUser.Username}");

                //Check if driver should be flagged

                if (toUser is Driver driver)
                {
                    CheckDriverRating(driver);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error submitting rating: {ex.Message}");
            }
        }


        public void CheckDriverRating(Driver driver)
        { 
            double averageRating = driver.GetAverageRating();
            int ratingCount = driver.ReceivedRatings.Count;

            // Flag drivers with low ratings (below 3.0) and at least 5 ratings
            if (averageRating < 3.0 && ratingCount >= 5)
            {
                Console.WriteLine($"🚨 WARNING: Driver {driver.Username} has been flagged for low ratings ({averageRating:F1}/5.0)");
            }
        }

        public void DisplayUserRatings(int userId)
        {
            try
            { 
                var user = _userRepository.GetUserById(userId);

                if (user == null)
                { 
                    throw new InvalidOperationException("User not found");
                }
                Console.WriteLine($"\n⭐ === RATINGS FOR {user.Username.ToUpper()} ===");
                Console.WriteLine($"Average Rating: {user.GetAverageRating():F1}/5.0 ({user.ReceivedRatings.Count} ratings)");

                if (user.ReceivedRatings.Any())
                {
                    Console.WriteLine("\nRecent Comments:");
                    var recentRatings = user.ReceivedRatings.OrderByDescending(r => r.CreatedAt)
                        .Take(5);

                    foreach (var rating in recentRatings)
                    {
                        Console.WriteLine($"  {rating.Stars}⭐ - {rating.Comment} ({rating.CreatedAt:MM/dd/yyyy})");
                    }
                }
                else
                {
                    Console.WriteLine("No ratings received yet.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error displaying ratings: {ex.Message}");
            }
        }
    }
}
