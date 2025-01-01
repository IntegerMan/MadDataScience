using GeneticSharp;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelGeneticSolver
{
    public void Solve()
    {
        FloatingPointChromosome chromosome = new FloatingPointChromosome([-5,-5,-5,-5,-5], [5,5,5,5,5], [1,1,1,1,1], [2,2,2,2,2]);
        Population population = new Population(minSize: 50, maxSize: 100, chromosome);
    }
}