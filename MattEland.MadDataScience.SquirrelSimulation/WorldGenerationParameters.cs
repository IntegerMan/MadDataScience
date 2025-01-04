using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class WorldGenerationParameters
{
    public required IBrain SquirrelBrain { get; set; }
    public int WorldSize { get; set; } = 13;
    public int MaxTurns { get; set; } = 100;
    public bool ProvideLogger { get; set; } = true;
    public Random? Random { get; set; }
    public int NumberOfTrees { get; set; } = 1;
    public int NumberOfDoggos { get; set; } = 1;
    public int NumberOfSquirrels { get; set; } = 1;
    public int NumberOfRabbits { get; set; } = 1;
    public int NumberOfAcorns { get; set; } = 1;
}