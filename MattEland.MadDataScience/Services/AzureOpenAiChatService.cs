using System.ClientModel;
using Azure;
using Azure.AI.OpenAI;
using MattEland.MadDataScience.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI.Images;

namespace MattEland.MadDataScience.Services;

public class AzureOpenAiChatService
{
    private readonly ILogger<AzureOpenAiChatService> _logger;
    private readonly AzureOpenAIClient _client;
    private readonly string _modelName;

    public AzureOpenAiChatService(ILogger<AzureOpenAiChatService> logger, IOptionsSnapshot<AzureOpenAiConfig> config)
    {
        _logger = logger;
        
        AzureOpenAiConfig chatConfig = config.Get("Chat");

        Uri endpoint = new(chatConfig.Endpoint);
        
        _logger.LogDebug("Creating Azure OpenAI Chat client with endpoint {Endpoint}", endpoint);
        
        AzureKeyCredential credential = new(chatConfig.Key);
        _client = new AzureOpenAIClient(endpoint, credential);

        _modelName = chatConfig.ModelName;
    }

    public async Task<string> GetTextCompletionsAsync(string prompt)
    {
        _logger.LogDebug("Generating text completions with prompt: {Prompt} using model {Model}", prompt, _modelName);
        
        ChatClient chatClient = _client.GetChatClient(_modelName);

        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(new SystemChatMessage(prompt));
        
        return result.Value?.Content.FirstOrDefault()?.Text ?? "The system did not generate a response";
    }
}