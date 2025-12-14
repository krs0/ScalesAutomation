using log4net;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MetrologyReaderNS;

class Program
{
    private static readonly ILog log = LogManager.GetLogger("generalLog");

    static int Main(string[] args)
    {
        try
        {
            log.Info($"Starting Metrology Reader...");

            var app = new CommandApp<MetrologyCommand>();
            return app.Run(args);
        }
        catch (Exception ex)
        {
            log.Error($"Error: {ex.Message}");
            return 1;
        }
        finally
        {
            log.Info($"Finished Metrology Reader!");
        }
    }
}

public class MetrologySettings : CommandSettings
{
    [CommandArgument(0, "<MEASUREMENTS_CENTRALIZER_FILE_PATH>")]
    [Description("Path to the measurements centralizer excel file")]
    public string MeasurementsCentralizerFilePath { get; set; } = string.Empty;

    [CommandArgument(1, "<MEASUREMENTS_FILE>")]
    [Description("Measurements file name to be interrogated")]
    public string MeasurementsFileName { get; set; } = string.Empty;
}

public class MetrologyCommand : Command<MetrologySettings>
{
    private static readonly ILog log = LogManager.GetLogger("generalLog");

    public override int Execute(CommandContext context, MetrologySettings settings, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate arguments
            if (string.IsNullOrEmpty(settings.MeasurementsCentralizerFilePath) || string.IsNullOrEmpty(settings.MeasurementsFileName))
            {
                log.Error($"Please provide the filepath for measurements centralizer excel file and the filepath for the measurements file to be interrogated as arguments.");
                return 1;
            }

            log.Info($"The following arguments were provided:{Environment.NewLine}" +
                $"\tMeasurement Centralizer File Path '{settings.MeasurementsCentralizerFilePath}'{Environment.NewLine}" +
                $"\tMeasurements File Name '{settings.MeasurementsFileName}'");

            if (!File.Exists(settings.MeasurementsCentralizerFilePath))
            {
                log.Error($"Measureemnts Centralizer file does not exist!");
                return 1;
            }

            var metrologyReader = new MetrologyReader();

            metrologyReader.InitializeExcel(settings.MeasurementsCentralizerFilePath);

            metrologyReader.GetMetrologyResult(settings.MeasurementsFileName);

            metrologyReader.Dispose();

            return 0;
        }
        catch (Exception ex)
        {
            log.Error($"Error: {ex.Message}");
            return 1;
        }
    }
}