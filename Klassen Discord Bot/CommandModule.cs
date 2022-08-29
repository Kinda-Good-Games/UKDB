using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
namespace Klassen_Discord_Bot
{

    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("HelloWorld")]
        public async Task HelloWorld()
        {
            await ReplyAsync("Hello World");
        }
    }
}
