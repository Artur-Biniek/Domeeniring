using System;

namespace Domineering.MinMax.Contracts
{
    public struct SearchParams
    {
        public static readonly SearchParams Default = new SearchParams(DateTime.MaxValue, false, int.MaxValue);

        public DateTime Deadline { get; private set; }

        public int NodesPerTimeCheck { get; private set; }
        public bool OrderMoves { get; private set; }

        public SearchParams(DateTime deadline, bool orderMoves, int nodesPerTimeCheck = 1024)
        {
            Deadline = deadline;
            OrderMoves = orderMoves;
            NodesPerTimeCheck = nodesPerTimeCheck;
        }
    }
}
