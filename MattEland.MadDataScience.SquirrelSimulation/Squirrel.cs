using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class Squirrel : GameObject, IGameActor
{
    public bool HasAcorn { get; set; }
    
    public override string Name => "Squirrel";
    public override bool Blocks(IGameActor actor) => actor is Rabbit;

    public int TurnOrder => 1;
    public WorldPosition GetGameMove(IEnumerable<TilePerceptions> choices, Random random)
    {
        // TODO: Have a brain that requests a specified tile
        return Position;
    }

    public void HandleCollision(GameObject otherObject, GameWorld world)
    {
        if (otherObject is Acorn acorn)
        {
            HasAcorn = true;
            world.Logger?.LogInformation("{Name} picked up an acorn", Name);
            world.Remove(acorn);
        }
        else if (otherObject is Tree && HasAcorn)
        {
            world.State = GameStatus.Won;
            world.Logger?.LogInformation("{Name} won the game", Name);
        }
    }
}