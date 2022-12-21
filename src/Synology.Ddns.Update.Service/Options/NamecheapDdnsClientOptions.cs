namespace Synology.Ddns.Update.Service.Options;

/// <summary>
/// Options for the Namecheap DDNS client.
/// </summary>
internal class NamecheapDdnsClientOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = nameof(NamecheapDdnsClientOptions);

    /// <summary>
    /// Gets or sets a value indicating whether to use a mock client.
    /// </summary>
    public bool MockClient { get; set; }

    /// <summary>
    /// Gets a <see cref="NamecheapDdnsClientOptions" /> from configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="NamecheapDdnsClientOptions"/>.</returns>
    public static NamecheapDdnsClientOptions FromConfiguration(IConfiguration configuration)
    {
        NamecheapDdnsClientOptions options = new();
        configuration.GetSection(SectionName).Bind(options);

        return options;
    }
}
