namespace Namecheap.Library.Models;

using System.Xml.Serialization;

/// <summary>
/// Represents errors from a Namecheap DDNS update response.
/// </summary>
[XmlRoot("response")]
public class NamecheapDdnsUpdateOperationResponse
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    [XmlElement("Description")]
    public required string Description { get; init; }

    /// <summary>
    /// Gets the response number.
    /// </summary>
    [XmlElement("ResponseNumber")]
    public int ResponseNumber { get; init; }

    /// <summary>
    /// Gets or sets the response string.
    /// </summary>
    [XmlElement("ResponseString")]
    public required string ResponseString { get; set; }
}
