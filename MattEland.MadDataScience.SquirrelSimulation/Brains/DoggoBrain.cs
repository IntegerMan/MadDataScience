namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public class DoggoBrain : IBrain
{
    public WorldPosition GetGameMove(IGameActor actor, IEnumerable<TilePerceptions> choices, Random random)
    {
        // Find adjacent prey
        List<TilePerceptions> preyPresent = choices.Where(c => c.SmellOfSquirrel >= 1 || c.SmellOfRabbit >= 1).ToList();

        // Stay put if no adjacent prey
        if (preyPresent.Count == 0)
        {
            return actor.Position;
        }
        
        // Move to a random adjacent tile with prey - could be rabbit or squirrel
        return preyPresent.ElementAt(random.Next(preyPresent.Count)).Position;
    }
}