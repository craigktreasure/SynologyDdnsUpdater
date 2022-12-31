namespace Synology.Ddns.Update.Service.Options;

internal class MonitoringOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = nameof(MonitoringOptions);

    /// <summary>
    /// Gets or sets a value indicating whether OpenTelemetry should be enabled.
    /// </summary>
    public bool OpenTelemetryEnabled { get; set; }

    /// <summary>
    /// Gets a <see cref="MonitoringOptions" /> from configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="MonitoringOptions"/>.</returns>
    public static MonitoringOptions FromConfiguration(IConfiguration configuration)
    {
        MonitoringOptions options = new();
        configuration.GetSection(SectionName).Bind(options);

        return options;
    }
}
