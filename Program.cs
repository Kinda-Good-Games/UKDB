using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using WebUntis.Net;
using WebUntisSharp;
using Klassen_Discord_Bot.WebServer;

namespace Klassen_Discord_Bot
{
    class Program
    {
        #region TOKEN
        private const string TOKEN = "MTAxNDI0ODM2MjU0MDc5Mzg5Ng.GTlEE1.F1pKH5Qv6pHObRB1yw2g3G6f9WuRcFlP4BjJa4";
        private DiscordSocketClient client;
        #endregion

        private CommandService commands;
        private SlashCommandHandler slashCommandHandler;
        private HttpServer server;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {

            Console.WriteLine("Starting bot");
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                UseInteractionSnowflakeDate = false,
            };

            client = new DiscordSocketClient();

            slashCommandHandler = new SlashCommandHandler();

            client.Ready += Client_Ready;
            client.Log += Log;

            commands = new CommandService();

            await client.LoginAsync(TokenType.Bot, TOKEN);
            await client.StartAsync();

            await InstallCommandsAsync();

            server = new HttpServer();
            await Task.Delay(-1);
        }
        public async Task Client_Ready()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = client.GetGuild(1011027844350103723);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var guildCommand = new SlashCommandBuilder();

            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            guildCommand.WithName("lennyface");

            // Descriptions can have a max length of 100.
            guildCommand.WithDescription("( ͡° ͜ʖ ͡°)");


            guildCommand.WithName("yesorno");
            guildCommand.WithDescription("responds either with yes or no");


            guildCommand.WithName("cheese");
            guildCommand.WithDescription("cheese.");


            guildCommand.WithName("ping");
            guildCommand.WithDescription("answers pong");

            guildCommand.WithName("sayhi");
            guildCommand.WithDescription("Hey!");

            // Let's do our global command
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("first-global-command");
            globalCommand.WithDescription("This is my first global slash command");

            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                await guild.CreateApplicationCommandAsync(guildCommand.Build());

                // With global commands we don't need the guild.
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
                Console.WriteLine("Done!");
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await slashCommandHandler.HandleSlashCommand(command);
        }
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            client.MessageReceived += HandleCommandAsync;

            client.SlashCommandExecuted += SlashCommandHandler;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
