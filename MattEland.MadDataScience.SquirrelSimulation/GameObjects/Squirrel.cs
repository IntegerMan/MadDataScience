using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public class Squirrel: GameObject, IGameActor
{
    public required IBrain Brain { get; init; }

    public bool HasAcorn { get; set; }
    public bool IsInTree { get; set; }
    
    public override string Name => "Squirrel";
    public override bool Blocks(IGameActor actor) 
        => !IsInTree && actor is Rabbit or Squirrel;

    public bool IsActive => !IsInTree;

    public int TurnOrder => 1;

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
            IsInTree = true;
            world.Logger?.LogInformation("{Name} safely got an acorn and got to a tree", Name);
        }
    }
}