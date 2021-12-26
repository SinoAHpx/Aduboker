namespace Aduboker.Core.Model;

public class TTSConfig
{
    public string Voice { get; set; } = "zh-CN-XiaoxiaoNeural";

    public double Rate { get; set; } = 1.0;

    public double Pitch { get; set; }

    public string Format { get; set; } = "audio-16khz-32kbitrate-mono-mp3";

    public string Region { get; set; } = "eastasia";
}