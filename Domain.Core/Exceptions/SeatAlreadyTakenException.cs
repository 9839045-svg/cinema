namespace Domain.Core.Exceptions
{
    /// <summary>Спроба забронювати або купити місце, яке вже продане чи заброньоване.</summary>
    public class SeatAlreadyTakenException : CinemaException
    {
        public SeatAlreadyTakenException(int row, int number)
            : base($"Місце вже продано або заброньовано (ряд {row}, місце {number}).")
        {
            Row = row;
            Number = number;
        }

        public int Row { get; }
        public int Number { get; }
    }
}
