using Serilog;

namespace Kradis.Helper;

public class LoggerConfigurationHelper
{
    public static void Configure()
    {   
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            
            // Console logs.
            .WriteTo.Logger(lc => lc
                .MinimumLevel.Debug()
                // Exclude
                .Filter.ByExcluding(logEvent =>
                    (
                        logEvent.Properties.TryGetValue("SourceContext", out var source) &&
                        source.ToString().Contains("DefaultHttpClientFactory")
                    )
                )
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                )
            )
            
            // General logs.
            .WriteTo.Logger(lc => lc
                .WriteTo.File(
                    "data/logs/application-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                )
            )
            
            .CreateLogger();
    }
}