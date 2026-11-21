namespace Domain.Core.Exceptions
{
    /// <summary>Вказаного місця не існує у залі.</summary>
    public class InvalidSeatException : CinemaException
    {
        public InvalidSeatException(int row, int number)
            : base($"У залі немає місця: ряд {row}, місце {number}.")
        {
            Row = row;
            Number = number;
        }

        public int Row { get; }
        public int Number { get; }
    }
}
