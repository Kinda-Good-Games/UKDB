using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Discord.Net;
using Discord.WebSocket;
using WebUntis.Net;
using Discord;
using Klassen_Discord_Bot.Exceptions;
using Discord.Commands;

namespace Klassen_Discord_Bot.Handler
{
    class SlashCommandHandler
    {
        private Program origin;
        public SlashCommandHandler(Program program)
        {
            origin = program;
        }
        #region Commands
        public async Task TestMenu(SocketSlashCommand command)
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select an option")
                .WithCustomId("menu-1")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption("Option A", "opt-a", "Option B is lying!")
                .AddOption("Option B", "opt-b", "Option A is telling the truth!");
            var row = new ActionRowBuilder();
            row.AddComponent((IMessageComponent)menuBuilder);

            var builder = new ComponentBuilder();

            await command.RespondAsync("Whos really lying?", components: builder.Build());
        }
        public async Task SquareRootTrainer(SocketSlashCommand command)
        {
            int streak = 0;
            var components = new ComponentBuilder();

            int rootRange = 20;

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("select a option")
                .WithCustomId("menu-root-trainer")
                .WithMinValues(1)
                .WithMaxValues(1);


            for (int i = 0; i < rootRange; i++)
            {
                menuBuilder.AddOption((i + 1).ToString(), (i + 1).ToString(), "select this if you think the root of the number is " + (i + 1));
            }

            components.WithSelectMenu(menuBuilder);


            var embed = new EmbedBuilder()
            {
                Title = "Do you know the answer?",
                Description = "√" + Math.Pow(new Random().Next(1, rootRange + 1), 2),
                Color = Color.DarkRed,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.AddField($"Current Streak : {streak}", "You surely know this!", true); ;

            await command.RespondAsync("Here is a task, can you solve it?!", embed: embed.Build(), components: components.Build());
        }

        public async Task ButtonTest(SocketSlashCommand command)
        {
            var builder = new ComponentBuilder()
                .WithButton("This is a test", "test-button");

            await command.RespondAsync("Here is a button!", embed: new EmbedBuilder()
            {
                Title = "This is a test command",
                Description = "Try pressing the button and see what happens!",
                Color = Color.DarkRed,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build(), components: builder.Build());
        }
        public async Task Cheese(SocketSlashCommand command)
        {
            await command.RespondAsync("cheese.");
        }

        public async Task Ping(SocketSlashCommand command)
        {
            await command.RespondAsync("pong");
        }
        public async Task SayHi(SocketSlashCommand command)
        {
            await command.RespondAsync("Hi My Friend");
        }
        public async Task NoOrYes(SocketSlashCommand command)
        {
            Random rndm = new Random();
            int i = rndm.Next(0, 2);
            string reply;
            if (i == 0)
            {
                reply = "Yes";
            }
            else
            {
                reply = "No";
            }
            await command.RespondAsync(reply);
        }
        public async Task Lenny(SocketSlashCommand command)
        {
            await command.RespondAsync("( ͡° ͜ʖ ͡°)");
        }
        public async Task Cookies(SocketSlashCommand command)
        {
            Random rnd = new Random();

            int i = rnd.Next(-1, 65);
            string s = "take some cookies: ";
            for (int x = 0; x < i; x++)
            {
                s += ":cookie:";
            }
            s += $"You have got {i} cookies!";
            if (i == 0)
            {
                s = "Sorry i´m out of cookies :sad:";
            }
            if (i == 1)
            {
                s += "L bro. just 1";
            }
            else if (i == 64)
            {
                s += " now im poorer than @Chipstyp_8109";
            }
            await command.RespondAsync(s);
        }
        public async Task Guilds(SocketSlashCommand command)
        {
            string guilds = "";
            foreach (var guild in origin.client.Guilds)
            {
                guilds += guild;
            }

            await command.RespondAsync(embed: new EmbedBuilder()
            {
                Title = "Server List",
                Description = "these are my servers: " + guilds,
                Color = Color.Blue,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build());
        }
        public async Task UntisHomework(SocketSlashCommand command)
        {
            await command.RespondAsync("Getting homework data...");

            WebUntisClient client = new WebUntisClient("Wächtler-gym-essen", "GongKev", "123Banane!", "borys.webuntis.com");

            await client.LoginAsync();

            var HomeWorks = await client.GetHomeWorksForDateAsync(DateTime.Now, DateTime.Now + TimeSpan.FromDays(7));

            string homeworks = "";

            foreach (var item in HomeWorks)
            {
                string s = $"Fach: {item.Subject.Subject} \n" +
                           $"Inhalt: {item.Text}\n" +
                           $"Abgabedatum: {item.DueDate.ToString()}";
                homeworks += s + "\n\n";
            }



            await command.FollowupAsync(embed: new EmbedBuilder()
            {
                Description = homeworks,
                Title = "Homework",
                Color = Color.Orange,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build());
        }
        public async Task Say(SocketSlashCommand command)
        {
            await command.DeferAsync();
            await command.FollowupAsync(embed: new EmbedBuilder()
            {
                Description = (string)command.Data.Options.First(),
                //Title = "Here, a glorifying message from our lord and savior",
                Color = Color.Gold,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            }.Build());
        }
        public async Task ForKarlo(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder()
            {
                Description = "wonderful stuff, just for " + command.User,
                Title = "Good Stuff",
                Color = Color.Gold,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            };
            embed.AddField("Field title",
                "Here is your [link](https://youtu.be/dQw4w9WgXcQ)!");
            await command.RespondAsync("Here you go!", embed: embed.Build());
        }
        public async Task UntisClasses(SocketSlashCommand command)
        {
            await command.RespondAsync("Getting Class data...");

            WebUntisClient client_wu = new WebUntisClient("Wächtler-gym-essen", "GongKev", "123Banane!", "borys.webuntis.com");

            await client_wu.LoginAsync();

            DateTime wantedDay = DateTime.Now;
            if (command.Data.Options.Where(x => x.Name == "days") != null)
            {
                var dayData = command.Data.Options.Where(x => x.Name == "days");
                wantedDay = DateTime.Now.AddDays((long)command.Data.Options.First().Value);
            }

            TimeTable tt = await client_wu.getOwnClassTimetableForDateAsync(wantedDay);

            await client_wu.LogoutAsync();

            Dictionary<DateTime, TimeTablePart> usedEndTimes = new Dictionary<DateTime, TimeTablePart>();

            foreach (var item in tt)
            {
                // goes through all values (time table part) of the used End Times to check if this item should be skipped

                bool shouldBeSkipped = false;

                foreach (var timeTablePart in usedEndTimes)
                {
                    if (timeTablePart.Key == item.EndTime)
                    {
                        Console.WriteLine(timeTablePart.Value.ActivityType);
                        shouldBeSkipped = true;
                        foreach (var subject in item.Subjects)
                        {
                            timeTablePart.Value.Subjects.Add(subject);
                        }
                        break;
                    }
                }

                if (shouldBeSkipped) continue;

                usedEndTimes.Add(item.EndTime, item);

            }

            SortedDictionary<DateTime, TimeTablePart> sortedClasses = new SortedDictionary<DateTime, TimeTablePart>(usedEndTimes);

            string classText = "";
            foreach (var item in sortedClasses)
            {
                string subjectText = "";
                foreach (var subject in item.Value.Subjects)
                {
                    subjectText += subject.Longname + ", ";
                }
                classText += $"Start Time: **{item.Value.StartTime.ToString("H:mm")}**   End Time: **{item.Value.EndTime.ToString("H:mm")}**\n Subjects: {subjectText} \n";
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
            {
                Color = Color.Orange,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            };

            embedBuilder.AddField("TIMETABLE", classText);
            await command.FollowupAsync($"The time table for the following date: {wantedDay}", embed: embedBuilder.Build());

        }
        #endregion
        public async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "roottrainer":
                    await SquareRootTrainer(command);
                    break;
                case "root-trainer":
                    await SquareRootTrainer(command);
                    break;
                case "buttontest":
                    await ButtonTest(command);
                    break;
                case "say":
                    await Say(command);
                    break;
                case "getserverlist":
                    await Guilds(command);
                    break;
                case "boobies":
                    await ForKarlo(command);
                    break;
                case "yesorno":
                    await NoOrYes(command);
                    break;
                case "cookies":
                    await Cookies(command);
                    break;
                case "lennyface":
                    await Lenny(command);
                    break;
                case "sayhi":
                    await SayHi(command);
                    break;
                case "cheese":
                    await Cheese(command);
                    break;
                case "alert":
                    await command.RespondAsync("Haha, nö.");
                    break;
                case "untistest":
                    await command.RespondAsync("Sorry this command seems to be removed :/");
                    break;
                case "untisclasses":
                    await UntisClasses(command);
                    break;
                case "untishomework":
                    await UntisHomework(command);
                    break;
                default:
                    throw new CommandNotImplementedException();
                    break;
            }
        }
    }
}
