using Domain.Core.Models;

namespace Domain.Core.Comparers
{
    /// <summary>Порівняння квитків за ціною.</summary>
    public class TicketByPriceComparer : IComparer<Ticket>
    {
        public int Compare(Ticket? x, Ticket? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return x.Price.CompareTo(y.Price);
        }
    }
}
