using System.Text;
using MoreLinq;

namespace Aduboker.Core.Services;

public class ChapterService
{
    public string Text { get; set; }

    public ChapterService(string text = null)
    {
        Text = text;
    }

    public static implicit operator ChapterService(string path)
    {
        return new ChapterService(path);
    }

    public async Task<List<StringBuilder>> GetChaptersAsync()
    {
        var chapters = new List<StringBuilder>();
        var lines = await GetLinesAsync();

        StringBuilder builder = null;

        foreach (var line in lines)
        {
            if (((line.Contains("第") && line.Contains("章")) || line.Contains("番外")) && !line.StartsWith(" "))
            {
                if (builder is not null)
                {
                    chapters.Add(builder);
                }

                builder = new StringBuilder($"{line}{Environment.NewLine}");
                continue;
            }

            builder?.Append($"{line}{Environment.NewLine}");

        }

        var index = lines.FindLastIndex(line =>
            ((line.Contains("第") && line.Contains("章")) || line.Contains("番外")) && !line.StartsWith(" "));

        chapters.Add(new StringBuilder(string.Join(string.Empty, lines.Take(new Range(index, lines.Count)))));

        return chapters;
    }

    /// <summary>
    /// Key: tuple of (string, int), which denoted chapter name and count of split batches
    /// Value: Split batches
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<(string, int), List<StringBuilder>>> GetSplitChaptersAsync(int batchCapacity = 2000, Range? range = null)
    {
        var re = new Dictionary<(string, int), List<StringBuilder>>();

        var chapters = await GetChaptersAsync();

        if (range is not null)
        {
            chapters = chapters.Take(range.Value).ToList();
        }

        foreach (var builder in chapters)
        {
            var chapter = builder.ToString();
            var meta = (chapter.Split(Environment.NewLine).First(), GetBatchSize(chapter, batchCapacity));

            var batch = chapter.Batch(batchCapacity);

            var toBuilders = batch
                .Select(x => string.Concat(x))
                .Select(x => new StringBuilder(x));

            re.Add(meta, toBuilders.ToList());
        }

        return re;
    }

    private int GetBatchSize(string text, int batchCapacity)
    {
        var div = Math.DivRem(text.Length, batchCapacity, out var remain);

        return remain == 0 ? div : div + 1;
    }

    public async Task<List<string>> GetLinesAsync()
    {
        return await Task.Run(() => Text.Split(Environment.NewLine).ToList());
    }
}