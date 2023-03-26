using NeoWorlds.EmissaryContainerLib.Builder;
using System.CommandLine;

namespace MakeEmissary
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // build - build an emissary (and optionally push to server)
            // show - show information about current repo
            // list - show builds on server for this repo
            // remove - remove a build on server
            // setup - alter setup information for repo

            var rootCommand = new RootCommand("Emissary Builder")
            {
                 new GenerateTar("build", "Cause Emissary to Be Built")
            };

  

            return await rootCommand.InvokeAsync(args);
        }
    }
}