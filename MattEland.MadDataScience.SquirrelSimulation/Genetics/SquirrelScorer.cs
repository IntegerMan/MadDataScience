using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelScorer(ILogger logger, int[] randomSeeds, IBrain rabbitBrain) : IFitness
{
    public int PointsPerTurnLeft { get; set; } = -1;
    public int PointsForAcornsOnBoard { get; set; } = -100;
    public int PointsForSquirrelsOnBoard { get; set; } = 100;
    public int PointsForRabbitsOnBoard { get; set; }
    public int PointsForWinningSquirrels { get; set; } = 500;

    private readonly GameWorldGenerator _generator = new(logger);

    public double Evaluate(IChromosome chromosome)
    {
        SmellWeights weights = GetWeightsFromChromosome(chromosome);

        WeightedBrain brain = new WeightedBrain
        {
            Weights = weights
        };

        float[] scores = new float[randomSeeds.Length];
        
        for (int i = 0; i < randomSeeds.Length; i++)
        {
            logger.LogTrace("Evaluating chromosome {Chromosome} with seed {Seed} (index {Index})", chromosome, randomSeeds[i], i);
            
            Random random = new Random(randomSeeds[i]);
            GameWorld world = _generator.Generate(new WorldGenerationParameters
            {
                SquirrelBrain = brain,
                RabbitBrain = rabbitBrain,
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
        SquirrelChromosome squirrelChromosome = (SquirrelChromosome)chromosome;
        
        return new SmellWeights
        {
            Acorn = squirrelChromosome.GetValue(0),
            Squirrel = squirrelChromosome.GetValue(1),
            Doggo = squirrelChromosome.GetValue(2),
            Rabbit = squirrelChromosome.GetValue(3),
            Tree = squirrelChromosome.GetValue(4)
        };
    }
}