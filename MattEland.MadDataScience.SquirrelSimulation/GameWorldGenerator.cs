using MattEland.MadDataScience.SquirrelSimulation.Brains;
using MattEland.MadDataScience.SquirrelSimulation.GameObjects;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorldGenerator(ILogger logger)
{
    public GameWorld Generate(IBrain squirrelBrain, int? randomSeed = null, int minSize = 9, int maxSize = 21, bool provideLogger = true)
    {
        Random random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

        int worldSize = random.Next(minSize, maxSize + 1);
        
        GameWorld world = new(worldSize, worldSize, maxTurns: 100)
        {
            Logger = provideLogger ? logger : null,
            Random = random
        };
        
        world.AddObject(new Tree
        {
            Position = world.FindOpenPosition()
        });
        world.AddObject(new Doggo
        {
            Position = world.FindOpenPosition()
        });        
        world.AddObject(new Squirrel
        {
            Brain = squirrelBrain,
            Position = world.FindOpenPosition()
        });        
        world.AddObject(new Rabbit
        {
            Position = world.FindOpenPosition()
        });
        world.AddObject(new Acorn
        {
            Position = world.FindOpenPosition()
        });        
        
        return world;
    }
}