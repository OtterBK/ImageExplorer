using Common;
using Crawler;
using MySettings;

Console.WriteLine("Start Image Searcher");

GlobalSetting.LoadToken();
GlobalSetting.LoadChromeDriverPath();
GlobalSetting.LoadNaverAPIInfo();

CommonInstance.naverSearchEngine = new NaverSearchEngine();
//CommonInstance.imageExplorer = new ChromeImageExplorer();
CommonInstance.discordHandler = new DiscordHandler();

await Task.Delay(-1);

Console.WriteLine("Bot has been stopped");
