using MattEland.MadDataScience.SquirrelSimulation.GameObjects;

namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class ScaredyCatRabbitBrain : IBrain
{
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        float bestScore = float.MinValue;
        WorldPosition? bestPosition = null;
        
        // Rabbits should avoid tiles next to squirrels always
        foreach (TilePerceptions choice in choices.Where(c => c.SmellOfSquirrel <= 0.4f))
        {
            // Rabbits move largely randomly, but they'll stick closer to other rabbits and try to avoid close squirrels
            float randomBonus = random.Next() * 0.05f;
            float rabbitBonus = choice.SmellOfRabbit;
            float squirrelBonus = -choice.SmellOfSquirrel * 1.5f;
            
            float score = randomBonus + rabbitBonus + squirrelBonus;
            
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = choice.Position;
            }
        }
        
        // If the rabbit was cornered by a squirrel, it may need  to move to the smallest smell of squirrel position
        if (bestPosition is null)
        {
            foreach (TilePerceptions choice in choices)
            {
                float score = -choice.SmellOfSquirrel * 1.5f;
            
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = choice.Position;
                }
            }
        }
        
        return bestPosition ?? actor.Position;
    }
}