using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class Doggo : GameObject, IGameActor
{
    public override string Name => "Doggo";
    public override bool Blocks(IGameActor actor) => true;

    public int TurnOrder => 3;
    public WorldPosition GetGameMove(IEnumerable<TilePerceptions> choices, Random random)
    {
        // Find adjacent prey
        List<TilePerceptions> preyPresent = choices.Where(c => c.SmellOfSquirrel >= 1 || c.SmellOfRabbit >= 1).ToList();

        // Stay put if no adjacent prey
        if (preyPresent.Count == 0)
        {
            return Position;
        }
        
        // Move to a random adjacent tile with prey - could be rabbit or squirrel
        return preyPresent.ElementAt(random.Next(preyPresent.Count)).Position;
    }

    public void HandleCollision(GameObject otherObject, GameWorld world)
    {
        if (otherObject is Squirrel or Rabbit)
        {
            world.Remove(otherObject);
            world.Logger?.LogInformation("{Name} caught {squirrel.Name}", Name, otherObject.Name);
        }
    }
}