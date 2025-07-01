using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Ryde;

namespace Utils
{
    public static class UserStorageHelper
    {
        private static readonly string DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string UsersFile = Path.Combine(DataDirectory, "users.json");

        static UserStorageHelper()
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
        }

        public static List<User> LoadUsers()
        {
            if (!File.Exists(UsersFile))
                return new List<User>();
            var json = File.ReadAllText(UsersFile);
            return JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions { Converters = { new UserJsonConverter() } }) ?? new List<User>();
        }

        public static void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true, Converters = { new UserJsonConverter() } });
            File.WriteAllText(UsersFile, json);
        }

        public static void AddUser(User user)
        {
            var users = LoadUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public static void UpdateUser(User user)
        {
            var users = LoadUsers();
            var idx = users.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
            {
                users[idx] = user;
                SaveUsers(users);
            }
        }

        public static User GetUserById(int id)
        {
            return LoadUsers().FirstOrDefault(u => u.Id == id);
        }

        public static User GetUserByUsername(string username)
        {
            return LoadUsers().FirstOrDefault(u => u.Username == username);
        }
    }
}
