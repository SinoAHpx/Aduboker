# Audboker

[Auzre Speech Service](https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/) based audio book generator.

## About

AHpx@yandex.com

[Telegram](t.me/AHpxEx)

### Build with

Without assistance of following great projects, Audboker would not exist.

#### Dependencies

+ Flurl.Http
+ morelinq
+ Newtonsoft.Json
+ Polly

#### IDE

+ Visual Studio 2022 Community
+ Reshaper 2021.3.1 (free license for oepn-source project provided by Jetbrains)
## Demo
```cs
using Aduboker.Core.Model;
using Aduboker.Core.Services;

var generator = new AudbokerService
{
    ChapterService = await File.ReadAllTextAsync(@"txt path"),
    Logger = new YourLogger,
    OutputDir = new DirectoryInfo(@"out put directory"),
    TTSService = new TTSService("azure speech service subscription key")
};

//generate all
await generator.GenerateAsync();

//generate chapter 420 to 450
await generator.GenerateAsync(new Range(420, 450));
```
