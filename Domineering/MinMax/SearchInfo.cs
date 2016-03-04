using Domineering.MinMax.Contracts;
using System;

namespace Domineering.MinMax
{
    public sealed class SearchInfo
    {
        internal int NodesCount;
        internal int TotalNodes;

        internal int MaxDepthSearched;

        public DateTime Deadline;

        public int NodesPerTimeCheck = 100;
    }
}
