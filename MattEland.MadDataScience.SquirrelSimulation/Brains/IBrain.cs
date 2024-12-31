namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public interface IBrain
{
    WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random);
}