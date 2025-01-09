using MattEland.MadDataScience.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace MattEland.MadDataScience.Services;

public class SpeechService(IOptionsSnapshot<AzureAiServicesConfig> options, ILogger<SpeechService> logger)
{
    private readonly AzureAiServicesConfig _options = options.Value;
    public string[] DefaultVoices { get; } = 
    [
        "en-US-AvaMultilingualNeural",
        "en-US-AndrewMultilingualNeural",
        "en-US-EmmaMultilingualNeural",
        "en-US-BrianMultilingualNeural",
        "en-US-GuyNeural",
        "en-GB-AlfieNeural",
        "en-GB-MaisieNeural",
        "en-AU-WilliamNeural",
        "en-AU-FreyaNeural",
        "es-MX-JorgeNeural",
        "fr-FR-JosephineNeural",
    ];

    public async Task<IEnumerable<string>> GetSpeechVoiceNamesAsync()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(_options.Key, _options.Region);
        SpeechSynthesizer synth = new(config);

        try
        {
            string[] locales =
            [
                "en-US",
                "en-GB",
                "en-AU",
                "es-MX",
                "fr-FR",
            ];
            
            List<string> voices = new();
            foreach (var locale in locales)
            {
                foreach (var voice in (await synth.GetVoicesAsync(locale: locale)).Voices)
                {
                    voices.Add(voice.ShortName);
                }
            }

            return voices;
        }
        catch
        {
            return DefaultVoices;
        }
    }
    
    public async Task SpeakAsync(string text, string? voiceName = null)
    {
        voiceName ??= _options.SpeechVoiceName;

        logger.LogInformation("Speaking: {Text} with voice {Voice}", text, voiceName);

        SpeechConfig config = SpeechConfig.FromSubscription(_options.Key, _options.Region);
        config.SpeechSynthesisVoiceName = voiceName;

        SpeechSynthesizer synth = new(config);

        // Start speaking doesn't wait for it to complete speaking before returning to the UI
        _ = synth.StartSpeakingTextAsync(text);
        
        await Task.CompletedTask; // keeping this async for consistency in error handling
    }

    public async Task<string?> ListenAsync()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(_options.Key, _options.Region);

        SpeechRecognizer recognizer = new(config);

        logger.LogDebug("Listening for speech...");
        SpeechRecognitionResult? result = await recognizer.RecognizeOnceAsync();
        logger.LogDebug("Speech recognition complete");

        logger.LogInformation("Recognized: {Text} with status {Status}", result?.Text, result?.Reason);

        return result?.Text;
    }
}