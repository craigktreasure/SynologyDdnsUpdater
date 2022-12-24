namespace Namecheap.Library.Models;

using System.Xml.Serialization;

/// <summary>
/// Represents errors from a Namecheap DDNS update response.
/// </summary>
[XmlRoot("errors")]
public class NamecheapDdnsUpdateResponseErrors
{
    /// <summary>
    /// Gets the first error.
    /// </summary>
    [XmlElement("Err1")]
    public string? Error1 { get; init; }
}
