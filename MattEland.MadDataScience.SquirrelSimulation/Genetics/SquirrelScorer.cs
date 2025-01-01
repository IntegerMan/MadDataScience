using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelScorer : IFitness
{
    public int PointsPerTurnLeft { get; set; }
    public int PointsForAcornsOnBoard { get; set; }
    public int PointsForSquirrelsOnBoard { get; set; }
    public int PointsForRabbitsOnBoard { get; set; }
    public int PointsForWinningSquirrels { get; set; }
    
    public double Evaluate(IChromosome chromosome)
    {
        SmellWeights weights = GetWeightsFromChromosome(chromosome);
        
        throw new NotImplementedException();
    }

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