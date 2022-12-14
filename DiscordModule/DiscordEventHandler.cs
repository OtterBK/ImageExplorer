
using Common;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;

namespace MySettings {
    public class DiscordEventHandler {

        public DiscordSocketClient client_;
        public Queue<SocketSlashCommand> requestQueue = new Queue<SocketSlashCommand>();

        public DiscordEventHandler(DiscordSocketClient client) {
            this.client_ = client;
        }

        //초기 실행 시
        public async Task OnClientReady() {
            //슬래시 명령어 등록
            await CreateCommand();

            await client_.SetGameAsync("/짤");

            Thread ProcessRequestThread = new Thread(() => {
                while (true) {
                    Thread.Sleep(100);
                    while (requestQueue.Count > 0) {
                        Console.WriteLine($"Request Queue size: {requestQueue.Count}");
                        var requestCommand = requestQueue.Dequeue();
                        ProcessRequest(requestCommand);
                    }
                }
            });
            ProcessRequestThread.Start();
        }

        //public void ProcessRequest(SocketSlashCommand command) {

        //    var options = command.Data.Options.ToList();
        //    if (options.Count == 0) {
        //        //TODO 짤 명령어 설명 표시
        //        return;
        //    }

        //    var searchWord = options[0].Value.ToString();

        //    var searchIndex = 1;
        //    if (options.Count > 1) {
        //        var secondOption = options[1].Value;
        //        Int32.TryParse(secondOption.ToString(), out int secondOptionValue);
        //        searchIndex = secondOptionValue;
        //    }

        //    var image_url = CommonInstance.imageExplorer.GetImageUrl(searchWord, searchIndex);

        //    //command.ModifyOriginalResponseAsync(msg => msg.Content = $"{searchWord}에 대한 {searchIndex}번째 이미지 결과 {image_url}");
        //    if(image_url == null) {
        //        command.ModifyOriginalResponseAsync(msg => msg.Content = $"{searchWord}, {searchIndex} 에 대한 이미지를 찾지 못했습니다... 아마 버그일겁니다.");
        //    } else {
        //        command.ModifyOriginalResponseAsync(msg => msg.Content = $"{image_url}");
        //        command.Channel.SendMessageAsync($"{searchWord}에 대한 {searchIndex}번째 이미지 결과");
        //    }

        //}

        public void ProcessRequest(SocketSlashCommand command) {

            var options = command.Data.Options.ToList();
            if (options.Count == 0) {
                //TODO 짤 명령어 설명 표시
                return;
            }

            var searchWord = options[0].Value.ToString();

            var searchIndex = 1;
            if (options.Count > 1) {
                var secondOption = options[1].Value;
                Int32.TryParse(secondOption.ToString(), out int secondOptionValue);
                searchIndex = secondOptionValue;
            }

            var image_url = CommonInstance.naverSearchEngine.GetImageUrl(searchWord, searchIndex);

            //command.ModifyOriginalResponseAsync(msg => msg.Content = $"{searchWord}에 대한 {searchIndex}번째 이미지 결과 {image_url}");
            if (image_url == null) {
                command.ModifyOriginalResponseAsync(msg => msg.Content = $"{searchWord}, {searchIndex} 에 대한 이미지를 찾지 못했습니다... 아마 버그일겁니다.");
            } else {
                command.ModifyOriginalResponseAsync(msg => msg.Content = $"{image_url}");
                command.Channel.SendMessageAsync($"{searchWord}에 대한 {searchIndex}번째 이미지 결과");
            }

        }

        /** 명령어 생성 부 **/
        public async Task CreateCommand() {
            Console.WriteLine("creating commands");

            var imageSearchCommand = new SlashCommandBuilder()
                .WithName("짤")
                .WithDescription("검색어에 해당하는 짤을 찾습니다..")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("검색어")
                    .WithDescription("찾고자하는 짤의 검색어를 입력해주세요.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("짤번호")
                    .WithDescription("여러 짤 중 몇번째 짤을 가져올 지에 대한 선택 옵션")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.Integer)
                );

            var imageSearchWithDriverCommand = new SlashCommandBuilder()
                .WithName("짤 크롤링")
                .WithDescription("검색어에 해당하는 짤을 찾습니다..")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("검색어")
                    .WithDescription("찾고자하는 짤의 검색어를 입력해주세요.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("짤번호")
                    .WithDescription("여러 짤 중 몇번째 짤을 가져올 지에 대한 선택 옵션")
                    .WithRequired(false)
                    .WithType(ApplicationCommandOptionType.Integer)
                );

            await client_.CreateGlobalApplicationCommandAsync(imageSearchCommand.Build());

            foreach (var guild in client_.Guilds) {
                Console.WriteLine($"creating commands for guild {guild.Id}");
                try {
                    await guild.CreateApplicationCommandAsync(imageSearchCommand.Build());
                    //await guild.CreateApplicationCommandAsync(imageSearchWithDriverCommand.Build());
                    Console.WriteLine($"{guild.Id} / {guild.Name} 에 명령어 등록 성공");
                } catch (HttpException exception) {
                    Console.WriteLine($"{guild.Id} / {guild.Name} 에 명령어 등록 실패");
                    Console.WriteLine(exception.Message);
                }
            }

            Console.WriteLine("commands has been created");
        }

        /** 슬래시 명령어 핸들링 **/
        public async Task OnSlashCommandExecuted(SocketSlashCommand command) {
            Console.WriteLine($"executed {command.User} {command.Data.Name}");
            var commandData = command.Data;
            var commandName = commandData.Name;

            switch (commandName) {
                case "짤":
                    await HandleImageSearchCommand(command);
                    break;
                //case "짤 크롤링":
                //    await HandleImageSearchWithDriverCommand(command); 
                //    break;
            }
        }

        //슬래시 명령어 기능
        public async Task HandleImageSearchCommand(SocketSlashCommand command) {

            await command.DeferAsync();

            ProcessRequest(command);
        }
        public async Task HandleImageSearchWithDriverCommand(SocketSlashCommand command) {

            requestQueue.Enqueue(command);

            await command.DeferAsync();

        }

    }
}
