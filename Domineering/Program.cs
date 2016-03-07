using System;
using Domineering.Game;
using Domineering.MinMax.Contracts;
using Domineering.MinMax.IterativeDeepening;

namespace Domineering
{
    class Program
    {
        static void Main(string[] args)
        {
            var player = Console.ReadLine() == "v" ? Player.One : Player.Two;

            var board = new bool[8, 8];

            for (int line = 0; line < 8; line++)
            {
                var str = Console.ReadLine().ToCharArray();

                for (int col = 0; col < 8; col++)
                {
                    board[line, col] = str[col] != '-';
                }
            }

            var game = new GameState(8, 8, board, player);


            var res = IterativeDeepeningSearch.Search(game, game.CurrentPlayer, sp: new SearchParams(DateTime.Now.AddSeconds(2.7), true));

            //Console.WriteLine(game);

            //Console.WriteLine(res.GameState);

            var move = ((GameState)res.GameState).LastMove;

            Console.WriteLine("{0} {1}", move.Item1, move.Item2);
        }


    }
}

