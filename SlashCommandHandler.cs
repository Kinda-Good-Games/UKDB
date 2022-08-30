using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Discord.Net;
using Discord.WebSocket;
using WebUntis.Net;
using Discord;

namespace Klassen_Discord_Bot
{
    class SlashCommandHandler
    {
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
        public async Task ForKarlo(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder()
            {
                Description = "wonderful stuff, just for " + command.User,
                Title = "Good Stuff",
                Color = Color.Red,
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
        public async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "boobies":
                    await ForKarlo(command);
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
            }
        }
    }
}
