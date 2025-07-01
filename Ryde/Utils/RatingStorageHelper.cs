using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Ryde;

namespace Utils
{
    public static class RatingStorageHelper
    {
        private static readonly string DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string RatingsFile = Path.Combine(DataDirectory, "ratings.json");

        static RatingStorageHelper()
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
        }

        public static List<Rating> LoadRatings()
        {
            if (!File.Exists(RatingsFile))
                return new List<Rating>();
            var json = File.ReadAllText(RatingsFile);
            return JsonSerializer.Deserialize<List<Rating>>(json) ?? new List<Rating>();
        }

        public static void SaveRatings(List<Rating> ratings)
        {
            var json = JsonSerializer.Serialize(ratings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(RatingsFile, json);
        }

        public static void AddRating(Rating rating)
        {
            var ratings = LoadRatings();
            ratings.Add(rating);
            SaveRatings(ratings);
        }

        public static List<Rating> GetRatingsForUser(int userId)
        {
            return LoadRatings().Where(r => r.ToUserId == userId || r.FromUserId == userId).ToList();
        }
    }
}
