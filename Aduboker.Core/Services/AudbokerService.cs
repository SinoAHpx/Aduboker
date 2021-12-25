using System.Diagnostics;
using System.Text;
using Aduboker.Core.Model;
using Polly;

namespace Aduboker.Core.Services;

public class AudbokerService
{
    public TTSService TTSService { get; set; }

    public ChapterService ChapterService { get; set; }

    public DirectoryInfo OutputDir { get; set; }

    public ILogger Logger { get; set; }

    public async Task GenerateAsync(Range? range = null)
    {
        var chapters = await ChapterService.GetSplitChaptersAsync(range: range);

        foreach (var ((title, batchSize), batches) in chapters)
        {
            await Policy
                .Handle<Exception>()
                .RetryAsync(5, (exception, i) =>
                {
                    Logger.Log($"Request has failed, retrying for {i} time", exception);
                })
                .ExecuteAsync(async () =>
                {
                    var path = Path.Combine(OutputDir.ToString(), $"{title}.mp3");

                    if (File.Exists(path))
                    {
                        Logger.Log($"The chapter {title} is already exist, skip");
                    }
                    else
                    {
                        var watch = new Stopwatch();
                        watch.Start();

                        Logger.Log($"Requesting for {title}...");
                        Logger.Log(
                            $"It has split into {batchSize} batches and {batches.Select(x => x.Capacity).Aggregate((a, b) => a + b)} characters in total");
                        var bytesList = new List<(int, byte[])>();

                        await Parallel.ForEachAsync(batches, async (stringBuilder, token) =>
                        {
                            var chapter = stringBuilder.ToString();
                            var bytes = await GenerateSingleAsync((title, batchSize), chapter);

                            bytesList.Add((batches.IndexOf(stringBuilder), bytes));
                        });

                        var audio = ConcatAudio(bytesList);

                        await File.WriteAllBytesAsync(path, audio);

                        watch.Stop();

                        Logger.Log($"Audio generating of {title} has completed in {watch.Elapsed.TotalSeconds:F1}s");
                    }
                });
        }
    }

    private byte[] ConcatAudio(IEnumerable<(int, byte[])> audio)
    {
        var waitList = audio
            .OrderBy(x => x.Item1)
            .Select(x => x.Item2)
            .ToList();

        var bytes = new List<byte>();

        var list = waitList.Aggregate(bytes, (list, bytes1) => list.Concat(bytes1).ToList());

        return list.ToArray();
    }

    private async Task<byte[]> GenerateSingleAsync((string, int) meta, string text)
    {
        var result = await Policy
            .Handle<Exception>()
            .RetryAsync(5, (exception, i) =>
            {
                Logger.Log($"Some single request has failed, retrying for {i} time", exception);
            })
            .ExecuteAndCaptureAsync(async () =>
            {
                var bytes = await TTSService.RequestSpeechAsync(text);

                return bytes;
            });

        return result.Result;
    }
}