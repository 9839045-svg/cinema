using System.Text.Json.Serialization;
using Domain.Core.Exceptions;

namespace Domain.Core.Models
{
    /// <summary>
    /// Кінозал: прямокутна схема місць. Останні VipRows рядів (найближчі до задньої стіни)
    /// вважаються VIP-місцями й коштують дорожче на коефіцієнт VipMultiplier.
    /// </summary>
    public class Hall : EntityBase
    {
        private string _name = "Зал";
        private int _rows = 1;
        private int _seatsPerRow = 1;
        private int _vipRows;
        private decimal _vipMultiplier = 1.5m;

        public string Name
        {
            get => _name;
            set => _name = RequireText(value, "Назва залу", 50);
        }

        public int Rows
        {
            get => _rows;
            set => _rows = RequireRange(value, 1, 30, "Кількість рядів");
        }

        public int SeatsPerRow
        {
            get => _seatsPerRow;
            set => _seatsPerRow = RequireRange(value, 1, 40, "Місць у ряду");
        }

        public int VipRows
        {
            get => _vipRows;
            set => _vipRows = RequireRange(value, 0, 30, "Кількість VIP-рядів");
        }

        public decimal VipMultiplier
        {
            get => _vipMultiplier;
            set => _vipMultiplier = RequireRange(value, 1m, 5m, "Коефіцієнт VIP");
        }

        [JsonIgnore]
        public int TotalSeats => Rows * SeatsPerRow;

        /// <summary>Перевіряє узгодженість параметрів, що залежать одне від одного.</summary>
        public void Validate()
        {
            if (VipRows > Rows)
                throw new ArgumentException("Кількість VIP-рядів не може перевищувати кількість рядів.");
        }

        public SeatType GetSeatType(int row)
        {
            return row > Rows - VipRows ? SeatType.Vip : SeatType.Regular;
        }

        public Seat GetSeat(int row, int number)
        {
            if (row < 1 || row > Rows || number < 1 || number > SeatsPerRow)
                throw new InvalidSeatException(row, number);

            return new Seat(row, number, GetSeatType(row));
        }

        public List<Seat> GetSeats()
        {
            var seats = new List<Seat>();
            for (int row = 1; row <= Rows; row++)
            {
                for (int number = 1; number <= SeatsPerRow; number++)
                    seats.Add(new Seat(row, number, GetSeatType(row)));
            }
            return seats;
        }

        /// <summary>Ціна квитка для місця заданої категорії.</summary>
        public decimal CalculatePrice(decimal basePrice, SeatType type)
        {
            return type == SeatType.Vip ? Math.Round(basePrice * VipMultiplier, 2) : basePrice;
        }
    }
}
