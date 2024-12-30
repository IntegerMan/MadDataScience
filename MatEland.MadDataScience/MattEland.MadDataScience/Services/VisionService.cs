using System.Drawing;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using MattEland.MadDataScience.Models;
using Microsoft.Extensions.Options;

namespace MattEland.MadDataScience.Services;

public class VisionService
{
    private readonly Uri _endpoint;
    private readonly AzureKeyCredential _credential;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ImageAnalysisClient _client;

    public VisionService(IOptionsSnapshot<AzureAiServicesConfig> config, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        _endpoint = new Uri(config.Value.Endpoint);
        _credential = new AzureKeyCredential(config.Value.SubscriptionKey);
        _client = new ImageAnalysisClient(_endpoint, _credential);
    }

    public async Task<BinaryData> LoadImageDataAsync(string imageSource)
    {
        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageSource);
        Console.WriteLine($"Reading Image Bytes from {filePath}");

        // Get the bytes of the image from the server
        Byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
        return BinaryData.FromBytes(imageBytes);
    }

    public async Task<string?> GetImageCaptionAsync(BinaryData data)
    {
        Console.WriteLine("Analyzing image...");
        Response<ImageAnalysisResult>? result = await _client.AnalyzeAsync(data, VisualFeatures.Caption);

        return result?.Value?.Caption?.Text;
    }

    public async Task<List<ObjectDetectionResult>> DetectObjectsAsync(BinaryData data)
    {
        List<ObjectDetectionResult> results = new();
        Response<ImageAnalysisResult>? result = await _client.AnalyzeAsync(data, VisualFeatures.Objects);

        if (result?.Value?.Objects is null)
        {
            return results;
        }

        foreach (DetectedObject obj in result.Value.Objects.Values)
        {
            Console.WriteLine($"Object: {string.Join(", ", obj.Tags.Select(t => t.Name))} at {obj.BoundingBox.X}, {obj.BoundingBox.Y}");
            results.Add(new ObjectDetectionResult
            {
                Name = obj.Tags.Select(t => t.Name).FirstOrDefault() ?? "Unknown",
                Confidence = obj.Tags.FirstOrDefault()?.Confidence ?? 0,
                BoundingBox = new Rectangle(obj.BoundingBox.X, obj.BoundingBox.Y, obj.BoundingBox.Width, obj.BoundingBox.Height)
            });
        }

        return results;
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