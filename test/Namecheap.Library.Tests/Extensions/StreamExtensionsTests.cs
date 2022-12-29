namespace Namecheap.Library.Tests.Extensions;

using System.Text;

using Namecheap.Library.Extensions;
using Namecheap.Library.Models;

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
