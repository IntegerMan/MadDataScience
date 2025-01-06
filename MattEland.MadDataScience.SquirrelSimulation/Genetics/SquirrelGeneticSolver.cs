using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelGeneticSolver(ILogger logger)
{
    public void Solve(int generations, 
        SmellWeights startWeights, 
        IBrain rabbitBrain,
        Action<GeneticAlgorithm> onGenerationComplete,
        Action<GeneticAlgorithm> onSolverComplete)
    {
        SquirrelChromosome chromosome = new();
        chromosome.SetWeights(startWeights);
        
        Population population = new Population(minSize: 50, maxSize: 100, chromosome)
        {
            GenerationStrategy = new PerformanceGenerationStrategy(),
        };

        int[] seeds = [RandomizationProvider.Current.GetInt(0, int.MaxValue), 
            RandomizationProvider.Current.GetInt(0, int.MaxValue), 
            RandomizationProvider.Current.GetInt(0, int.MaxValue)];
        
        IFitness fitness = new SquirrelScorer(logger, seeds, rabbitBrain);
        ISelection selection = new EliteSelection();
        ICrossover crossover = new TwoPointCrossover();
        IMutation mutation = new FlipBitMutation();
        
        GeneticAlgorithm ga = new(population, fitness, selection, crossover, mutation)
        {
            Termination = new GenerationNumberTermination(generations),
            Reinsertion = new ElitistReinsertion(),
            MutationProbability = 0.3f
        };
        
        ga.GenerationRan += (sender, _) =>
        {
            double best = ga.Fitness.Evaluate(ga.BestChromosome);
            logger.LogInformation("Generation {Generation} complete with best score of {Best}: {Details}", ga.GenerationsNumber, best, ga.BestChromosome.ToString());
            onGenerationComplete((GeneticAlgorithm)sender!);
        };
        ga.TerminationReached += (sender, _) =>
        {
            double best = ga.Fitness.Evaluate(ga.BestChromosome);
            logger.LogInformation("Solver complete with best score of {Best}: {Details}", best, ga.BestChromosome.ToString());
            onSolverComplete((GeneticAlgorithm)sender!);
        };
        
        ga.Start();
    }
}