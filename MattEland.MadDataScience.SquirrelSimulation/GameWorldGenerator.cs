using MattEland.MadDataScience.SquirrelSimulation.GameObjects;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorldGenerator(ILogger logger)
{
    public GameWorld Generate(WorldGenerationParameters parameters)
    {
        Random random = parameters.Random ?? new Random();
        
        GameWorld world = new(parameters.WorldSize, parameters.WorldSize, maxTurns: parameters.MaxTurns)
        {
            Logger = parameters.ProvideLogger ? logger : null,
            Random = random
        };
        
        for (int i = 0; i < parameters.NumberOfTrees; i++)
        {
            world.AddObject(new Tree
            {
                Position = world.FindOpenPosition()
            });
        }
        for (int i = 0; i < parameters.NumberOfDoggos; i++)
        {
            world.AddObject(new Doggo
            {
                Position = world.FindOpenPosition()
            });
        }
        for (int i = 0; i < parameters.NumberOfSquirrels; i++)
        {
            world.AddObject(new Squirrel
            {
                Brain = parameters.SquirrelBrain,
                Position = world.FindOpenPosition()
            });
        }
        for (int i = 0; i < parameters.NumberOfRabbits; i++)
        {
            world.AddObject(new Rabbit
            {
                Position = world.FindOpenPosition(),
                Brain = parameters.RabbitBrain
            });
        }
        for (int i = 0; i < parameters.NumberOfAcorns; i++)
        {
            world.AddObject(new Acorn
            {
                Position = world.FindOpenPosition()
            });
        }    
        
        return world;
    }
}