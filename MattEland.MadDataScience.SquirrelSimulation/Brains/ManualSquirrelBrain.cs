using MattEland.MadDataScience.SquirrelSimulation.GameObjects;

namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class ManualSquirrelBrain : IBrain
{
    public GameDirection RequestedDirection { get; set; } = GameDirection.Wait;

    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        WorldPosition requestedPosition = actor.Position.Move(RequestedDirection);
        
        if (choices.Any(c => c.Position == requestedPosition))
        {
            return requestedPosition;
        }

        return actor.Position;
    }
}