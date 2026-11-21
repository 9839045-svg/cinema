namespace Domain.Core.Interfaces
{
    /// <summary>
    /// Узагальнений репозиторій: елементарні операції над колекцією сутностей.
    /// </summary>
    public interface IRepository<T> where T : class, IEntity
    {
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        bool Remove(T item);
        T? GetById(Guid id);
        List<T> GetAll();
        List<T> Find(Predicate<T> match);
    }
}
