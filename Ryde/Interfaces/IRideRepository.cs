using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryde;

namespace Ryde.Interfaces
{
    public interface IRideRepository
    {
        void AddRide(Ride ride);
        Ride GetRideById(int id);
        List<Ride> GetRidesByPassengerId(int passengerId);
        List<Ride> GetRidesByDriverId(int driverId);
        List<Ride> GetAvailableRides();
        void UpdateRide(Ride ride);
    }
}
