using Discord.Commands;
using Discord.WebSocket;

namespace MySettings {
    public class DiscordHandler {

        public DiscordSocketClient client_ = null; //봇 클라이언트
        public CommandService commands = null; //명령어 수신 클라이언트
        public DiscordHandler() {
            Console.WriteLine("Initializing bot");
            InitializeDiscord();
        }

        public async void InitializeDiscord() {

            Console.WriteLine($"Token loaded... {GlobalSetting.token}");

            client_ = new DiscordSocketClient(new DiscordSocketConfig() {
                LogLevel = Discord.LogSeverity.Verbose
            });

            commands = new Discord.Commands.CommandService(new Discord.Commands.CommandServiceConfig() {
                LogLevel = Discord.LogSeverity.Verbose
            });

            //로그 관련
            //DiscordSession.client.Log += OnClientLogReceived;
            //DiscordSession.commands.Log += OnClientLogReceived;

            DiscordSocketClient client = client_;

            await client.LoginAsync(Discord.TokenType.Bot, GlobalSetting.token);
            await client.StartAsync(); //봇 활성화

            DiscordEventHandler eventHandler = new DiscordEventHandler(client);

            /** 실제 이벤트 등록 **/
            client.Ready += eventHandler.OnClientReady; //초기 실행 이벤트 추가
            client.SlashCommandExecuted += eventHandler.OnSlashCommandExecuted; //메시지 수신 이벤트 추가

            Console.WriteLine("Bot started");

            await Task.Delay(-1);

        }

    }
}
