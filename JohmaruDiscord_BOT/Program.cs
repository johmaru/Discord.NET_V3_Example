using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private DiscordSocketClient _client;
    
    //メインのコード
    public async Task MainAsync()
    {
        //ここでGateWayIntentsをALLにしないとユーザーの入力テキストが認識されずに!pingを打っても反応しない
        
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        
        //BOTのコア部分起動 Configを添えて
        
        _client = new DiscordSocketClient(config);
        
        //コマンド機能のコアを起動

        var commandService = new CommandService();

        //ここでMyCommandの内容を呼び出し
        
        await InstallCommandAsync();
        
        //LOGを作成
        
        _client.Log += Log;
        
        //ここにトークを入れて

        var token = "BOTのトークンに変更";
        
        //BOTにLOGINする

        await _client.LoginAsync(TokenType.Bot, token);
        
        //システム起動

        await _client.StartAsync();

        await Task.Delay(-1);
        
        //MyCommandとかのシステムをCommandのシステムに追加
        
         async Task InstallCommandAsync()
         {
             _client.MessageReceived += HandleCommandAsync;

             await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
         }
         
         //コマンドを制御する的なやつ

        async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null) return;

            if (message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
            
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                
                message.Author.IsBot)
                    
                return;

            var context = new SocketCommandContext(_client, message);

            var result = await commandService.ExecuteAsync(context, argPos, null, MultiMatchHandling.Best);

            if (!result.IsSuccess)
            
                await context.Channel.SendMessageAsync(result.ErrorReason);

        }
    }
    

    //LOG作成コード
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    //コマンドを追加するコード
    
    public class MyCommand: ModuleBase<SocketCommandContext>

    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
    }
}