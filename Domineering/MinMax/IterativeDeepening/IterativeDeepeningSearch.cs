using Domineering.MinMax.Contracts;
using System;

namespace Domineering.MinMax.IterativeDeepening
{
    public static class IterativeDeepeningSearch
    {
        private const long LONG_TIME = 7 * 24 * 3600 * 1000;

        public static IGameState Search(IGameState node, Player currentPlayer, int depth = int.MaxValue, long timeLimitInMs = LONG_TIME)
        {
            var si = new SearchInfo()
            {
                Deadline = DateTime.Now + TimeSpan.FromMilliseconds(timeLimitInMs)
            };

            var start = DateTime.Now;

            var negaMax = new NegaMax(currentPlayer);

            SearchResult best = null;
            var nodes = 0L;
            for (int curDepth = 1; curDepth <= depth; curDepth++)
            {
                SearchResult curBest = negaMax.Search(node, currentPlayer, curDepth);

                if (curBest.TimedOut)
                {
                    Console.WriteLine("Timed out.");
                    break;
                }

                if (curBest.TotalNodesSearched > nodes)
                {
                    nodes = curBest.TotalNodesSearched;
                }
                else
                {
                    break;
                }

                best = curBest;
            }

            Console.WriteLine("In {0} visited {1} nodes. Last attempt {2}.", DateTime.Now - start, nodes, best.TotalNodesSearched);

            return best.GameState;
        }

    }
}
