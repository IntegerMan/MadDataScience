using MattEland.MadDataScience.SquirrelSimulation;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    private readonly GameWorldGenerator _generator = new(logger);
    public IBrain RabbitBrain { get; set; } = new ScaredyCatRabbitBrain();

    public GameWorld BuildTestWorld(IBrain squirrelBrain, int worldSize, Random? random = null)
    {
        return _generator.Generate(new WorldGenerationParameters
        {
            SquirrelBrain = squirrelBrain,
            RabbitBrain = RabbitBrain,
            WorldSize = worldSize,
            Random = random,
        });
    }

    public GameWorld BuildLargeWorld(IBrain squirrelBrain, Random? random = null)
    {
        return _generator.Generate(new WorldGenerationParameters
        {
            SquirrelBrain = squirrelBrain,
            RabbitBrain = RabbitBrain,
            WorldSize = 40,
            Random = random,
            NumberOfAcorns = 5,
            NumberOfGorillas = 5,
            NumberOfRabbits = 5,
            NumberOfSquirrels = 5,
            NumberOfTrees = 5
        });
    }

    public SmellWeights BestWeightsFallback 
        => new()
        {
            Acorn = 10f,
            Squirrel = -8.4f,
            Gorilla = -11.5f,
            Rabbit = -10f,
            Tree = 6.8f
        };
    
    public SmellWeights BestKillerWeightsFallback 
        => new()
        {
            Acorn = 6f,
            Squirrel = -1f,
            Gorilla = -10f,
            Rabbit = 9f,
            Tree = 3.4f
        };
}