using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryde.Interfaces
{
    public interface IPayable
    {
        bool ProcessPayment(decimal amount);
        decimal GetBalance();
        void AddFunds(decimal amount);
        void UpdateBalance(decimal amount);
    }
}
