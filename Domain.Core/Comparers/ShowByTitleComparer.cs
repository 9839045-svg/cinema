using Domain.Core.Models;

namespace Domain.Core.Comparers
{
    /// <summary>Порівняння сеансів за назвою фільму (без урахування регістру).</summary>
    public class ShowByTitleComparer : IComparer<MovieShow>
    {
        public int Compare(MovieShow? x, MovieShow? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}
