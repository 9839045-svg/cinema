namespace Domain.Core.Models
{
    /// <summary>Стан квитка: заброньований, викуплений або скасований.</summary>
    public enum TicketStatus
    {
        Booked,
        Purchased,
        Cancelled
    }
}
