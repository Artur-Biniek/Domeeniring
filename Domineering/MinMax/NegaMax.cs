using System;
using System.Collections.Generic;
using System.Linq;
using Domineering.MinMax.Contracts;
using Domineering.MinMax.Transpositions;

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

        public ISearchResult Search(IGameState node, Player player, int depth, SearchParams sp)
        {
            return Search(node, player, depth, NEGINF, POSINF, new SearchParamsInternal(sp));
        }

        private ISearchResult Search(IGameState node, Player player, int depth, int a, int b, SearchParamsInternal spi)
        {
            var alphaOrig = a;

            spi.NodesCount++;
            spi.TotalNodesSearched++;

            if (_transpositionTable != null)
            {
                var ttEntry = _transpositionTable.Lookup(node);
                if (ttEntry != Transposition.Empty && ttEntry.Depth >= depth)
                {
                    switch (ttEntry.Type)
                    {
                        case Transposition.TranspositionType.Exact:
                            return new SearchResult(node, ttEntry.Value, spi);
                        case Transposition.TranspositionType.LowerBound:
                            a = Math.Max(a, ttEntry.Value);
                            break;
                        case Transposition.TranspositionType.UpperBound:
                            b = Math.Min(b, ttEntry.Value);
                            break;
                    }

                    if (a >= b)
                    {
                        return new SearchResult(node, ttEntry.Value, spi);
                    }
                }
            }

            if (depth == 0 || node.IsTerminal)
            {
                // Console.WriteLine(node.ToString());
                return new SearchResult(node, node.GetValue(player), spi);
            }

            var bestValue = int.MinValue;
            IGameState bestNode = null;

           var moves = new List<IGameState>(node.GetMoves(player));

            if (spi.SP.OrderMoves)
            {
                moves.Sort();               
            }

            foreach (var child in moves)
            {
                var negamaxNode = Search(child, OpponentOf(player), depth - 1, -b, -a, spi);

                if (negamaxNode.TimedOut) return negamaxNode;

                if (spi.NodesCount > spi.SP.NodesPerTimeCheck)
                {
                    spi.NodesCount = 0;

                    if (DateTime.Now > spi.SP.Deadline)
                    {
                        return new SearchResult(negamaxNode.GameState, 0, spi, timedOut: true);
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

            return new SearchResult(bestNode, bestValue, spi);
        }

        private Player OpponentOf(Player player)
        {
            return player == Player.One ? Player.Two : Player.One;
        }

        private class SearchParamsInternal
        {
            public SearchParams SP { get; private set; }

            public SearchParamsInternal(SearchParams sp)
            {
                SP = sp;
            }

            public int NodesCount;

            public int TotalNodesSearched;
        }

        private sealed class SearchResult : ISearchResult
        {
            public SearchResult(IGameState gameState, int value, SearchParamsInternal psi, bool timedOut = false)
            {
                GameState = gameState;
                Value = value;
                TotalNodesSearched = psi.TotalNodesSearched;
                TimedOut = timedOut;
            }

            public IGameState GameState { get; private set; }
            public bool TimedOut { get; private set; }
            public int TotalNodesSearched { get; private set; }
            public int Value { get; private set; }
        }
    }
}
