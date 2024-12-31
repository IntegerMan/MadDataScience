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

    public void HandlePlayerMove(GameWorld gameWorld, GameDirection direction)
    {
        var squirrel = gameWorld.Objects.OfType<Squirrel>().FirstOrDefault();
        if (squirrel == null)
        {
            logger.LogWarning("No squirrel found in the world");
            return;
        }

        WorldPosition newPosition = squirrel.Position.Move(direction);
        if (!gameWorld.IsValidPosition(newPosition))
        {
            logger.LogWarning("Invalid move attempted");
        }
        else
        {
            // TODO: Handle collisions
            
            logger.LogDebug("Moving squirrel from {0} to {1}", squirrel.Position, newPosition);
            squirrel.Position = newPosition;
        }
        
        // TODO: Simulate other actors
    }
}