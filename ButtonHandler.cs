using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klassen_Discord_Bot.Handler
{
    class ButtonHandler
    {
        private Program program;
        public ButtonHandler(Program program)
        {
            this.program = program;
        }

        public async Task NewRootTrainer(SocketMessageComponent component)
        {

            int streak = int.Parse(component.Message.Embeds.First().Fields.First().Name.Split(": ")[1]);

            if (component.Message.Embeds.First().Color == Color.Red) streak = 0;

            await component.UpdateAsync(x =>
            {
                x.Components = new ComponentBuilder().WithButton("Play Again", "root-play-again", emote: new Emoji("🔄"), disabled: true).Build();
            });
            var components = new ComponentBuilder();


            int rootRange = 20;

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("select a number")
                .WithCustomId("menu-root-trainer")
                .WithMinValues(1)
                .WithMaxValues(1);


            for (int i = 0; i < rootRange; i++)
            {
                menuBuilder.AddOption((i + 1).ToString(), (i + 1).ToString(), "select this if you think the root of the number is " + (i + 1));
            }

            components.WithSelectMenu(menuBuilder);
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Do you know the answer?",
                Description = "√" + Math.Pow(new Random().Next(1, rootRange + 1), 2),
                Color = Color.DarkRed,
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder() { Text = "Made by Kevin Gong" }
            };

            embed.AddField($"Current Streak : {streak}", "I know you know this, its first grade stuff", true);
            await component.FollowupAsync("Here is a task!", embed:embed.Build(), components: components.Build());
        }
        public async Task HandleInteraction(SocketMessageComponent component)
        {
            try
            {
                Console.WriteLine("Got Data!");
                // We can now check for our custom id
                switch (component.Data.CustomId)
                {
                    case "root-play-again":
                        await NewRootTrainer(component);
                        break;

                    // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                    case "test-button":
                        // Lets respond by sending a message saying they clicked the button
                        await component.RespondAsync($"{component.User.Mention} has clicked the button!");
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }
            }
            catch (Exception ex)
            {
                await program.HandleException(ex);
            }
        }
    }
}
