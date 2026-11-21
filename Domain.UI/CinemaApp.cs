using Domain.Core.Comparers;
using Domain.Core.Exceptions;
using Domain.Core.Models;
using Domain.Core.Services;

namespace Domain.UI
{
    /// <summary>Консольне меню застосування та обробники пунктів меню.</summary>
    public class CinemaApp
    {
        private readonly CinemaService _service;
        private readonly CinemaStorage _storage;

        public CinemaApp(CinemaService service, CinemaStorage storage)
        {
            _service = service;
            _storage = storage;
        }

        public void Run()
        {
            SeedDemoDataIfEmpty();

            while (true)
            {
                PrintMenu();
                int choice = ConsoleInput.ReadInt("Ваш вибір: ", 0, 15);
                if (choice == 0)
                    return;

                Execute(choice);
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("\n========== КІНОТЕАТР: ПРОДАЖ КВИТКІВ ==========");
            Console.WriteLine(" 1. Показати зали");
            Console.WriteLine(" 2. Додати зал");
            Console.WriteLine(" 3. Показати сеанси (із сортуванням)");
            Console.WriteLine(" 4. Додати сеанс");
            Console.WriteLine(" 5. Редагувати сеанс");
            Console.WriteLine(" 6. Видалити сеанс");
            Console.WriteLine(" 7. Пошук і фільтрація");
            Console.WriteLine(" 8. Схема місць у залі");
            Console.WriteLine(" 9. Забронювати місце");
            Console.WriteLine("10. Купити квиток");
            Console.WriteLine("11. Викупити бронь");
            Console.WriteLine("12. Скасувати бронь");
            Console.WriteLine("13. Квитки сеансу");
            Console.WriteLine("14. Виручка");
            Console.WriteLine("15. Зберегти дані зараз");
            Console.WriteLine(" 0. Вихід (дані зберігаються у файли)");
        }

        private void Execute(int choice)
        {
            try
            {
                switch (choice)
                {
                    case 1: ConsolePrinter.PrintHalls(_service.GetHalls()); break;
                    case 2: AddHall(); break;
                    case 3: ShowShows(); break;
                    case 4: AddShow(); break;
                    case 5: EditShow(); break;
                    case 6: RemoveShow(); break;
                    case 7: SearchAndFilter(); break;
                    case 8: ShowSeatMap(); break;
                    case 9: SellSeat(false); break;
                    case 10: SellSeat(true); break;
                    case 11: ChangeBooking(true); break;
                    case 12: ChangeBooking(false); break;
                    case 13: ShowTicketsOfShow(); break;
                    case 14: ShowRevenue(); break;
                    case 15:
                        _storage.Save();
                        ConsolePrinter.PrintSuccess("Дані збережено.");
                        break;
                }
            }
            catch (SeatAlreadyTakenException ex)
            {
                ConsolePrinter.PrintError("Місце недоступне. " + ex.Message);
            }
            catch (ShowAlreadyStartedException ex)
            {
                ConsolePrinter.PrintError("Сеанс уже розпочався. " + ex.Message);
            }
            catch (TicketLimitExceededException ex)
            {
                ConsolePrinter.PrintError("Перевищено ліміт. " + ex.Message);
            }
            catch (CinemaException ex)
            {
                ConsolePrinter.PrintError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ConsolePrinter.PrintError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ConsolePrinter.PrintError(ex.Message);
            }
        }

        // ---------- Вибір об'єктів зі списку ----------

        private MovieShow? ChooseShow()
        {
            List<MovieShow> shows = _service.GetShows();
            if (shows.Count == 0)
            {
                Console.WriteLine("Сеансів ще немає.");
                return null;
            }

            ConsolePrinter.PrintShows(shows, _service);
            int index = ConsoleInput.ReadInt("Номер сеансу (0 — скасувати): ", 0, shows.Count);
            return index == 0 ? null : shows[index - 1];
        }

        private Hall? ChooseHall()
        {
            List<Hall> halls = _service.GetHalls();
            if (halls.Count == 0)
            {
                Console.WriteLine("Залів ще немає. Спочатку додайте зал (пункт 2).");
                return null;
            }

            ConsolePrinter.PrintHalls(halls);
            int index = ConsoleInput.ReadInt("Номер залу (0 — скасувати): ", 0, halls.Count);
            return index == 0 ? null : halls[index - 1];
        }

        // ---------- Зали та сеанси ----------

        private void AddHall()
        {
            string name = ConsoleInput.ReadText("Назва залу: ", 50);
            int rows = ConsoleInput.ReadInt("Кількість рядів (1-30): ", 1, 30);
            int seats = ConsoleInput.ReadInt("Місць у ряду (1-40): ", 1, 40);
            int vipRows = ConsoleInput.ReadInt($"Кількість VIP-рядів (0-{rows}): ", 0, rows);
            decimal multiplier = ConsoleInput.ReadDecimal("Коефіцієнт ціни VIP (1-5): ", 1m, 5m);

            var hall = new Hall
            {
                Name = name,
                Rows = rows,
                SeatsPerRow = seats,
                VipRows = vipRows,
                VipMultiplier = multiplier
            };
            _service.AddHall(hall);
            ConsolePrinter.PrintSuccess($"Зал «{hall.Name}» додано.");
        }

        private void ShowShows()
        {
            Console.WriteLine("Сортувати: 1 — за часом, 2 — за назвою, 3 — за ціною");
            int mode = ConsoleInput.ReadInt("Ваш вибір: ", 1, 3);

            IComparer<MovieShow>? comparer = mode switch
            {
                2 => new ShowByTitleComparer(),
                3 => new ShowByPriceComparer(),
                _ => null
            };

            ConsolePrinter.PrintShows(_service.GetShows(comparer), _service);
        }

        private void AddShow()
        {
            Hall? hall = ChooseHall();
            if (hall == null)
                return;

            string title = ConsoleInput.ReadText("Назва фільму: ", 100);
            DateTime start = ConsoleInput.ReadDateTime("Початок (дд.мм.рррр гг:хх): ");
            int duration = ConsoleInput.ReadInt("Тривалість, хв (1-600): ", 1, 600);
            decimal price = ConsoleInput.ReadDecimal("Базова ціна квитка, грн (1-10000): ", 1m, 10000m);

            var show = new MovieShow
            {
                Title = title,
                HallId = hall.Id,
                StartTime = start,
                DurationMinutes = duration,
                BasePrice = price
            };
            _service.AddShow(show);
            ConsolePrinter.PrintSuccess($"Сеанс «{show.Title}» додано.");
        }

        private void EditShow()
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            Console.WriteLine($"Редагування сеансу «{show.Title}». Введіть нові значення.");
            string title = ConsoleInput.ReadText("Назва фільму: ", 100);
            DateTime start = ConsoleInput.ReadDateTime("Початок (дд.мм.рррр гг:хх): ");
            int duration = ConsoleInput.ReadInt("Тривалість, хв (1-600): ", 1, 600);
            decimal price = ConsoleInput.ReadDecimal("Базова ціна квитка, грн (1-10000): ", 1m, 10000m);

            _service.UpdateShow(show.Id, title, start, duration, price);
            ConsolePrinter.PrintSuccess("Сеанс оновлено. Уже продані квитки зберігають свою ціну.");
        }

