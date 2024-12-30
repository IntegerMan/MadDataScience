using System.ClientModel;
using Azure;
using Azure.AI.OpenAI;
using MattEland.MadDataScience.Models;
using Microsoft.Extensions.Options;
using OpenAI.Images;

namespace MattEland.MadDataScience.Services;

public class AzureOpenAiImageService
{
    private readonly ILogger<AzureOpenAiImageService> _logger;
    private readonly AzureOpenAIClient _client;
    private readonly string _imageModelName;

    public AzureOpenAiImageService(ILogger<AzureOpenAiImageService> logger, IOptionsSnapshot<AzureOpenAiConfig> config)
    {
        _logger = logger;
        
        AzureOpenAiConfig imageConfig = config.Get("Images");

        Uri endpoint = new(imageConfig.Endpoint);
        AzureKeyCredential credential = new(imageConfig.Key);
        _client = new AzureOpenAIClient(endpoint, credential);

        _imageModelName = imageConfig.ModelName;
    }

    public async Task<ImageGenerationResult> GenerateImageAsync(string prompt)
    {
        _logger.LogDebug("Generating image with prompt: {Prompt} using model {Model}", prompt, _imageModelName);
        
        ImageClient imageClient = _client.GetImageClient(_imageModelName);

        ClientResult<GeneratedImage> result = await imageClient.GenerateImageAsync(prompt, new()
        {
            Quality = GeneratedImageQuality.Standard,
            Size = GeneratedImageSize.W1024xH1024,
            ResponseFormat = GeneratedImageFormat.Uri
        });

        return new ImageGenerationResult
        {
            ImageUri = result.Value?.ImageUri,
            Caption = result.Value?.RevisedPrompt
        };
    }
}