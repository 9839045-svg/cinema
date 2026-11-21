namespace Domain.Core.Exceptions
{
    /// <summary>Сутність із заданим ідентифікатором не знайдено.</summary>
    public class EntityNotFoundException : CinemaException
    {
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName} з ідентифікатором {id} не знайдено.")
        {
            EntityName = entityName;
            EntityId = id;
        }

        public string EntityName { get; }
        public Guid EntityId { get; }
    }
}
