using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchange.Helpers
{
    public class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _comparer;
        public ReverseComparer() : this(null) { }
        public ReverseComparer(IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
        }
        int IComparer<T>.Compare(T x, T y) { return _comparer.Compare(y, x); }
    }
}
