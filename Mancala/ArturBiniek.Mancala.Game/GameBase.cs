using System;
using System.Collections.Generic;

namespace ArturBiniek.Mancala.Game
{
    public abstract class GameStateBase
    {
        protected const int NEGINF = int.MinValue + 100;
        protected const int POSINF = int.MaxValue - 100;


        protected abstract IEnumerable<GameStateBase> Moves { get; }

        protected abstract bool IsTerminal { get; }

        protected abstract Player CurentPlayer { get; }

        protected abstract int Score { get; }

        protected ScoredMove NegaMax(int depth)
        {
            var moves = NegaMax(depth, NEGINF, POSINF);

            return moves;
        }

        private ScoredMove NegaMax(int depth, int alpha, int beta)
        {
            if (depth == 0 || IsTerminal)
            {                
                return new ScoredMove(Score);
            }

            var bestMove = (GameStateBase)null;
            int bestValue = NEGINF;
            var bestScoredMove = default(ScoredMove);
            int v;

            foreach (var move in OrderMoves(Moves))
            {
                var scm = NegaMax(depth - 1, -beta, -alpha);

                v = -scm.Value;

                if (v > bestValue)
                {
                    bestValue = v;
                    bestMove = move;
                    bestScoredMove = scm;
                }

                alpha = Math.Max(alpha, v);

                if (alpha >= beta) break;
            }

            return new ScoredMove(bestValue, bestMove, bestScoredMove);
        }

        protected abstract IEnumerable<GameStateBase> OrderMoves(IEnumerable<GameStateBase> moves);    

        protected class ScoredMove
        {
            public GameStateBase Move { get; private set; }

            public ScoredMove Next { get; private set; }

            public int Value { get; private set; }

            public ScoredMove(int value, GameStateBase move = null, ScoredMove next = null)
            {
                Value = value;
                Move = move;
                Next = next;
            }
        }

        protected Player NextPlayer(Player player)
        {
            return player == Player.One ? Player.Two : Player.One;
        }

        public enum Player
        {
            One, 
            Two
        }
    }
}
