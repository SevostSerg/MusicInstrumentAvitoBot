using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot
{
    public class Program
    {
        private const string GeneralLogsFileName = "log.txt";
        private const string MessagesLogsFileName = "Sended messages.txt";
        private const string OutputTemplate = "{Timestamp} [{Level}] {Message}{NewLine}{Exception}";

        public static ILoggerFactory Logger { get; private set; }

        public static ILoggerFactory SendedMessagesLogger { get; private set; }

        static async Task Main()
        {
            ConfigureLogger();
            await new Test().Start().ConfigureAwait(false);
            Console.ReadLine();
        }

        private static void ConfigureLogger()
        {
            var logger = new LoggerConfiguration()
                     .MinimumLevel.Information()
                     .WriteTo.File(GeneralLogsFileName,
                                    outputTemplate: OutputTemplate,
                                    rollingInterval: RollingInterval.Day,
                                    rollOnFileSizeLimit: true)
                     .CreateLogger();
            var sendedMessagesLogger = new LoggerConfiguration()
                     .MinimumLevel.Information()
                     .WriteTo.File(MessagesLogsFileName,
                                    outputTemplate: OutputTemplate,
                                    rollingInterval: RollingInterval.Day,
                                    rollOnFileSizeLimit: true)
                     .CreateLogger();
            Logger = new LoggerFactory();
            SendedMessagesLogger = new LoggerFactory();
            Logger.AddSerilog(logger);
            SendedMessagesLogger.AddSerilog(sendedMessagesLogger);
        }
    }
}