using System.Text.Json.Serialization;

namespace Domain.Core.Models
{
    /// <summary>
    /// Кіносеанс: фільм, зал, час початку, тривалість і базова ціна квитка.
    /// За замовчуванням сеанси впорядковуються за часом початку (IComparable).
    /// </summary>
    public class MovieShow : EntityBase, IComparable<MovieShow>
    {
        private string _title = "Фільм";
        private Guid _hallId;
        private int _durationMinutes = 90;
        private decimal _basePrice = 100m;

        public string Title
        {
            get => _title;
            set => _title = RequireText(value, "Назва фільму", 100);
        }

        public Guid HallId
        {
            get => _hallId;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Зал сеансу не вказано.");
                _hallId = value;
            }
        }

        public DateTime StartTime { get; set; }

        public int DurationMinutes
        {
            get => _durationMinutes;
            set => _durationMinutes = RequireRange(value, 1, 600, "Тривалість (хв)");
        }

        public decimal BasePrice
        {
            get => _basePrice;
            set => _basePrice = RequireRange(value, 1m, 10000m, "Базова ціна");
        }

        [JsonIgnore]
        public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);

        public int CompareTo(MovieShow? other)
        {
            if (other is null) return 1;

            int byTime = StartTime.CompareTo(other.StartTime);
            return byTime != 0
                ? byTime
                : string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}
