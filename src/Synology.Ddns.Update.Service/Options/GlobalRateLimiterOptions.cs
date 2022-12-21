namespace Synology.Ddns.Update.Service.Options;

/// <summary>
/// Options related to the global rate limiter.
/// </summary>
internal class GlobalRateLimiterOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = nameof(GlobalRateLimiterOptions);

    /// <summary>
    /// Gets or sets the maximum number of permit counters that can be allowed in a window.
    /// </summary>
    public int PermitLimit { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum cumulative permit count of queued acquisition requests.
    /// </summary>
    public int QueueLimit { get; set; } = 5;

    /// <summary>
    /// Gets or sets the time window that takes in the requests.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Gets a <see cref="GlobalRateLimiterOptions" /> from configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="GlobalRateLimiterOptions"/>.</returns>
    public static GlobalRateLimiterOptions FromConfiguration(IConfiguration configuration)
    {
        GlobalRateLimiterOptions options = new();
        configuration.GetSection(SectionName).Bind(options);

        return options;
    }
}
