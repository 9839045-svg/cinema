namespace Domain.Core.Exceptions
{
    /// <summary>Сеанс не можна видалити, поки на нього є чинні квитки.</summary>
    public class ShowHasActiveTicketsException : CinemaException
    {
        public ShowHasActiveTicketsException(string showTitle, int ticketsCount)
            : base($"Не можна видалити сеанс «{showTitle}»: чинних квитків — {ticketsCount}.")
        {
            TicketsCount = ticketsCount;
        }

        public int TicketsCount { get; }
    }
}
