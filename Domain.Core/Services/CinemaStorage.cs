using Domain.Core.Exceptions;
using Domain.Core.Interfaces;
using Domain.Core.Models;

namespace Domain.Core.Services
{
    /// <summary>
    /// Зберігає та завантажує всі репозиторії у JSON-файли (halls.json, shows.json, tickets.json).
    /// </summary>
    public class CinemaStorage
    {
        private readonly JsonFileService<Hall> _hallFile;
        private readonly JsonFileService<MovieShow> _showFile;
        private readonly JsonFileService<Ticket> _ticketFile;

        private readonly IRepository<Hall> _halls;
        private readonly IRepository<MovieShow> _shows;
        private readonly IRepository<Ticket> _tickets;

        public CinemaStorage(
            string directory,
            IRepository<Hall> halls,
            IRepository<MovieShow> shows,
            IRepository<Ticket> tickets)
        {
            _halls = halls;
            _shows = shows;
            _tickets = tickets;

            _hallFile = new JsonFileService<Hall>(Path.Combine(directory, "halls.json"));
            _showFile = new JsonFileService<MovieShow>(Path.Combine(directory, "shows.json"));
            _ticketFile = new JsonFileService<Ticket>(Path.Combine(directory, "tickets.json"));
        }

        /// <summary>
        /// Читає всі файли в тимчасові списки й лише після успіху заповнює репозиторії,
        /// щоб помилка в одному файлі не залишила застосування з частково завантаженими даними.
        /// </summary>
        public void Load()
        {
            List<Hall> halls = _hallFile.Load();
            List<MovieShow> shows = _showFile.Load();
            List<Ticket> tickets = _ticketFile.Load();

            try
            {
                _halls.AddRange(halls);
                _shows.AddRange(shows);
                _tickets.AddRange(tickets);
            }
            catch (ArgumentException ex)
            {
                throw new DataStorageException("Дані у файлах суперечливі: " + ex.Message, ex);
            }
        }

        public void Save()
        {
            _hallFile.Save(_halls.GetAll());
            _showFile.Save(_shows.GetAll());
            _ticketFile.Save(_tickets.GetAll());
        }
    }
}
