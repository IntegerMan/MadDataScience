using MattEland.MadDataScience.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace MattEland.MadDataScience.Services;

public class SpeechService(IOptionsSnapshot<AzureAiServicesConfig> options)
{
    private readonly AzureAiServicesConfig _options = options.Value;

    public Task SpeakAsync(string text, string? voiceName = null)
    {
        voiceName ??= _options.SpeechVoiceName;
        
        Console.WriteLine($"Speaking: {text} as {voiceName}");

        SpeechConfig config = SpeechConfig.FromSubscription(_options.SubscriptionKey, _options.Region);
        config.SpeechSynthesisVoiceName = voiceName;
        
        SpeechSynthesizer synth = new(config);

        return synth.SpeakTextAsync(text);
    }
}