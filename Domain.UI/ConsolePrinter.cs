using System.Globalization;
using Domain.Core.Models;
using Domain.Core.Services;

namespace Domain.UI
{
    /// <summary>Форматований вивід даних у консоль.</summary>
    public static class ConsolePrinter
    {
        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Помилка] {message}");
            Console.ResetColor();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static string FormatDate(DateTime value)
        {
            return value.ToString(ConsoleInput.DateTimeFormat, CultureInfo.InvariantCulture);
        }

        public static string FormatMoney(decimal value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture) + " грн";
        }

        public static string CategoryToText(SeatType type)
        {
            return type == SeatType.Vip ? "VIP" : "звичайне";
        }

        public static string StatusToText(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Booked => "заброньовано",
                TicketStatus.Purchased => "куплено",
                TicketStatus.Cancelled => "скасовано",
                _ => status.ToString()
            };
        }

        public static void PrintHalls(List<Hall> halls)
        {
            Console.WriteLine("\n--- Зали ---");
            if (halls.Count == 0)
            {
                Console.WriteLine("Залів ще немає.");
                return;
            }

            for (int i = 0; i < halls.Count; i++)
            {
                Hall hall = halls[i];
                Console.WriteLine(
                    $"{i + 1,3}. {hall.Name,-14} рядів: {hall.Rows,2}, місць у ряду: {hall.SeatsPerRow,2}, " +
                    $"усього: {hall.TotalSeats,3}, VIP-рядів: {hall.VipRows}, коефіцієнт VIP: {hall.VipMultiplier}");
            }
        }

        public static void PrintShows(List<MovieShow> shows, CinemaService service)
        {
            Console.WriteLine("\n--- Сеанси ---");
            if (shows.Count == 0)
            {
                Console.WriteLine("Сеансів не знайдено.");
                return;
            }

            for (int i = 0; i < shows.Count; i++)
            {
                MovieShow show = shows[i];
                Hall hall = service.GetHall(show.HallId);
                int free = service.GetFreeSeatsCount(show.Id);

                Console.WriteLine(
                    $"{i + 1,3}. {show.Title,-26} {hall.Name,-12} {FormatDate(show.StartTime)}  " +
                    $"{show.DurationMinutes,3} хв  {FormatMoney(show.BasePrice),11}  вільно: {free}/{hall.TotalSeats}");
            }
        }

        public static void PrintSeatMap(Hall hall, Dictionary<(int Row, int Number), Ticket> occupied)
        {
            Console.WriteLine();
            Console.WriteLine("        ============ ЕКРАН ============");
            Console.Write("       ");
            for (int number = 1; number <= hall.SeatsPerRow; number++)
                Console.Write($"{number,3}");
            Console.WriteLine();

            for (int row = 1; row <= hall.Rows; row++)
            {
                Console.Write($"Ряд {row,2} ");
                for (int number = 1; number <= hall.SeatsPerRow; number++)
                {
                    string symbol;
                    if (occupied.TryGetValue((row, number), out Ticket? ticket))
                        symbol = ticket.Status == TicketStatus.Purchased ? "X" : "B";
                    else
                        symbol = hall.GetSeatType(row) == SeatType.Vip ? "V" : "o";

                    Console.Write($"{symbol,3}");
                }
                Console.WriteLine(hall.GetSeatType(row) == SeatType.Vip ? "   VIP" : string.Empty);
            }

            Console.WriteLine("\nПозначення: o — вільне, V — вільне VIP, B — заброньоване, X — продане");
        }

        public static void PrintTickets(List<Ticket> tickets, CinemaService service)
        {
            Console.WriteLine("\n--- Квитки ---");
            if (tickets.Count == 0)
            {
                Console.WriteLine("Квитків не знайдено.");
                return;
            }

            for (int i = 0; i < tickets.Count; i++)
            {
                Ticket ticket = tickets[i];
                string title = service.TryGetShow(ticket.ShowId)?.Title ?? "(сеанс видалено)";

                Console.WriteLine(
                    $"{i + 1,3}. {title,-26} ряд {ticket.Row,2}, місце {ticket.SeatNumber,2} " +
                    $"({CategoryToText(ticket.Category),-8}) {FormatMoney(ticket.Price),11}  " +
                    $"{StatusToText(ticket.Status),-12} {ticket.CustomerName}");
            }
        }

        public static void PrintRevenue(List<ShowRevenueInfo> report, decimal total)
        {
            Console.WriteLine("\n--- Виручка за сеансами ---");
            foreach (ShowRevenueInfo item in report)
            {
                Console.WriteLine(
                    $"{item.Show.Title,-26} {FormatDate(item.Show.StartTime)}  " +
                    $"продано: {item.TicketsSold,3}  броней: {item.TicketsBooked,3}  " +
                    $"виручка: {FormatMoney(item.Revenue),12}");
            }
            Console.WriteLine($"\nЗагальна виручка: {FormatMoney(total)}");
        }
    }
}
