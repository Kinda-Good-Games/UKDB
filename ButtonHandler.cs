using Discord.WebSocket;
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

        public async Task HandleInteraction(SocketMessageComponent component)
        {
            try
            {
                Console.WriteLine("Got Data!");
                // We can now check for our custom id
                switch (component.Data.CustomId)
                {
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
            catch(Exception ex)
            {
                await program.HandleException(ex);
            }
        }
    }
}
