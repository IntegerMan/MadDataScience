using MattEland.MadDataScience.SquirrelSimulation;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    public GameWorld BuildTestWorld()
    {
        var world = new GameWorld(11, 11);

        world.AddObject(new Squirrel
        {
            Position = new WorldPosition(3, 6)
        });        
        world.AddObject(new Acorn
        {
            Position = new WorldPosition(9, 8)
        });        
        world.AddObject(new Doggo
        {
            Position = new WorldPosition(7, 7)
        });        
        world.AddObject(new Rabbit
        {
            Position = new WorldPosition(2, 6)
        });
        world.AddObject(new Tree
        {
            Position = new WorldPosition(5, 2)
        });

        return world;
    }
}