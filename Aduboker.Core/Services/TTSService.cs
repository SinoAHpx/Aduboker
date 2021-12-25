using System.Reflection;
using Aduboker.Core.Model;
using Flurl.Http;
using Newtonsoft.Json;

namespace Aduboker.Core.Services;

public class TTSService
{
    public string? SubscriptionKey { get; set; }


    public TTSService(string? subscriptionKey = null)
    {
        SubscriptionKey = subscriptionKey;
    }

    public TTSConfig Config { get; set; } = new()
    {
        Pitch = 0,
        Rate = 1.0,
        Voice = "zh-CN-XiaoxiaoNeural",
        Format = "audio-16khz-32kbitrate-mono-mp3",
        Region = "eastasia"
    };

    public async Task<byte[]> RequestSpeechAsync(string text)
    {
        var body =
            "<speak version=\"1.0\" xml:lang=\"en-US\">" +
                $"<voice name=\"{Config.Voice}\">" +
                    $"<prosody rate=\"{Config.Rate}\" pitch=\"{Config.Pitch}%\">" +
                        $"{text}" +
                    "</prosody>" +
                "</voice>" +
            "</speak>";

        var response = await $"https://{Config.Region}.tts.speech.microsoft.com/cognitiveservices/v1"
            .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
            .WithHeader("Content-Type", "application/ssml+xml")
            .WithHeader("X-Microsoft-OutputFormat", Config.Format)
            .WithHeader("User-Agent", "Audboker")
            .WithTimeout(TimeSpan.FromMinutes(10))
            .PostStringAsync(body)
            .ReceiveBytes();

        return response;
    }
}