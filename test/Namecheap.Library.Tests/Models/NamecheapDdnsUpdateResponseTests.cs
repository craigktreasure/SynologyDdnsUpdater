namespace Namecheap.Library.Tests.Models;

using System.Xml;
using System.Xml.Serialization;

using Namecheap.Library.Models;

using static Test.Library.Namecheap.MockResponseConstants;

public class NamecheapDdnsUpdateResponseTests
{
    [Fact]
    public void Deserialize_DomainNameNotFound_Error()
    {
        // Arrange
        XmlSerializer xmlSerializer = new(typeof(NamecheapDdnsUpdateResponse));
        using TextReader textReader = new StringReader(DomainNameNotFound);
        using XmlReader xmlReader = new XmlTextReader(textReader);

        // Act
        object? deserialized = xmlSerializer.Deserialize(xmlReader);

        // Assert
        Assert.NotNull(deserialized);
        NamecheapDdnsUpdateResponse response = Assert.IsType<NamecheapDdnsUpdateResponse>(deserialized);
        Assert.True(response.Done);
        Assert.False(response.Success);
        Assert.Equal(1, response.ErrorCount);
        Assert.Equal(1, response.ResponseCount);
        Assert.Equal("Domain name not found", response.Errors.Error1);
        NamecheapDdnsUpdateOperationResponse operationResponse = Assert.Single(response.Responses);
        Assert.Equal("Domain name not found", operationResponse.Description);
        Assert.Equal(316153, operationResponse.ResponseNumber);
        Assert.Equal("Validation error; not found; domain name(s)", operationResponse.ResponseString);
    }

    [Fact]
    public void Deserialize_InvalidIp_Error()
    {
        // Arrange
        XmlSerializer xmlSerializer = new(typeof(NamecheapDdnsUpdateResponse));
        using TextReader textReader = new StringReader(InvalidIp);
        using XmlReader xmlReader = new XmlTextReader(textReader);

        // Act
        object? deserialized = xmlSerializer.Deserialize(xmlReader);

        // Assert
        Assert.NotNull(deserialized);
        NamecheapDdnsUpdateResponse response = Assert.IsType<NamecheapDdnsUpdateResponse>(deserialized);
        Assert.True(response.Done);
        Assert.False(response.Success);
        Assert.Equal(1, response.ErrorCount);
        Assert.Equal(1, response.ResponseCount);
        Assert.Equal("Invalid IP", response.Errors.Error1);
        NamecheapDdnsUpdateOperationResponse operationResponse = Assert.Single(response.Responses);
        Assert.Equal("Invalid IP", operationResponse.Description);
        Assert.Equal(304156, operationResponse.ResponseNumber);
        Assert.Equal("Validation error; invalid ; IP Address", operationResponse.ResponseString);
    }

    [Fact]
    public void Deserialize_PasswordsDoNotMatch_Error()
    {
        // Arrange
        XmlSerializer xmlSerializer = new(typeof(NamecheapDdnsUpdateResponse));
        using TextReader textReader = new StringReader(PasswordsDoNotMatch);
        using XmlReader xmlReader = new XmlTextReader(textReader);

        // Act
        object? deserialized = xmlSerializer.Deserialize(xmlReader);

        // Assert
        Assert.NotNull(deserialized);
        NamecheapDdnsUpdateResponse response = Assert.IsType<NamecheapDdnsUpdateResponse>(deserialized);
        Assert.True(response.Done);
        Assert.False(response.Success);
        Assert.Equal(1, response.ErrorCount);
        Assert.Equal(1, response.ResponseCount);
        Assert.Equal("Passwords do not match", response.Errors.Error1);
        NamecheapDdnsUpdateOperationResponse operationResponse = Assert.Single(response.Responses);
        Assert.Equal("Passwords do not match", operationResponse.Description);
        Assert.Equal(304156, operationResponse.ResponseNumber);
        Assert.Equal("Validation error; invalid ; password", operationResponse.ResponseString);
    }

    [Fact]
    public void Deserialize_RecordNotFound_Error()
    {
        // Arrange
        XmlSerializer xmlSerializer = new(typeof(NamecheapDdnsUpdateResponse));
        using TextReader textReader = new StringReader(RecordNotFound);
        using XmlReader xmlReader = new XmlTextReader(textReader);

        // Act
        object? deserialized = xmlSerializer.Deserialize(xmlReader);

        // Assert
        Assert.NotNull(deserialized);
        NamecheapDdnsUpdateResponse response = Assert.IsType<NamecheapDdnsUpdateResponse>(deserialized);
        Assert.True(response.Done);
        Assert.False(response.Success);
        Assert.Equal(1, response.ErrorCount);
        Assert.Equal(1, response.ResponseCount);
        Assert.Equal("No Records updated. A record not Found;", response.Errors.Error1);
        NamecheapDdnsUpdateOperationResponse operationResponse = Assert.Single(response.Responses);
        Assert.Equal("No Records updated. A record not Found;", operationResponse.Description);
        Assert.Equal(380091, operationResponse.ResponseNumber);
        Assert.Equal("No updates; A record not Found;", operationResponse.ResponseString);
    }

    [Fact]
    public void Deserialize_Success()
    {
        // Arrange
        XmlSerializer xmlSerializer = new(typeof(NamecheapDdnsUpdateResponse));
        using TextReader textReader = new StringReader(Success);
        using XmlReader xmlReader = new XmlTextReader(textReader);

        // Act
        object? deserialized = xmlSerializer.Deserialize(xmlReader);

        // Assert
        Assert.NotNull(deserialized);
        NamecheapDdnsUpdateResponse response = Assert.IsType<NamecheapDdnsUpdateResponse>(deserialized);
        Assert.True(response.Done);
        Assert.True(response.Success);
        Assert.Equal("127.0.0.1", response.IPAddress);
        Assert.Equal(0, response.ErrorCount);
        Assert.Equal(0, response.ResponseCount);
        Assert.Empty(response.Responses);
        Assert.Null(response.Errors.Error1);
    }
}
