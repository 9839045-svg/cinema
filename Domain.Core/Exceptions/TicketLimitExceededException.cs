namespace Domain.Core.Exceptions
{
    /// <summary>Перевищено ліміт квитків одного клієнта на один сеанс.</summary>
    public class TicketLimitExceededException : CinemaException
    {
        public TicketLimitExceededException(string message, int limit)
            : base($"{message} (максимальний ліміт: {limit})")
        {
            Limit = limit;
        }

        public int Limit { get; }
    }
}
