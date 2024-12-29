using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace MattEland.MadDataScience.Services;

public class MachineLearningService
{
    public void TrainModel()
    {
        // Load data
        DataFrame df = DataFrame.LoadCsv("video_game_reviews.csv");
        Console.WriteLine("Data Frame Loaded with " + df.Rows.Count + " rows");
        
        // Drop unreliable columns and columns not relevant for training
        df.Columns.Remove("Game Title");
        df.Columns.Remove("User Review");
        
        // Split data into train / test splits
        MLContext mlContext = new();
        DataOperationsCatalog.TrainTestData split = mlContext.Data.TrainTestSplit(df, 0.2);
        
        // Train using AutoML
        Console.WriteLine("Training Model...");
        var experiment = mlContext.Auto().CreateRegressionExperiment(maxExperimentTimeInSeconds: 10);
        ExperimentResult<RegressionMetrics>? result = experiment.Execute(split.TrainSet, labelColumnName: "Price");
        
        // Print results
        Console.WriteLine("Best Trainer: " + result.BestRun.TrainerName);
        Console.WriteLine("R Squared: " + result.BestRun.ValidationMetrics.RSquared);
        Console.WriteLine("Mean Absolute Error: " + result.BestRun.ValidationMetrics.MeanAbsoluteError);
        Console.WriteLine("Root Mean Squared Error: " + result.BestRun.ValidationMetrics.RootMeanSquaredError);
        
        // Save model
        Console.WriteLine("Saving Model...");
        mlContext.Model.Save(result.BestRun.Model, split.TrainSet.Schema, "model.zip");
        Console.WriteLine("Model Saved to model.zip");
    }
}