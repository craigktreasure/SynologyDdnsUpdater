namespace Namecheap.Library.Extensions;

using System.Xml;
using System.Xml.Serialization;

internal static class StreamExtensions
{
    /// <summary>
    /// Deserializes the stream content from XML to a <typeparamref name="T"/> asynchronously.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream">The stream.</param>
    /// <returns><typeparamref name="T"/>.</returns>
    public static T? ReadFromXml<T>(this Stream stream)
        where T : class
    {
        using XmlReader xmlReader = new XmlTextReader(stream);

        XmlSerializer xmlSerializer = new(typeof(T));

        object? result = xmlSerializer.Deserialize(xmlReader);

        return result as T;
    }
}
