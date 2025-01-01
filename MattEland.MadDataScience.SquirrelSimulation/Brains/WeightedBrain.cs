namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class WeightedBrain : IBrain
{
    public SmellWeights Weights { get; init; } = new();
    
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        float bestScore = float.MinValue;
        WorldPosition? bestPosition = null;
        
        // Random order here helps in cases of tie scores since the first with that value will win
        foreach (TilePerceptions choice in choices.OrderBy(_ => random.Next()))
        {
            float score = (choice.SmellOfSquirrel * Weights.Squirrel) + 
                          (choice.SmellOfRabbit * Weights.Rabbit) +
                          (choice.SmellOfDoggo * Weights.Doggo) +
                          (choice.SmellOfTree * Weights.Tree) +
                          (choice.SmellOfAcorn * Weights.Acorn);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = choice.Position;
            }
        }
        
        return bestPosition ?? actor.Position;
    }
}