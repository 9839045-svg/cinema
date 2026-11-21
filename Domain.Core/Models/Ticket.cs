namespace Domain.Core.Models
{
    /// <summary>
    /// Квиток на конкретне місце конкретного сеансу. Ціна фіксується в момент
    /// бронювання/купівлі, тому подальша зміна ціни сеансу на нього не впливає.
    /// </summary>
    public class Ticket : EntityBase
    {
        private int _row = 1;
        private int _seatNumber = 1;
        private decimal _price;
        private string _customerName = "Клієнт";

        public Guid ShowId { get; set; }

        public int Row
        {
            get => _row;
            set => _row = RequireRange(value, 1, 100, "Ряд");
        }

        public int SeatNumber
        {
            get => _seatNumber;
            set => _seatNumber = RequireRange(value, 1, 100, "Місце");
        }

        public SeatType Category { get; set; }

        public decimal Price
        {
            get => _price;
            set => _price = RequireRange(value, 0m, 100000m, "Ціна квитка");
        }

        public string CustomerName
        {
            get => _customerName;
            set => _customerName = RequireText(value, "Ім'я клієнта", 100);
        }

        public TicketStatus Status { get; set; } = TicketStatus.Booked;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
