using Domineering.MinMax.Contracts;

namespace Domineering.MinMax.Transpositions
{
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

}
