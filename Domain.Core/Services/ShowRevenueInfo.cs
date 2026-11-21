using Domain.Core.Models;

namespace Domain.Core.Services
{
    /// <summary>Підсумок по сеансу для звіту про виручку.</summary>
    public sealed class ShowRevenueInfo
    {
        public ShowRevenueInfo(MovieShow show, int ticketsSold, int ticketsBooked, decimal revenue)
        {
            Show = show;
            TicketsSold = ticketsSold;
            TicketsBooked = ticketsBooked;
            Revenue = revenue;
        }

        public MovieShow Show { get; }
        public int TicketsSold { get; }
        public int TicketsBooked { get; }
        public decimal Revenue { get; }
    }
}
