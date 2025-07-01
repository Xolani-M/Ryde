using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Ryde.Interfaces
{
    public interface IRideable
    {
        void RequestRide(string pickupLocation, string dropOffLocation);
        void CancelRide(int rideId);
        void ViewRideHistory();
    }
}
