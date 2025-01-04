using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelScorer : IFitness
{
    public int PointsPerTurnLeft { get; set; } = -1;
    public int PointsForAcornsOnBoard { get; set; } = -100;
    public int PointsForSquirrelsOnBoard { get; set; } = 100;
    public int PointsForRabbitsOnBoard { get; set; } = 0;
    public int PointsForWinningSquirrels { get; set; } = 500;

    private readonly GameWorldGenerator _generator;
    private readonly ILogger _logger;
    private readonly int[] _randomSeeds;

    public SquirrelScorer(ILogger logger, int[] randomSeeds)
    {
        _logger = logger;
        _randomSeeds = randomSeeds;
        _generator = new GameWorldGenerator(logger);
    }

    public double Evaluate(IChromosome chromosome)
    {
        SmellWeights weights = GetWeightsFromChromosome(chromosome);

        WeightedBrain brain = new WeightedBrain
        {
            Weights = weights
        };

        float[] scores = new float[_randomSeeds.Length];
        
        for (int i = 0; i < _randomSeeds.Length; i++)
        {
            _logger.LogTrace("Evaluating chromosome {Chromosome} with seed {Seed} (index {Index})", chromosome, _randomSeeds[i], i);
            
            Random random = new Random(_randomSeeds[i]);
            GameWorld world = _generator.Generate(new WorldGenerationParameters
            {
                SquirrelBrain = brain,
                Random = random,
                ProvideLogger = false,
                WorldSize = 13
            });
            GameResult result = world.RunToCompletion();
            scores[i] = ScoreGame(result);
        }

        return scores.Average();
    }

    public float ScoreGame(GameResult result) =>
        (result.WinningSquirrels * PointsForWinningSquirrels) +
        (result.TurnsLeft * PointsPerTurnLeft) +
        (result.AcornsOnBoard * PointsForAcornsOnBoard) +
        (result.SquirrelsOnBoard * PointsForSquirrelsOnBoard) +
        (result.RabbitsOnBoard * PointsForRabbitsOnBoard);

    public static SmellWeights GetWeightsFromChromosome(IChromosome chromosome)
    {
        FloatingPointChromosome fpChromosome = (FloatingPointChromosome)chromosome;
        double[] values = fpChromosome.ToFloatingPoints();
        
        return new SmellWeights
        {
            Acorn = (float)values[0],
            Squirrel = (float)values[1],
            Doggo = (float)values[2],
            Rabbit = (float)values[3],
            Tree = (float)values[4]
        };
    }
}