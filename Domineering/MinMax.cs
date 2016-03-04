using System;
using System.Collections.Generic;
using System.Linq;

namespace Domineering
{
    public class MinMax
    {
        private const int NEGINF = int.MinValue + 100;
        private const int POSINF = int.MaxValue - 100;

        private const long LONG_TIME = 7 * 24 * 3600 * 1000;

        private readonly Player _maximizingPlayer;
        private readonly TranspositionTable _transpositionTable;

        public MinMax(Player maximizingPlayer, TranspositionTable table = null)
        {
            _maximizingPlayer = maximizingPlayer;
            _transpositionTable = table;
        }

        public IGameState NegaMax(IGameState node, int depth = int.MaxValue, long timeLimitInMs = LONG_TIME)
        {
            var si = new SearchInfo()
            {
                Start = DateTime.Now,
                End = DateTime.Now + TimeSpan.FromMilliseconds(timeLimitInMs)
            };

            IGameState best = null;
            var nodes = 0L;
            for (int curDepth = 1; curDepth <= depth; curDepth++)
            {
                si.MaxDepthSearched = depth;
                SearchResult curBest = null;

                curBest = NegaMax(node, curDepth, _maximizingPlayer,NEGINF, POSINF, si);

                si.TotalNodes += si.NodesCount;

                if (si.TimedOut)
                {
                    Console.WriteLine("Timed out.");
                   break;
                }

                //Console.WriteLine(si.TotalNodes);

                if (si.TotalNodes > nodes)
                {
                    nodes = si.TotalNodes;                  
                }
                else
                {
                    break;
                }

                si.TotalNodes = 0;
                si.NodesCount = 0;

                best = curBest.GameState;
            }

            Console.WriteLine("In {0} visited {1} nodes. Last attempt {2}.", DateTime.Now - si.Start, nodes, si.TotalNodes);

            return best;
        }

        private SearchResult NegaMax(IGameState node, int depth, Player player, int a, int b, SearchInfo si)
        {
            var alphaOrig = a;

            si.NodesCount++;

            if (_transpositionTable != null)
            {
                var ttEntry = _transpositionTable.Lookup(node);
                if (ttEntry != Transposition.Empty && ttEntry.Depth >= depth)
                {
                    switch (ttEntry.Type)
                    {
                        case Transposition.TranspositionType.Exact:
                            {
                                return new SearchResult(node, ttEntry.Value);
                            }
                        case Transposition.TranspositionType.LowerBound:
                            a = Math.Max(a, ttEntry.Value);
                            break;
                        case Transposition.TranspositionType.UpperBound:
                            b = Math.Min(b, ttEntry.Value);
                            break;
                    }

                    if (a >= b)
                    {
                        return new SearchResult(node, ttEntry.Value);
                    }
                }
            }

            if (depth == 0 || node.IsTerminal)
            {
                // Console.WriteLine(node.ToString());
                return new SearchResult(node, node.GetValue(player));
            }

            var bestValue = int.MinValue;
            IGameState bestNode = null;

            foreach (var child in node.GetMoves(player))
            {
                var negamaxNode = NegaMax(child, depth - 1, oponentOf(player), - b, -a, si);

                if (si.TimedOut) return null;
                
                if (si.NodesCount > 2000)
                {
                    si.TotalNodes += si.NodesCount;
                    si.NodesCount = 0;                    

                    if (DateTime.Now > si.End)
                    {
                        si.TimedOut = true;
                        return null;
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

            return new SearchResult(bestNode, bestValue);
        }

        private Player oponentOf(Player player)
        {
            return player == Player.One ? Player.Two : Player.One;
        }

        public class SearchResult
        {
            public SearchResult(IGameState gameState, int value)
            {
                GameState = gameState;
                Value = value;
            }

            public IGameState GameState { get; private set; }
            public int Value { get; private set; }
        }

        public class TranspositionTable
        {
            public virtual Transposition Lookup(IGameState node)
            {
                return Transposition.Empty;
            }

            public virtual void Store(IGameState node, Transposition transposition)
            {

            }
        }

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

        public interface IGameState
        {
            bool IsTerminal { get; }

            IEnumerable<IGameState> GetMoves(Player p);

            int GetValue(Player p);

            Player CurrentPlayer { get; }

            Player Opponent { get; }
        }

        public enum Player
        {
            One,
            Two
        }

        private class SearchInfo
        {
            public long NodesCount;
            public long TotalNodes;

            public DateTime Start;
            public DateTime End;

            public bool TimedOut;

            public IGameState BestNode;

            public long MaxDepthSearched;
        }
    }
}
