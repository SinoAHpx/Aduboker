using Aduboker.Core.Model;
using Aduboker.Core.Services;

var generator = new AudbokerService
{
    Logger = new BasicLogger(),
    OutputDir = new DirectoryInfo(@"C:\Users\ahpx\Desktop\Audboker"),
    TTSService = new TTSService("")
    {
        Config = new TTSConfig
        {
            Rate = 1.1,
            Region = "eastus"
        }
    }
};

var text = await File.ReadAllTextAsync(@"C:\Users\ahpx\Desktop\ll.txt");

await generator.GenerateAsync(text, "如何以p社游戏世界观为基础写一出悲剧?", 500);

class BasicLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:T} {message}");
    }

    public void Log(string message, Exception exception)
    {
        Console.WriteLine($"{DateTime.Now:T} {exception.Message}: {message}");
    }
}