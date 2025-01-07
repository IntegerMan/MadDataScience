using MattEland.MadDataScience.SquirrelSimulation.GameObjects;

namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class WeightedBrain : IBrain
{
    public virtual SmellWeights Weights { get; set; } = new();
    
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        float bestScore = float.MinValue;
        WorldPosition? bestPosition = null;

        HashSet<WorldPosition> positionsToAvoid = new();
        foreach (var pos in actor.PastPositions)
        {
            positionsToAvoid.Add(pos);
        }

        // Random order here helps in cases of tie scores since the first with that value will win
        foreach (TilePerceptions choice in choices.Where(c => !positionsToAvoid.Contains(c.Position)).OrderBy(_ => random.Next()))
        {
            float score = GameWorld.CalculateTileAttractiveness(choice, Weights);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = choice.Position;
            }
        }
        
        return bestPosition ?? actor.Position;
    }
}