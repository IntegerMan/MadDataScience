namespace MattEland.MadDataScience.Models;

public class AzureAiServicesConfig
{
    public string SubscriptionKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string SpeechVoiceName { get; set; } = "en-US-AvaMultilingualNeural";
    public string Endpoint { get; set; } = string.Empty;
}