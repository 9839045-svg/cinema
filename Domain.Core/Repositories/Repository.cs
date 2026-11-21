using Domain.Core.Interfaces;

namespace Domain.Core.Repositories
{
    /// <summary>
    /// Репозиторій у пам'яті на основі узагальненої колекції List&lt;T&gt;.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (_items.Any(existing => existing.Id == item.Id))
                throw new ArgumentException($"Елемент з ідентифікатором {item.Id} уже існує.");

            _items.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            foreach (T item in items)
                Add(item);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public T? GetById(Guid id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>Повертає копію списку, щоб зовнішній код не змінював колекцію напряму.</summary>
        public List<T> GetAll()
        {
            return new List<T>(_items);
        }

        public List<T> Find(Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match);
            return _items.FindAll(match);
        }
    }
}
