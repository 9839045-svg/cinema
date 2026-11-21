using Domain.Core.Models;

namespace Domain.Core.Comparers
{
    /// <summary>Порівняння сеансів за базовою ціною квитка.</summary>
    public class ShowByPriceComparer : IComparer<MovieShow>
    {
        public int Compare(MovieShow? x, MovieShow? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return x.BasePrice.CompareTo(y.BasePrice);
        }
    }
}
