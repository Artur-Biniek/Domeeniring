using System;
using System.Collections.Generic;
using Domineering.MinMax.Contracts;

namespace Domineering.MinMax
{
    public class NegaMax
    {
        public const int NEGINF = int.MinValue + 100;
        public const int POSINF = int.MaxValue - 100;

        private readonly Player _maximizingPlayer;

        public NegaMax(Player maximizingPlayer)
        {
            _maximizingPlayer = maximizingPlayer;
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
