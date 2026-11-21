using Domain.Core.Exceptions;
using Domain.Core.Interfaces;
using Domain.Core.Models;

namespace Domain.Core.Services
{
    /// <summary>
    /// Бізнес-логіка кінотеатру: зали, сеанси, бронювання та купівля квитків, виручка.
    /// Не залежить від способу введення/виведення, тож його можна підключити до будь-якого UI.
    /// </summary>
    public class CinemaService
    {
        public const int MaxTicketsPerCustomerPerShow = 6;

        private readonly IRepository<Hall> _halls;
        private readonly IRepository<MovieShow> _shows;
        private readonly IRepository<Ticket> _tickets;
        private readonly Func<DateTime> _clock;

        public CinemaService(
            IRepository<Hall> halls,
            IRepository<MovieShow> shows,
            IRepository<Ticket> tickets,
            Func<DateTime>? clock = null)
        {
            _halls = halls ?? throw new ArgumentNullException(nameof(halls));
            _shows = shows ?? throw new ArgumentNullException(nameof(shows));
            _tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            _clock = clock ?? (() => DateTime.Now);
        }

        // ---------- Зали ----------

        public void AddHall(Hall hall)
        {
            ArgumentNullException.ThrowIfNull(hall);
            hall.Validate();
            _halls.Add(hall);
        }

        public List<Hall> GetHalls()
        {
            return _halls.GetAll();
        }

        public Hall GetHall(Guid id)
        {
            return _halls.GetById(id) ?? throw new EntityNotFoundException("Зал", id);
        }

        // ---------- Сеанси ----------

        public MovieShow GetShow(Guid id)
        {
            return _shows.GetById(id) ?? throw new EntityNotFoundException("Сеанс", id);
        }

        public MovieShow? TryGetShow(Guid id)
        {
            return _shows.GetById(id);
        }

        public void AddShow(MovieShow show)
        {
            ArgumentNullException.ThrowIfNull(show);
            GetHall(show.HallId);
            EnsureHallIsFree(show, null);
            _shows.Add(show);
        }

        public void UpdateShow(
            Guid showId, string title, DateTime startTime, int durationMinutes, decimal basePrice)
        {
            MovieShow show = GetShow(showId);
            EnsureNotStarted(show);

            // Спочатку створюємо копію: її setter-и перевіряють значення до зміни справжнього об'єкта.
            var updated = new MovieShow
            {
                Id = show.Id,
                Title = title,
                HallId = show.HallId,
                StartTime = startTime,
                DurationMinutes = durationMinutes,
                BasePrice = basePrice
            };
            EnsureHallIsFree(updated, show.Id);

            show.Title = updated.Title;
            show.StartTime = updated.StartTime;
            show.DurationMinutes = updated.DurationMinutes;
            show.BasePrice = updated.BasePrice;
        }

        public void RemoveShow(Guid showId)
        {
            MovieShow show = GetShow(showId);
            List<Ticket> showTickets = _tickets.Find(x => x.ShowId == showId);

            int activeCount = showTickets.Count(x => x.Status != TicketStatus.Cancelled);
            if (activeCount > 0)
                throw new ShowHasActiveTicketsException(show.Title, activeCount);

            foreach (Ticket ticket in showTickets)
                _tickets.Remove(ticket);

            _shows.Remove(show);
        }

        /// <summary>Усі сеанси. Без компаратора — за часом початку (IComparable).</summary>
        public List<MovieShow> GetShows(IComparer<MovieShow>? comparer = null)
        {
            List<MovieShow> result = _shows.GetAll();
            result.Sort(comparer ?? Comparer<MovieShow>.Default);
            return result;
        }

        public List<MovieShow> GetUpcomingShows()
        {
            DateTime now = _clock();
            List<MovieShow> result = _shows.Find(x => x.StartTime > now);
            result.Sort();
            return result;
        }

