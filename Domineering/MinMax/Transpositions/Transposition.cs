using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domineering.MinMax.Transpositions
{
    public class Transposition
    {
        public static readonly Transposition Empty = new Transposition();

        public int Value { get; set; }

        public int Depth { get; set; }

        public TranspositionType Type { get; set; }

        public enum TranspositionType
        {
            Exact,
            LowerBound,
            UpperBound
        }
    }
}
