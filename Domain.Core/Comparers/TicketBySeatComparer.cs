using Domain.Core.Models;

namespace Domain.Core.Comparers
{
    /// <summary>Порівняння квитків за місцем: спочатку ряд, потім номер місця.</summary>
    public class TicketBySeatComparer : IComparer<Ticket>
    {
        public int Compare(Ticket? x, Ticket? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int byRow = x.Row.CompareTo(y.Row);
            return byRow != 0 ? byRow : x.SeatNumber.CompareTo(y.SeatNumber);
        }
    }
}
