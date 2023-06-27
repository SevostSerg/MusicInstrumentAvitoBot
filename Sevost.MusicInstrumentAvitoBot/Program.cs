using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot
{
    public class Program
    {
        

        

        static async Task Main()
        {
            ConfigureLogger();
            await new Test().Start().ConfigureAwait(false);
            Console.ReadLine();
        }

        
    }
}