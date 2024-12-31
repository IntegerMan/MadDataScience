namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class RabbitBrain : IBrain
{
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        // Rabbit will wander randomly. Make sure we don't stay in the same spot.
        List<TilePerceptions> otherChoices = choices.Where(c => c.Position != actor.Position).ToList();
        
        if (!otherChoices.Any())
        {
            return actor.Position;
        }
        
        return otherChoices.ElementAt(random.Next(otherChoices.Count)).Position;
    }
}