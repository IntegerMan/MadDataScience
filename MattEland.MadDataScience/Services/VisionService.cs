using System.Drawing;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using MattEland.MadDataScience.Models;
using Microsoft.Extensions.Options;

namespace MattEland.MadDataScience.Services;

public class VisionService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ImageAnalysisClient _client;

    public VisionService(IOptionsSnapshot<AzureAiServicesConfig> config, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        var endpoint = new Uri(config.Value.Endpoint);
        var credential = new AzureKeyCredential(config.Value.Key);
        _client = new ImageAnalysisClient(endpoint, credential);
    }

    public BinaryData? LoadImageData(string imageSource)
    {
        try
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageSource);
            Console.WriteLine($"Reading Image Bytes from {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Image not found at {filePath}");
                return null;
            }
            
            // Get the bytes of the image from the server
            byte[] imageBytes = File.ReadAllBytes(filePath);
            return BinaryData.FromBytes(imageBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType().Name} Error loading image: {ex.Message}");
            return null;
        }   
    }

    public async Task<string?> GetImageCaptionAsync(BinaryData data)
    {
        Console.WriteLine("Analyzing image...");
        Response<ImageAnalysisResult>? result = await _client.AnalyzeAsync(data, VisualFeatures.Caption);

        return result?.Value?.Caption?.Text;
    }
    
    public async Task<List<ObjectDetectionResult>> CaptionObjectsAsync(BinaryData data)
    {
        List<ObjectDetectionResult> results = new();
        Response<ImageAnalysisResult>? result = await _client.AnalyzeAsync(data, VisualFeatures.DenseCaptions);

        if (result?.Value?.DenseCaptions is null)
        {
            return results;
        }

        foreach (DenseCaption obj in result.Value.DenseCaptions.Values)
        {
            Console.WriteLine($"Object: {obj.Text} at {obj.BoundingBox.X}, {obj.BoundingBox.Y}");
            results.Add(new ObjectDetectionResult
            {
                Name = obj.Text,
                Confidence = obj.Confidence,
                BoundingBox = new Rectangle(obj.BoundingBox.X, obj.BoundingBox.Y, obj.BoundingBox.Width, obj.BoundingBox.Height)
            });
        }

        return results;
    }
}