namespace Domain.Core.Models
{
    /// <summary>
    /// Місце у залі (незмінний об'єкт-значення). Створюється залом, окремо не зберігається.
    /// </summary>
    public sealed class Seat
    {
        public Seat(int row, int number, SeatType type)
        {
            Row = row;
            Number = number;
            Type = type;
        }

        public int Row { get; }
        public int Number { get; }
        public SeatType Type { get; }
    }
}
