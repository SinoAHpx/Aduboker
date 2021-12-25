using Aduboker.Core.Model;
using Aduboker.Core.Services;

var generator = new AudbokerService
{
    ChapterService = await File.ReadAllTextAsync(@"C:\Users\ahpx\Downloads\信息全知者.txt"),
    Logger = new BasicLogger(),
    OutputDir = new DirectoryInfo(@"C:\Users\ahpx\Desktop\Audboker"),
    TTSService = new TTSService(args[0])
};

await generator.GenerateAsync(new Range(420, 450));

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