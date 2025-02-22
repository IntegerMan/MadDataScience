using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public class Gorilla : GameObject, IGameActor
{
    public override string Name => "Gorilla";
    public override bool Blocks(IGameActor actor) => true;

    public int TurnOrder => 3;
    
    public IBrain Brain { get; init; } = new GorillaBrain();

    public void HandleCollision(GameObject otherObject, GameWorld world)
    {
        if (otherObject is Squirrel or Rabbit)
        {
            world.Remove(otherObject);
            world.Logger?.LogInformation("{Name} caught {squirrel.Name}", Name, otherObject.Name);
        }
    }
}