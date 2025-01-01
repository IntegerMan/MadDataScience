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
    
    public void Solve(int generations, Action<GeneticAlgorithm> onGenerationComplete, Action<GeneticAlgorithm> onSolverComplete)
    {
        FloatingPointChromosome chromosome = new(
            minValue: [-5,-5,-5,-5,-5], 
            maxValue: [5,5,5,5,5], 
            totalBits: [64,64,64,64,64], 
            fractionDigits: [2,2,2,2,2],
            geneValues: [0,0,0,0,0]  // TODO: These values should start random, then come from the best prior run
        );
        Population population = new Population(minSize: 50, maxSize: 100, chromosome);

        IFitness fitness = new SquirrelScorer(_logger, _randomSeeds);
        ISelection selection = new EliteSelection();
        ICrossover crossover = new UniformCrossover();
        IMutation mutation = new FlipBitMutation();
        
        GeneticAlgorithm ga = new(population, fitness, selection, crossover, mutation)
        {
            Termination = new GenerationNumberTermination(generations),
        };
        
        ga.GenerationRan += (sender, args) =>
        {
            _logger.LogInformation("Generation {Generation} complete", ga.GenerationsNumber);
            _logger.LogDebug("Sender {Sender} args {Args}", sender, args);
            onGenerationComplete((GeneticAlgorithm)sender!);
        };
        ga.TerminationReached += (sender, args) =>
        {
            _logger.LogInformation("Solver complete");
            _logger.LogDebug("Sender {Sender} args {Args}", sender, args);
            onSolverComplete((GeneticAlgorithm)sender!);
        };
        
        ga.Start();
    }
}