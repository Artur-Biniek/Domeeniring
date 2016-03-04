using Domineering.MinMax.Contracts;
using System;
using System.Diagnostics;

namespace Domineering.MinMax.IterativeDeepening
{
    public static class IterativeDeepeningSearch
    {
        public static ISearchResult Search(IGameState node, Player currentPlayer, int depth = int.MaxValue, SearchParams? sp = null)
        {
            var searchParams = sp ?? SearchParams.Default;

            var start = DateTime.Now;
            var nodes = 0;

            var negaMax = new NegaMax(currentPlayer);

            ISearchResult lastResult = null;

            for (int curDepth = 1; curDepth <= depth; curDepth++)
            {
                ISearchResult currentResult = negaMax.Search(node, currentPlayer, curDepth, searchParams);

                if (currentResult.TimedOut)
                {
                    Debug.WriteLine("Timed out.");
                    break;
                }

                if (currentResult.TotalNodesSearched > nodes)
                {
                    nodes = currentResult.TotalNodesSearched;
                }
                else
                {
                    break;
                }

                lastResult = currentResult;
            }

            Debug.WriteLine(string.Format("In {0} visited {1} nodes. Last attempt {2}.", DateTime.Now - start, lastResult.TotalNodesSearched, lastResult.TotalNodesSearched));


            return lastResult;
        }

    }
}
