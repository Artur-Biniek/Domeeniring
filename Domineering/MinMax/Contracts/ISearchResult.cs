namespace Domineering.MinMax.Contracts
{
    public interface ISearchResult
    {
        IGameState GameState { get; }
        bool TimedOut { get; }
        int TotalNodesSearched { get; }
        int Value { get; }
    }
}