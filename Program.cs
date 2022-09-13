using Common;
using Crawler;
using MySettings;

Console.WriteLine("Start Image Searcher");

GlobalSetting.LoadToken();
GlobalSetting.LoadChromeDriverPath();
CommonInstance.imageExplorer = new ImageExplorer();
CommonInstance.discordHandler = new DiscordHandler();

await Task.Delay(-1);

Console.WriteLine("Bot has been stopped");
