using System;
using Domineering.Game;
using Domineering.MinMax;
using Domineering.MinMax.Contracts;
using Domineering.MinMax.IterativeDeepening;

namespace Domineering
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new GameState(4,5, Player.One);
           
            Console.WriteLine(board);

            var next = board;

            while (!next.IsTerminal)
            {
                var start = DateTime.Now;

               var res = IterativeDeepeningSearch.Search(next, next.CurrentPlayer, sp: new SearchParams(DateTime.Now.AddSeconds(3)));
              // var res = (new NegaMax(next.CurrentPlayer)).Search(next, next.CurrentPlayer, int.MaxValue, SearchParams.Default);

                next = (GameState)res.GameState;

                Console.WriteLine("Player: {0}, Game Over: {1}, Searched nodes {2} in time {3}{4}.", next.CurrentPlayer, next.IsTerminal, res.TotalNodesSearched, DateTime.Now - start, res.TimedOut ? " (Timed Out)" : "");
                Console.WriteLine(next);
            }

            
        }
    }
}
