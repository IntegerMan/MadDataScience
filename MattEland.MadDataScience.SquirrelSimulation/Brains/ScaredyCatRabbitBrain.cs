using MattEland.MadDataScience.SquirrelSimulation.GameObjects;

namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class ScaredyCatRabbitBrain : IBrain
{
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        float bestScore = float.MinValue;
        WorldPosition? bestPosition = null;
        
        // Random order here helps in cases of tie scores since the first with that value will win
        foreach (TilePerceptions choice in choices.OrderBy(_ => random.Next()))
        {
            // Rabbits move largely randomly, but they'll stick closer to other rabbits and try to avoid close squirrels
            float randomBonus = random.Next() * 0.1f;
            float rabbitBonus = choice.SmellOfRabbit < 0.3 ? 0.0f : choice.SmellOfRabbit;
            float squirrelBonus = choice.SmellOfSquirrel < 0.5f ? 0.0f : -choice.SmellOfSquirrel * 1.2f;
            
            float score = randomBonus + rabbitBonus + squirrelBonus;
            
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = choice.Position;
            }
        }
        
        return bestPosition ?? actor.Position;
    }
}