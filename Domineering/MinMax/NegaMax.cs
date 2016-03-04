using Domineering.MinMax.Contracts;
using Domineering.MinMax.Transpositions;
using System;

namespace Domineering.MinMax
{
    public class NegaMax
    {
        public const int NEGINF = int.MinValue + 100;
        public const int POSINF = int.MaxValue - 100;

        private readonly Player _maximizingPlayer;
        private readonly TranspositionTable _transpositionTable;

        public NegaMax(Player maximizingPlayer, TranspositionTable table = null)
        {
            _maximizingPlayer = maximizingPlayer;
            _transpositionTable = table;
        }

        public SearchResult Search(IGameState node, Player player, int depth = POSINF, SearchInfo si = null)
        {
            var sinfo = si ?? new SearchInfo()
            {
                Deadline = DateTime.MaxValue
            };

            return Search(node, player, depth, NEGINF, POSINF, sinfo);
        }

        private SearchResult Search(IGameState node, Player player, int depth, int a, int b, SearchInfo si)
        {
            var alphaOrig = a;

            si.NodesCount++;
            si.TotalNodes++;
            si.MaxDepthSearched++;

            if (_transpositionTable != null)
            {
                var ttEntry = _transpositionTable.Lookup(node);
                if (ttEntry != Transposition.Empty && ttEntry.Depth >= depth)
                {
                    switch (ttEntry.Type)
                    {
                        case Transposition.TranspositionType.Exact:
                            return new SearchResult(node, ttEntry.Value,si.TotalNodes);
                        case Transposition.TranspositionType.LowerBound:
                            a = Math.Max(a, ttEntry.Value);
                            break;
                        case Transposition.TranspositionType.UpperBound:
                            b = Math.Min(b, ttEntry.Value);
                            break;
                    }

                    if (a >= b)
                    {
                        return new SearchResult(node, ttEntry.Value, si.TotalNodes);
                    }
                }
            }

            if (depth == 0 || node.IsTerminal)
            {
                // Console.WriteLine(node.ToString());
                return new SearchResult(node, node.GetValue(player), si.TotalNodes);
            }

            var bestValue = int.MinValue;
            IGameState bestNode = null;

            foreach (var child in node.GetMoves(player))
            {
                var negamaxNode = Search(child, OpponentOf(player), depth - 1, -b, -a, si);

                if (negamaxNode.TimedOut) return negamaxNode;

                if (si.NodesCount > si.NodesPerTimeCheck)
                {
                    si.NodesCount = 0;

                    if (DateTime.Now > si.Deadline)
                    {                  
                        return new SearchResult(negamaxNode.GameState, 0, si.TotalNodes, true);
                    }
                }

                var val = -negamaxNode.Value;

                if (val > bestValue)
                {
                    bestValue = val;
                    bestNode = child;
                }

                a = Math.Max(a, val);

                if (a > b)
                {
                    break;
                }
            }

            if (_transpositionTable != null)
            {
                var ttEntry = new Transposition();

                if (bestValue <= alphaOrig)
                {
                    ttEntry.Type = Transposition.TranspositionType.UpperBound;
                }
                else if (bestValue >= b)
                {
                    ttEntry.Type = Transposition.TranspositionType.LowerBound;
                }
                else
                {
                    ttEntry.Type = Transposition.TranspositionType.Exact;
                }

                ttEntry.Depth = depth;

                _transpositionTable.Store(node, ttEntry);
            }

            return new SearchResult(bestNode, bestValue, si.TotalNodes);
        }

        private Player OpponentOf(Player player)
        {
            return player == Player.One ? Player.Two : Player.One;
        }

    }
}
