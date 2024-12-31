using MattEland.MadDataScience.SquirrelSimulation;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    public GameWorld BuildTestWorld(IBrain squirrelBrain, int? randomSeed = null)
    {
        Random random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

        int worldSize = random.Next(9, 24);
        
        GameWorld world = new(worldSize, worldSize, maxTurns: 100)
        {
            Logger = logger,
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