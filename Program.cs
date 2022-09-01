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
using WebUntisSharp;
using Klassen_Discord_Bot.WebServer;
using Klassen_Discord_Bot.Handler;

namespace Klassen_Discord_Bot
{
    class Program
    {
        #region TOKEN
        private const string TOKEN = "MTAxNDI0ODM2MjU0MDc5Mzg5Ng.GDYg1f.YVtuzxF9gHSy68uH5KSutJu3uSwjf084nC-77I";
        #endregion
        public DiscordSocketClient client { get; private set; }
        public ButtonHandler buttonHandler { get; private set; }

        private CommandService commands;
        private SlashCommandHandler slashCommandHandler;
        private HttpServer server;

        private SocketGuild classGuild;
        private ISocketMessageChannel ukdbChannel;

        /*
         Color Codes:
            Orange  -   WebUntis
            Red     -   Error
            Blue    -   Bot Stats
            Green   -   Fun Commands
         */

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            try
            {
                Console.WriteLine("Starting the discord bot, please wait... ");
                DiscordSocketConfig config = new()
                {
                    UseInteractionSnowflakeDate = false,
                };

                client = new DiscordSocketClient();

                slashCommandHandler = new SlashCommandHandler(this);

                client.Ready += Client_Ready;
                client.Log += Log;
                client.LoggedOut += LogOut;

                buttonHandler = new(this);

                client.ButtonExecuted += buttonHandler.HandleInteraction;

                commands = new CommandService();

                await client.LoginAsync(TokenType.Bot, TOKEN);
                await client.StartAsync();

                await InstallCommandsAsync();

                server = new HttpServer();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }
        public async Task HandleException(Exception ex)
        {
            if (ukdbChannel != null)
            {
                var embed = new EmbedBuilder()
                {
                    Title = "An Error occured!",
                    Description = "Please report this to the developer!",
                    Color = Color.Red,
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" },
                };

                var trace = new System.Diagnostics.StackTrace(ex, true);

                embed.AddField(ex.GetType().Name, ex.Message);
                embed.AddField("Stack Trace", trace.ToString());
                await ukdbChannel.SendMessageAsync(embed: embed.Build(), flags: MessageFlags.Urgent);
            }
            Console.WriteLine(ex);
        }
        public async Task LogOut()
        {
            await ukdbChannel.SendMessageAsync(embed: new EmbedBuilder()
            {
                Title = "Good Morning!",
                Description = "I'm online now! You can use /[command name] to execute commands",
                Color = Color.Blue,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build());
        }
        public async Task Client_Ready()
        {
            classGuild = client.GetGuild(1011027844350103723);
            foreach (var item in classGuild.Channels)
            {
                if (item.Name == "ukdb-channel") { ukdbChannel = (ISocketMessageChannel)item; break; }
            }
            Console.WriteLine($"Logged in with {client.Latency} ms latency");

            await ukdbChannel.SendMessageAsync(embed: new EmbedBuilder()
            {
                Title = "Good Morning!",
                Description = "I'm online now! You can use /[command name] to execute commands",
                Color = Color.Blue,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build());

            await Task.Delay(1);

            await CreateSlashCommands();
        }
        private async Task CreateSlashCommands()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            try
            {
                var guildCommand = new SlashCommandBuilder();

                // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$

                #region command declaration

                /*guildCommand.WithName("untisclasses");
                guildCommand.WithDescription("gets the time table of a certain day");
                guildCommand.AddOption("days", ApplicationCommandOptionType.Integer, "gets added towards the current date to the timetable");
                await guild.CreateApplicationCommandAsync(guildCommand.Build());*/

                guildCommand = guildCommand.WithName("buttontest");
                guildCommand = guildCommand.WithDescription("a test button");

                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());

                guildCommand = guildCommand.WithName("say");
                guildCommand = guildCommand.WithDescription("lemme say something");
                guildCommand.AddOption("content", ApplicationCommandOptionType.String, "what I am going to say", isRequired:true);
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());

                guildCommand = new();

                guildCommand = guildCommand.WithName("about");
                guildCommand = guildCommand.WithDescription("get information about me!");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());

                guildCommand = new();

                guildCommand.WithName("getserverlist");
                guildCommand.WithDescription("gets all servers i am on");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();

                ///* 
                guildCommand.WithName("lennyface");

                // Descriptions can have a max length of 100.
                guildCommand.WithDescription("( ͡° ͜ʖ ͡°)");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();


                guildCommand.WithName("yesorno");
                guildCommand.WithDescription("responds either with yes or no");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();


                guildCommand.WithName("cheese");
                guildCommand.WithDescription("cheese.");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();


                guildCommand.WithName("ping");
                guildCommand.WithDescription("answers pong");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();

                guildCommand.WithName("sayhi");
                guildCommand.WithDescription("Hey!");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();
                guildCommand.WithName("cookies");
                guildCommand.WithDescription("yummy!");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();


                guildCommand.WithName("lennyface");
                guildCommand.WithDescription("( ͡° ͜ʖ ͡°)");
                await classGuild.CreateApplicationCommandAsync(guildCommand.Build());
                guildCommand = new();
                //*/

                // Let's do our global command
                var globalCommand = new SlashCommandBuilder();
                globalCommand.WithName("first-global-command");
                globalCommand.WithDescription("This is my first global slash command");

                #endregion
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
            try
            {
                await slashCommandHandler.HandleSlashCommand(command);
            }
            catch (Exception e)
            {
                var embed = new EmbedBuilder()
                {
                    Title = "An Error occured!",
                    Description = "Please report this to the developer!",
                    Color = Color.Red,
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" },
                };

                var trace = new System.Diagnostics.StackTrace(e, true);

                embed.AddField(e.GetType().Name, e.Message);
                string traceText = trace.ToString();
                if (traceText.Length > 1024)
                {
                    traceText = traceText.Remove(1021);
                    traceText += "...";
                }
                embed.AddField("Stack Trace", traceText);
                if (!command.HasResponded)
                {
                    await command.RespondAsync("Oh no! seems like there was an error while executing the command!", embed: embed.Build());
                }
                else
                {
                    await command.FollowupAsync("Oh no! seems like there was an error while executing the command!", embed: embed.Build());
                }
            }
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

            if (message.Content == "hi")
            {
                await message.ReplyAsync("Hi!");
            }


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
