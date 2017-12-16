using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class TicketManager
    {
        protected abstract string ReservationUrl { get; }
        protected abstract string BuyUrl { get; }
    }
}
