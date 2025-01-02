using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelGeneticSolver
{
    private readonly ILogger _logger;
    private readonly int[] _randomSeeds;

    public SquirrelGeneticSolver(ILogger logger, int[] randomSeeds)
    {
        _logger = logger;
        _randomSeeds = randomSeeds;
    }
    
    public void Solve(int generations, 
        SmellWeights startWeights, 
        Action<GeneticAlgorithm> onGenerationComplete,
        Action<GeneticAlgorithm> onSolverComplete)
    {
        FloatingPointChromosome chromosome = new(
            minValue: [-5,-5,-5,-5,-5], 
            maxValue: [5,5,5,5,5], 
            totalBits: [64,64,64,64,64], 
            fractionDigits: [2,2,2,2,2],
            geneValues: [startWeights.Acorn, startWeights.Squirrel, startWeights.Doggo, startWeights.Rabbit, startWeights.Tree]
        );
        Population population = new Population(minSize: 100, maxSize: 250, chromosome);

        IFitness fitness = new SquirrelScorer(_logger, _randomSeeds);
        ISelection selection = new EliteSelection();
        ICrossover crossover = new UniformCrossover();
        IMutation mutation = new FlipBitMutation();
        
        GeneticAlgorithm ga = new(population, fitness, selection, crossover, mutation)
        {
            Termination = new GenerationNumberTermination(generations),
            Reinsertion = new ElitistReinsertion()
        };
        
        ga.GenerationRan += (sender, _) =>
        {
            double best = ga.Fitness.Evaluate(ga.BestChromosome);
            _logger.LogInformation("Generation {Generation} complete with best score of {Best}", ga.GenerationsNumber, best);
            onGenerationComplete((GeneticAlgorithm)sender!);
        };
        ga.TerminationReached += (sender, _) =>
        {
            double best = ga.Fitness.Evaluate(ga.BestChromosome);
            _logger.LogInformation("Solver complete with best score of {Best}", best);
            onSolverComplete((GeneticAlgorithm)sender!);
        };
        
        ga.Start();
    }
}