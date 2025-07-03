using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Ryde;

namespace Utils
{
    public static class RideStorageHelper
    {
        private static readonly string DataDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data"));
        private static readonly string RidesFile = Path.Combine(DataDirectory, "rides.json");

        static RideStorageHelper()
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
        }

        public static List<Ride> LoadRides()
        {
            if (!File.Exists(RidesFile))
                return new List<Ride>();
            var json = File.ReadAllText(RidesFile);
            return JsonSerializer.Deserialize<List<Ride>>(json) ?? new List<Ride>();
        }

        public static void SaveRides(List<Ride> rides)
        {
            var json = JsonSerializer.Serialize(rides, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(RidesFile, json);
        }
    }
}
