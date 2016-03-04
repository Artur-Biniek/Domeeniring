using Domineering.MinMax.Contracts;

namespace Domineering.MinMax
{
    public class SearchResult
    {
        public SearchResult(IGameState gameState, int value, int totalNodesSearched, bool timedOut = false)
        {
            GameState = gameState;
            Value = value;
            TotalNodesSearched = totalNodesSearched;
            TimedOut = timedOut;
        }

        public IGameState GameState { get; private set; }
        public bool TimedOut { get; private set; }
        public int TotalNodesSearched { get; private set; }
        public int Value { get; private set; }
    }
}
