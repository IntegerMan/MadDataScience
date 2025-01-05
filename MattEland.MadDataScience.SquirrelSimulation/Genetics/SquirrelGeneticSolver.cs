using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelGeneticSolver(ILogger logger, int[] randomSeeds)
{
    public void Solve(int generations, 
        SmellWeights startWeights, 
        IBrain rabbitBrain,
        Action<GeneticAlgorithm> onGenerationComplete,
        Action<GeneticAlgorithm> onSolverComplete)
    {
        SquirrelChromosome chromosome = new();
        chromosome.SetValue(0, startWeights.Acorn);
        chromosome.SetValue(1, startWeights.Squirrel);
        chromosome.SetValue(2, startWeights.Doggo);
        chromosome.SetValue(3, startWeights.Rabbit);
        chromosome.SetValue(4, startWeights.Tree);
        
        Population population = new Population(minSize: 50, maxSize: 100, chromosome)
        {
            GenerationStrategy = new PerformanceGenerationStrategy(),
        };

        IFitness fitness = new SquirrelScorer(logger, randomSeeds, rabbitBrain);
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
            logger.LogInformation("Generation {Generation} complete with best score of {Best}", ga.GenerationsNumber, best);
            onGenerationComplete((GeneticAlgorithm)sender!);
        };
        ga.TerminationReached += (sender, _) =>
        {
            double best = ga.Fitness.Evaluate(ga.BestChromosome);
            logger.LogInformation("Solver complete with best score of {Best}", best);
            onSolverComplete((GeneticAlgorithm)sender!);
        };
        
        ga.Start();
    }
}