        private void RemoveShow()
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            if (!ConsoleInput.ReadYesNo($"Видалити сеанс «{show.Title}»?"))
                return;

            _service.RemoveShow(show.Id);
            ConsolePrinter.PrintSuccess("Сеанс видалено.");
        }

        private void SearchAndFilter()
        {
            Console.WriteLine("1 — сеанси за назвою, 2 — сеанси за датою, 3 — лише майбутні сеанси, 4 — квитки клієнта");
            int mode = ConsoleInput.ReadInt("Ваш вибір: ", 1, 4);

            switch (mode)
            {
                case 1:
                    string title = ConsoleInput.ReadText("Частина назви: ", 100);
                    ConsolePrinter.PrintShows(_service.FindShowsByTitle(title), _service);
                    break;
                case 2:
                    DateTime date = ConsoleInput.ReadDateTime("Дата (дд.мм.рррр 00:00): ");
                    ConsolePrinter.PrintShows(_service.FindShowsByDate(date), _service);
                    break;
                case 3:
                    ConsolePrinter.PrintShows(_service.GetUpcomingShows(), _service);
                    break;
                case 4:
                    string customer = ConsoleInput.ReadText("Ім'я клієнта (або його частина): ", 100);
                    ConsolePrinter.PrintTickets(_service.FindTicketsByCustomer(customer), _service);
                    break;
            }
        }

        // ---------- Місця та квитки ----------

