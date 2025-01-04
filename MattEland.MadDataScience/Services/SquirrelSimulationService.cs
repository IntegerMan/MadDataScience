using MattEland.MadDataScience.SquirrelSimulation;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    private readonly GameWorldGenerator _generator = new(logger);
    
    public GameWorld BuildTestWorld(IBrain squirrelBrain, int worldSize, Random? random = null)
    {
        return _generator.Generate(new WorldGenerationParameters()
        {
            SquirrelBrain = squirrelBrain,
            WorldSize = worldSize,
            Random = random,
        });
    }

    public GameWorld BuildLargeWorld(IBrain squirrelBrain, Random? random = null)
    {
        return _generator.Generate(new WorldGenerationParameters()
        {
            SquirrelBrain = squirrelBrain,
            WorldSize = 40,
            Random = random,
            NumberOfAcorns = 5,
            NumberOfDoggos = 5,
            NumberOfRabbits = 5,
            NumberOfSquirrels = 5,
            NumberOfTrees = 5
        });
    }
}