using System.Text;
using MattEland.MadDataScience.Models;
using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace MattEland.MadDataScience.Services;

public class MachineLearningService (ILogger<MachineLearningService> logger, IWebHostEnvironment webHostEnvironment)
{
    
    public string TrainModel(uint seconds, bool saveModel)
    {
        // Load data
        DataFrame df = LoadDataFrame();

        // Drop title since we don't really need it
        df.Columns.Remove("Title");
        
        // Split data into train / test splits
        MLContext mlContext = new();
        DataOperationsCatalog.TrainTestData split = mlContext.Data.TrainTestSplit(df, 0.2);
        
        // Train using AutoML
        Console.WriteLine("Training Model...");
        var experiment = mlContext.Auto().CreateRegressionExperiment(maxExperimentTimeInSeconds: seconds);
        ExperimentResult<RegressionMetrics>? result = experiment.Execute(split.TrainSet, labelColumnName: "Rating");
        
        // Print results
        StringBuilder sb = new();
        sb.AppendLine($"**Training Complete ({(seconds == 1 ? "1 second" : $"{seconds} seconds")})**");
        sb.AppendLine();
        sb.AppendLine($"- Best Trainer: {result.BestRun.TrainerName}");
        sb.AppendLine($"- R Squared: {result.BestRun.ValidationMetrics.RSquared:F2}");
        sb.AppendLine($"- Mean Absolute Error: {result.BestRun.ValidationMetrics.MeanAbsoluteError:F2}");
        sb.AppendLine($"- Root Mean Squared Error: {result.BestRun.ValidationMetrics.RootMeanSquaredError:F2}");
        
        string message = sb.ToString();
        logger.LogInformation(message);
        
        // Save model
        if (saveModel)
        {
            logger.LogDebug("Saving Model...");
            string modelPath = Path.Combine(webHostEnvironment.WebRootPath, "model.zip");
            mlContext.Model.Save(result.BestRun.Model, split.TrainSet.Schema, modelPath);
            logger.LogDebug("Model Saved to {Path}", modelPath);
        }
        
        return message;
    }

    public DataFrame LoadDataFrame()
    {
        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "Data.csv");
        DataFrame df = DataFrame.LoadCsv(filePath);
        logger.LogDebug("Data Frame Loaded with {Count} rows", df.Rows.Count);
        return df;
    }

    public float PredictGameRating(VideoGame game)
    {
        logger.LogDebug("Predicting rating for {Title}", game.Title);
        
        MLContext mlContext = new();
        ITransformer model = mlContext.Model.Load(Path.Combine(webHostEnvironment.WebRootPath, "model.zip"), out _);
        PredictionEngine<VideoGame, PricePrediction> engine = mlContext.Model.CreatePredictionEngine<VideoGame, PricePrediction>(model);
        
        PricePrediction prediction = engine.Predict(game);
        logger.LogInformation("Predicted rating for {Title}: {Price:C}", game.Title, prediction.Price);
        
        return prediction.Price;
    }
}