using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchange.Models
{
    internal class Level
    {
        // Using SortedSet instead of queue for faster insertions with O(1)
        // As OrderID is a sequential number the sort will keep order of priority
        public SortedSet<int> OrderIdQueue { get; set; } = new SortedSet<int>();

        public int Volume { get; set; }
    }
}
