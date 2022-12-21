namespace Namecheap.Library.Models;

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

/// <summary>
/// Represents a Namecheap DDNS update response.
/// </summary>
[XmlRoot("interface-response")]
public class NamecheapDdnsUpdateResponse
{
    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    [XmlElement("Command")]
    public required string Command { get; init; }

    /// <summary>
    /// Gets or sets the debug information.
    /// </summary>
    [XmlElement("debug")]
    public string? Debug { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the operation is done.
    /// </summary>
    [XmlElement("Done")]
    public bool Done { get; set; }

    /// <summary>
    /// Gets or sets the error count.
    /// </summary>
    [XmlElement("ErrCount")]
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the errors.
    /// </summary>
    [XmlElement("errors")]
    public required NamecheapDdnsUpdateResponseErrors Errors { get; init; }

    /// <summary>
    /// Gets or sets the ip address.
    /// </summary>
    [XmlElement("IP")]
    public string? IPAddress { get; set; }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    [XmlElement("Language")]
    public required string Language { get; init; }

    /// <summary>
    /// Gets or sets the response count.
    /// </summary>
    [XmlElement("ResponseCount")]
    public int ResponseCount { get; set; }

    /// <summary>
    /// Gets or sets the responses.
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "XML won't handle interfaces.")]
    [XmlArray("responses")]
    [XmlArrayItem("response")]
    public required NamecheapDdnsUpdateOperationResponse[] Responses { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    [MemberNotNullWhen(true, nameof(IPAddress))]
    public bool Success => this.ErrorCount == 0 && this.IPAddress is not null;
}
