using MattEland.MadDataScience.SquirrelSimulation;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    public GameWorld BuildTestWorld(bool assumeDirectControl)
    {
        GameWorld world = new(11, 11, 50)
        {
            Logger = logger,
            UseManualSquirrelControl = assumeDirectControl
        };

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

    public void HandlePlayerMove(GameWorld gameWorld, GameDirection direction)
    {
        var squirrel = gameWorld.Objects.OfType<Squirrel>().FirstOrDefault();
        if (squirrel == null)
        {
            logger.LogWarning("No squirrel found in the world");
            return;
        }

        WorldPosition newPosition = squirrel.Position.Move(direction);
        gameWorld.HandleSquirrelMove(squirrel, newPosition);
    }
}