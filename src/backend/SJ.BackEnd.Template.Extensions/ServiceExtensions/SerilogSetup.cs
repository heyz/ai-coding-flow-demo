using Elasticsearch.Net;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using SJ.BackEnd.Template.Common;

namespace SJ.BackEnd.Template.Extensions.ServiceExtensions;

public static class SerilogSetup
{
    public static LoggerConfiguration SetupSerilogSinks(
        this LoggerConfiguration loggerConfig,
        SerilogConfig config)
    {
        var minLevel = ParseLogEventLevel(config.MinimumLevel.Default);

        loggerConfig.MinimumLevel.Is(minLevel);

        foreach (var (source, level) in config.MinimumLevel.Override)
        {
            loggerConfig.MinimumLevel.Override(source, ParseLogEventLevel(level));
        }

        foreach (var enrich in config.Enrich)
        {
            switch (enrich)
            {
                case "FromLogContext":
                    loggerConfig.Enrich.FromLogContext();
                    break;
                case "WithMachineName":
                    loggerConfig.Enrich.WithMachineName();
                    break;
                case "WithThreadId":
                    loggerConfig.Enrich.WithThreadId();
                    break;
            }
        }

        loggerConfig.WriteTo.Console(theme: AnsiConsoleTheme.Code);

        loggerConfig.WriteTo.Async(a => a.File(
            new CompactJsonFormatter(),
            "logs/log-.log",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 102400,
            flushToDiskInterval: TimeSpan.FromSeconds(2),
            retainedFileCountLimit: 7
        ));

        if (config.Seq.Enabled && !string.IsNullOrWhiteSpace(config.Seq.ServerUrl))
        {
            var seqLevel = ParseLogEventLevel(config.Seq.MinimumLevel);
            loggerConfig.WriteTo.Async(a => a.Seq(
                config.Seq.ServerUrl,
                apiKey: config.Seq.ApiKey,
                restrictedToMinimumLevel: seqLevel
            ));
        }

        if (config.Elasticsearch.Enabled && !string.IsNullOrWhiteSpace(config.Elasticsearch.ServerUrl))
        {
            var esLevel = ParseLogEventLevel(config.Elasticsearch.MinimumLevel);
            var connectionPool = new SingleNodeConnectionPool(new Uri(config.Elasticsearch.ServerUrl));
            loggerConfig.WriteTo.Async(a => a.Elasticsearch(
                new ElasticsearchSinkOptions(connectionPool)
                {
                    IndexFormat = config.Elasticsearch.IndexFormat,
                    AutoRegisterTemplate = config.Elasticsearch.AutoRegisterTemplate,
                    MinimumLogEventLevel = esLevel
                }
            ));
        }

        return loggerConfig;
    }

    private static LogEventLevel ParseLogEventLevel(string level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var result)
            ? result
            : LogEventLevel.Information;
    }
}