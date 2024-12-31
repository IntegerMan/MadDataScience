using MattEland.MadDataScience.SquirrelSimulation;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    public GameWorld BuildTestWorld(IBrain squirrelBrain)
    {
        GameWorld world = new(11, 11, 50)
        {
            Logger = logger
        };

        world.AddObject(new Tree
        {
            Position = new WorldPosition(5, 2)
        });
        world.AddObject(new Doggo
        {
            Position = new WorldPosition(7, 7)
        });        
        world.AddObject(new Squirrel
        {
            Brain = squirrelBrain,
            Position = new WorldPosition(3, 6)
        });        
        world.AddObject(new Rabbit
        {
            Position = new WorldPosition(2, 6)
        });
        world.AddObject(new Acorn
        {
            Position = new WorldPosition(9, 8)
        });        


        return world;
    }
}