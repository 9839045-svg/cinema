namespace Domain.Core.Exceptions
{
    /// <summary>Помилка читання або запису файлів даних.</summary>
    public class DataStorageException : CinemaException
    {
        public DataStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
