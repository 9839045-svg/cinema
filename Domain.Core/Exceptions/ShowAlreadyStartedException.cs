namespace Domain.Core.Exceptions
{
    /// <summary>Операція неможлива, бо сеанс уже розпочався.</summary>
    public class ShowAlreadyStartedException : CinemaException
    {
        public ShowAlreadyStartedException(string showTitle, DateTime startTime)
            : base($"Сеанс «{showTitle}» уже розпочався о {startTime:dd.MM.yyyy HH:mm}.")
        {
            ShowTitle = showTitle;
            StartTime = startTime;
        }

        public string ShowTitle { get; }
        public DateTime StartTime { get; }
    }
}
