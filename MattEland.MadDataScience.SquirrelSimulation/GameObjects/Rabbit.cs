using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public class Rabbit : GameObject, IGameActor
{
    public override string Name => "Rabbit";
    public override bool Blocks(IGameActor actor) 
        => actor is Squirrel or Rabbit;

    public int TurnOrder => 2;
    public required IBrain Brain { get; init; }

    public void HandleCollision(GameObject otherObject, GameWorld world)
    {
        // Rabbits don't do anything special on collision
    }
}