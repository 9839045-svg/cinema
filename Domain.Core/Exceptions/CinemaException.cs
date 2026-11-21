namespace Domain.Core.Exceptions
{
    /// <summary>Базовий клас усіх виключень бізнес-логіки кінотеатру.</summary>
    public class CinemaException : Exception
    {
        public CinemaException(string message) : base(message)
        {
        }

        public CinemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
