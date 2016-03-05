using System.Collections.Generic;
using System.Text;
using Domineering.MinMax.Contracts;

namespace Domineering.Game
{
    public class GameState : IGameState
    {
        private static StringBuilder _sb = new StringBuilder();

        bool[,] _board;

        public Player CurrentPlayer { get; private set; }

        public Player Opponent { get { return GetOpponent(CurrentPlayer); } }

        public int Cols { get; private set; }

        public int Rows { get; private set; }

        public IEnumerable<IGameState> GetMoves(Player player)
        {
            int dx = player == Player.One ? 1 : 0;
            int dy = player == Player.One ? 0 : 1;

            for (int r = 0; r < Rows - dx; r++)
            {
                for (int c = 0; c < Cols - dy; c++)
                {
                    if (_board[r, c] == false && _board[r + dx, c + dy] == false)
                    {
                        yield return WithMove(r, c, r + dx, c + dy, GetOpponent(player));
                    }
                }
            }
        }

        public bool IsTerminal
        {
            get { return GetIsTerminal(CurrentPlayer); }
        }

        public GameState(int rows, int cols, Player currentPlayer)
            : this(rows, cols, new bool[rows, cols], currentPlayer)
        { }

        public GameState(int rows, int cols, bool[,] initialState, Player currentPlayer)
        {
            Rows = rows;
            Cols = cols;

            CurrentPlayer = currentPlayer;

            _board = new bool[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    _board[r, c] = initialState[r, c];
                }
            }
        }

        public int GetValue(Player player)
        {
            if (GetIsTerminal(player))
            {
                return -100;
            }

            if (GetIsTerminal(GetOpponent(player)))
            {
                return 100;
            }

            return 0;
        }

        public override string ToString()
        {
            _sb.Clear();

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_board[r, c]) _sb.Append("# ");
                    else _sb.Append("\u00b7 ");
                }
                _sb.Append("\n");
            }

            return _sb.ToString();
        }

        private Player GetOpponent(Player player)
        {
            return player == Player.One ? Player.Two : Player.One;
        }

        private IGameState WithMove(int r1, int c1, int r2, int c2, Player nextPlayer)
        {
            var nextGameState = new GameState(Rows, Cols, _board, nextPlayer);

            nextGameState._board[r1, c1] = true;
            nextGameState._board[r2, c2] = true;

            return nextGameState;
        }

        private bool GetIsTerminal(Player player)
        {
            int dx = player == Player.One ? 1 : 0;
            int dy = player == Player.One ? 0 : 1;

            for (int r = 0; r < Rows - dx; r++)
            {
                for (int c = 0; c < Cols - dy; c++)
                {
                    if (_board[r, c] == false && _board[r + dx, c + dy] == false)
                        return false;
                }
            }

            return true;
        }
    }
}