        public List<MovieShow> FindShowsByTitle(string text)
        {
            string query = text.Trim();
            List<MovieShow> result = _shows.Find(
                x => x.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
            result.Sort();
            return result;
        }

        public List<MovieShow> FindShowsByDate(DateTime date)
        {
            List<MovieShow> result = _shows.Find(x => x.StartTime.Date == date.Date);
            result.Sort();
            return result;
        }

        // ---------- Місця ----------

        /// <summary>Зайняті (заброньовані або продані) місця сеансу: ключ — (ряд, місце).</summary>
        public Dictionary<(int Row, int Number), Ticket> GetOccupiedSeats(Guid showId)
        {
            GetShow(showId);

            var occupied = new Dictionary<(int Row, int Number), Ticket>();
            List<Ticket> active = _tickets.Find(
                x => x.ShowId == showId && x.Status != TicketStatus.Cancelled);

            foreach (Ticket ticket in active)
                occupied[(ticket.Row, ticket.SeatNumber)] = ticket;

            return occupied;
        }

        public int GetFreeSeatsCount(Guid showId)
        {
            MovieShow show = GetShow(showId);
            Hall hall = GetHall(show.HallId);
            return hall.TotalSeats - GetOccupiedSeats(showId).Count;
        }

        // ---------- Квитки ----------

        public Ticket GetTicket(Guid id)
        {
            return _tickets.GetById(id) ?? throw new EntityNotFoundException("Квиток", id);
        }

        public Ticket BookSeat(Guid showId, int row, int number, string customerName)
        {
            return CreateTicket(showId, row, number, customerName, TicketStatus.Booked);
        }

        public Ticket PurchaseSeat(Guid showId, int row, int number, string customerName)
        {
            return CreateTicket(showId, row, number, customerName, TicketStatus.Purchased);
        }

        /// <summary>Викуп раніше заброньованого квитка.</summary>
        public Ticket ConfirmBooking(Guid ticketId)
        {
            Ticket ticket = GetTicket(ticketId);
            if (ticket.Status != TicketStatus.Booked)
                throw new InvalidOperationException("Викупити можна лише заброньований квиток.");

            EnsureNotStarted(GetShow(ticket.ShowId));
            ticket.Status = TicketStatus.Purchased;
            return ticket;
        }

        /// <summary>Скасування броні; місце знову стає вільним.</summary>
        public Ticket CancelBooking(Guid ticketId)
        {
            Ticket ticket = GetTicket(ticketId);
            if (ticket.Status != TicketStatus.Booked)
                throw new InvalidOperationException("Скасувати можна лише заброньований квиток.");

            EnsureNotStarted(GetShow(ticket.ShowId));
            ticket.Status = TicketStatus.Cancelled;
            return ticket;
        }

        public List<Ticket> GetTicketsByShow(Guid showId, IComparer<Ticket>? comparer = null)
        {
            GetShow(showId);
            List<Ticket> result = _tickets.Find(x => x.ShowId == showId);
            result.Sort(comparer ?? new Comparers.TicketBySeatComparer());
            return result;
        }

        public List<Ticket> FindTicketsByCustomer(string text)
        {
            string query = text.Trim();
            List<Ticket> result = _tickets.Find(
                x => x.CustomerName.Contains(query, StringComparison.OrdinalIgnoreCase));
            result.Sort(new Comparers.TicketBySeatComparer());
            return result;
        }

        // ---------- Виручка ----------

        /// <summary>Виручка сеансу: сума цін викуплених квитків (броні не враховуються).</summary>
        public decimal CalculateShowRevenue(Guid showId)
        {
            GetShow(showId);
            return _tickets
                .Find(x => x.ShowId == showId && x.Status == TicketStatus.Purchased)
                .Sum(x => x.Price);
        }

        public decimal CalculateTotalRevenue()
        {
            return _tickets.Find(x => x.Status == TicketStatus.Purchased).Sum(x => x.Price);
        }

        public List<ShowRevenueInfo> GetRevenueReport()
        {
            var report = new List<ShowRevenueInfo>();
            foreach (MovieShow show in GetShows())
            {
                List<Ticket> own = _tickets.Find(x => x.ShowId == show.Id);
                int sold = own.Count(x => x.Status == TicketStatus.Purchased);
                int booked = own.Count(x => x.Status == TicketStatus.Booked);
                decimal revenue = own
                    .Where(x => x.Status == TicketStatus.Purchased)
                    .Sum(x => x.Price);

                report.Add(new ShowRevenueInfo(show, sold, booked, revenue));
            }
            return report;
        }

        // ---------- Внутрішні методи ----------

        private Ticket CreateTicket(Guid showId, int row, int number, string customerName, TicketStatus status)
        {
            MovieShow show = GetShow(showId);
            EnsureNotStarted(show);

            Hall hall = GetHall(show.HallId);
            Seat seat = hall.GetSeat(row, number);

            var ticket = new Ticket
            {
                ShowId = showId,
                Row = seat.Row,
                SeatNumber = seat.Number,
                Category = seat.Type,
                Price = hall.CalculatePrice(show.BasePrice, seat.Type),
                CustomerName = customerName,
                Status = status,
                CreatedAt = _clock()
            };

            if (GetOccupiedSeats(showId).ContainsKey((seat.Row, seat.Number)))
                throw new SeatAlreadyTakenException(seat.Row, seat.Number);

            string customer = ticket.CustomerName;
            int customerTickets = _tickets.Find(x =>
                x.ShowId == showId
                && x.Status != TicketStatus.Cancelled
                && string.Equals(x.CustomerName, customer, StringComparison.OrdinalIgnoreCase)).Count;

            if (customerTickets >= MaxTicketsPerCustomerPerShow)
            {
                throw new TicketLimitExceededException(
                    $"Клієнт «{ticket.CustomerName}» уже має максимум квитків на цей сеанс",
                    MaxTicketsPerCustomerPerShow);
            }

            _tickets.Add(ticket);
            return ticket;
        }

        private void EnsureNotStarted(MovieShow show)
        {
            if (_clock() >= show.StartTime)
                throw new ShowAlreadyStartedException(show.Title, show.StartTime);
        }

        private void EnsureHallIsFree(MovieShow candidate, Guid? ignoredShowId)
        {
            List<MovieShow> conflicts = _shows.Find(x =>
                x.HallId == candidate.HallId
                && x.Id != ignoredShowId
                && x.StartTime < candidate.EndTime
                && candidate.StartTime < x.EndTime);

            if (conflicts.Count > 0)
                throw new InvalidOperationException("У цьому залі вже заплановано інший сеанс на цей час.");
        }
    }
}
