namespace Namecheap.Library.Tests.Extensions;

using Namecheap.Library.Extensions;
using Namecheap.Library.Models;
using System.Text;
using Test.Library.Namecheap;

public class StreamExtensionsTests
{
    [Fact]
    public void ReadFromXml()
    {
        // Arrange
        using Stream stream = new MemoryStream(Encoding.Unicode.GetBytes(MockResponseConstants.Success));

        // Act
        NamecheapDdnsUpdateResponse? result = stream.ReadFromXml<NamecheapDdnsUpdateResponse>();

        // Act
        Assert.NotNull(result);
    }
}
