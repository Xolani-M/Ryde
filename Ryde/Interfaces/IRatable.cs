using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryde.Interfaces
{
    public interface IRatable
    {
        void RateUser(int userId, int rating, string comment = "");
        double GetAverageRating();
        void ViewRatings();
    }
}
