namespace Domain.Core.Interfaces
{
    /// <summary>
    /// Контракт сутності, яку можна зберігати в репозиторії: має унікальний ідентифікатор.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
    }
}
