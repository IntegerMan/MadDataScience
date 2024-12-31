namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class WeightedBrain : IBrain
{
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        // TODO: This will need to take in some tile importances and use those for its decisions
        
        return actor.Position;
    }
}