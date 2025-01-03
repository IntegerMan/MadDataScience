using MattEland.MadDataScience.SquirrelSimulation;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.Services;

public class SquirrelSimulationService(ILogger<SquirrelSimulationService> logger)
{
    private readonly GameWorldGenerator _generator = new(logger);
    
    public GameWorld BuildTestWorld(IBrain squirrelBrain, Random? random = null) 
        => _generator.Generate(squirrelBrain, random);
}