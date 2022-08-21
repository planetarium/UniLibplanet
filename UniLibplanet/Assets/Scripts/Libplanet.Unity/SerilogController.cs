using Serilog;
using Serilog.Events;

namespace Libplanet.Unity
{
    public class SerilogController
    {
        /// <summary>
        /// Creates a new log file which is named to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The path to save a file.</param>
        /// <param name="logLevel">Set Log Event Level which is want to write log sentences.</param>
        /// <param name="logMessage">The message to write what we want to log.</param>
        public static void WriteSerilog(string fileName, LogEventLevel logLevel, string logMessage)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(fileName)
                .CreateLogger();

            switch (logLevel)
            {
                case LogEventLevel.Verbose:
                    Log.Verbose(logMessage);
                    break;
                case LogEventLevel.Debug:
                    Log.Debug(logMessage);
                    break;
                case LogEventLevel.Information:
                    Log.Information(logMessage);
                    break;
                case LogEventLevel.Warning:
                    Log.Warning(logMessage);
                    break;
                case LogEventLevel.Error:
                    Log.Error(logMessage);
                    break;
                case LogEventLevel.Fatal:
                    Log.Fatal(logMessage);
                    break;
            }
        }
    }
}
