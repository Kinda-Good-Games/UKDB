using Discord.WebSocket;
using Discord.Rest;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Klassen_Discord_Bot
{
    class SelectMenuHandler
    {
        private Program program;
        public SelectMenuHandler(Program program)
        {
            this.program = program;
        }
        public async Task HandleRootData(SocketMessageComponent component)
        {
            await component.UpdateAsync(x => { x.Components = new ComponentBuilder().Build(); });

            int streak = int.Parse(component.Message.Embeds.First().Fields.First().Name.Split(": ")[1]) + 1;

            var text = string.Join(", ", component.Data.Values);

            Embed embed = null;
            foreach (var item in component.Message.Embeds)
            {
                embed = item;
                break;
            }


            int realResult = (int)Math.Sqrt(int.Parse(embed.Description.Remove(0, 1)));
            int choice = int.Parse(text.Trim());

            ComponentBuilder components = new();

            components.WithButton("Play Again", "root-play-again", emote: new Emoji("🔄"));

            RestFollowupMessage followUp;



            if (realResult == choice)
            {
                var embedBuilder = new EmbedBuilder()
                {
                    Title = "And the answer is...",
                    Description = $"✅ {realResult}, You were **right**! Congratulations!",
                    Color = Color.Green,
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
                };

                embedBuilder.AddField($"Current Streak : {streak}", "Keep going! You got this!", true);
                followUp = await component.FollowupAsync(embed: embedBuilder.Build(), components: components.Build());
            }
            else
            {
                var embedBuilder = new EmbedBuilder()
                {
                    Title = "And the answer is...",
                    Description = $"❌ {realResult}, You choose {choice} and were sadly **wrong**, better luck next time!",
                    Color = Color.Red,
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
                };

                embedBuilder.AddField("Your Streak : 0", "Good job! Try again for a better score", true);

                followUp = await component.FollowupAsync(embed: embedBuilder.Build(), components: components.Build());
            }

            // Scrapped

            /*await Task.Delay(TimeSpan.FromSeconds(1));

            components = new();
            components.WithButton("Play Again", "root-play-again" , emote: new Emoji("🔄"),disabled:true);

            await followUp.ModifyAsync(x => { x.Components = components.Build(); });*/
        }
        public async Task HandleSelectMenu(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "menu-root-trainer":
                    await HandleRootData(component);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
