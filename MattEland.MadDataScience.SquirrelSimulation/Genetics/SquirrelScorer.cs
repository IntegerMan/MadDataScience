using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelScorer(ILogger logger, int[] randomSeeds) : IFitness
{
    public int PointsPerTurnLeft { get; set; } = -1;
    public int PointsForAcornsOnBoard { get; set; } = -100;
    public int PointsForSquirrelsOnBoard { get; set; } = 100;
    public int PointsForRabbitsOnBoard { get; set; } = 0;
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
            
            GameWorld world = _generator.Generate(brain, randomSeeds[i]);
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
        => new()
        {
            Acorn = (float)chromosome.GetGene(0).Value,
            Squirrel = (float)chromosome.GetGene(1).Value,
            Doggo = (float)chromosome.GetGene(2).Value,
            Rabbit = (float)chromosome.GetGene(3).Value,
            Tree = (float)chromosome.GetGene(4).Value
        };
}