using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryde.Interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User GetUserById(int id);
        User GetUserByUsername(string username);
        List<User> GetAllUsers();
        void UpdateUser(User user);
        bool DeleteUser(int id);
    }
}
