using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryde
{
    public enum RideStatus
    {
        Requested,
        Accepted,
        EnRouteToPickup, // Driver is heading to pickup
        ArrivedAtPickup, // Driver has arrived at pickup
        InProgress,      // Passenger picked up, ride in progress
        Completed,
        Cancelled
    }
}
