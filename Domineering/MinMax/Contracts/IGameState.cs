using System.Collections.Generic;

namespace Domineering.MinMax.Contracts
{
    public interface IGameState
    {
        bool IsTerminal { get; }

        IEnumerable<IGameState> GetMoves(Player p);

        int GetValue(Player p);

        Player CurrentPlayer { get; }

        Player Opponent { get; }
    }
}
