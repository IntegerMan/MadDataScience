using MattEland.MadDataScience.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace MattEland.MadDataScience.Services;

public class SpeechService(IOptionsSnapshot<AzureAiServicesConfig> options, ILogger<SpeechService> logger)
{
    private readonly AzureAiServicesConfig _options = options.Value;

    public Task SpeakAsync(string text, string? voiceName = null)
    {
        voiceName ??= _options.SpeechVoiceName;
        
        logger.LogInformation("Speaking: {Text} with voice {Voice}", text, voiceName);

        SpeechConfig config = SpeechConfig.FromSubscription(_options.Key, _options.Region);
        config.SpeechSynthesisVoiceName = voiceName;
        
        SpeechSynthesizer synth = new(config);

        return synth.SpeakTextAsync(text);
    }

    public async Task<string?> ListenAsync()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(_options.Key, _options.Region);
        
        SpeechRecognizer recognizer = new(config);

        logger.LogDebug("Listening for speech...");
        SpeechRecognitionResult? result = await recognizer.RecognizeOnceAsync();
        logger.LogDebug("Speech recognition complete");
        
        logger.LogInformation("Recognized: {Text} with status {Status}", result?.Text,result?.Reason);
        
        return result?.Text;
    }
}