        private void ShowSeatMap()
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            Hall hall = _service.GetHall(show.HallId);
            ConsolePrinter.PrintSeatMap(hall, _service.GetOccupiedSeats(show.Id));
        }

        private void SellSeat(bool purchase)
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            Hall hall = _service.GetHall(show.HallId);
            ConsolePrinter.PrintSeatMap(hall, _service.GetOccupiedSeats(show.Id));

            int row = ConsoleInput.ReadInt($"Ряд (1-{hall.Rows}): ", 1, hall.Rows);
            int number = ConsoleInput.ReadInt($"Місце (1-{hall.SeatsPerRow}): ", 1, hall.SeatsPerRow);

            SeatType category = hall.GetSeatType(row);
            decimal price = hall.CalculatePrice(show.BasePrice, category);
            Console.WriteLine($"Категорія: {ConsolePrinter.CategoryToText(category)}, ціна: {ConsolePrinter.FormatMoney(price)}");

            string customer = ConsoleInput.ReadText("Ім'я клієнта: ", 100);

            Ticket ticket = purchase
                ? _service.PurchaseSeat(show.Id, row, number, customer)
                : _service.BookSeat(show.Id, row, number, customer);

            ConsolePrinter.PrintSuccess(
                $"Готово: ряд {ticket.Row}, місце {ticket.SeatNumber}, {ConsolePrinter.FormatMoney(ticket.Price)} " +
                $"({ConsolePrinter.StatusToText(ticket.Status)}).");
        }

        private void ChangeBooking(bool confirm)
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            List<Ticket> booked = _service.GetTicketsByShow(show.Id)
                .FindAll(x => x.Status == TicketStatus.Booked);
            if (booked.Count == 0)
            {
                Console.WriteLine("Для цього сеансу немає заброньованих квитків.");
                return;
            }

            ConsolePrinter.PrintTickets(booked, _service);
            int index = ConsoleInput.ReadInt("Номер квитка (0 — скасувати): ", 0, booked.Count);
            if (index == 0)
                return;

            Ticket selected = booked[index - 1];
            if (confirm)
            {
                _service.ConfirmBooking(selected.Id);
                ConsolePrinter.PrintSuccess("Бронь викуплено.");
            }
            else
            {
                _service.CancelBooking(selected.Id);
                ConsolePrinter.PrintSuccess("Бронь скасовано, місце вільне.");
            }
        }

        private void ShowTicketsOfShow()
        {
            MovieShow? show = ChooseShow();
            if (show == null)
                return;

            ConsolePrinter.PrintTickets(_service.GetTicketsByShow(show.Id), _service);
        }

        private void ShowRevenue()
        {
            ConsolePrinter.PrintRevenue(_service.GetRevenueReport(), _service.CalculateTotalRevenue());
        }

        // ---------- Демонстраційні дані ----------

        private void SeedDemoDataIfEmpty()
        {
            if (_service.GetHalls().Count > 0)
                return;

            var mainHall = new Hall { Name = "Зал 1", Rows = 8, SeatsPerRow = 12, VipRows = 2, VipMultiplier = 1.5m };
            var vipHall = new Hall { Name = "Зал 2", Rows = 5, SeatsPerRow = 8, VipRows = 5, VipMultiplier = 2m };
            _service.AddHall(mainHall);
            _service.AddHall(vipHall);

            DateTime now = TrimToMinutes(DateTime.Now);
            DateTime tomorrow = DateTime.Today.AddDays(1);

            // Перший сеанс уже розпочався — на ньому можна продемонструвати ShowAlreadyStartedException.
            _service.AddShow(new MovieShow
            {
                Title = "Інтерстеллар", HallId = mainHall.Id, StartTime = now.AddMinutes(-30),
                DurationMinutes = 120, BasePrice = 150m
            });
            _service.AddShow(new MovieShow
            {
                Title = "Матриця", HallId = mainHall.Id, StartTime = now.AddHours(3),
                DurationMinutes = 130, BasePrice = 140m
            });
            _service.AddShow(new MovieShow
            {
                Title = "Форрест Гамп", HallId = vipHall.Id, StartTime = tomorrow.AddHours(18),
                DurationMinutes = 140, BasePrice = 180m
            });

            Console.WriteLine("Створено демонстраційні дані: 2 зали та 3 сеанси.");
        }

        private static DateTime TrimToMinutes(DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerMinute);
        }
    }
}
