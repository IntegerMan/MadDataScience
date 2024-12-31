namespace MattEland.MadDataScience.SquirrelSimulation;

public class Rabbit : GameObject, IGameActor
{
    public override string Name => "Rabbit";
    public override bool Blocks(IGameActor actor) => actor is Squirrel;

    public int TurnOrder => 2;

    public WorldPosition GetGameMove(IEnumerable<TilePerceptions> choices, Random random)
    {
        // Rabbit will wander randomly. Make sure we don't stay in the same spot.
        List<TilePerceptions> otherChoices = choices.Where(c => c.Position != Position).ToList();
        
        if (!otherChoices.Any())
        {
            return Position;
        }
        
        return otherChoices.ElementAt(random.Next(otherChoices.Count)).Position;
    }

    public void HandleCollision(GameObject otherObject, GameWorld world)
    {
        // Rabbits don't do anything special on collision
    }
}