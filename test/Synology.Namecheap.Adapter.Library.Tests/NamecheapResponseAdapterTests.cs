namespace Synology.Namecheap.Adapter.Library.Tests;

using global::Namecheap.Library.Models;

public class NamecheapResponseAdapterTests
{
    [Fact]
    public void GetSynologyResponse_DomainNameNotFound()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse(NamecheapDdnsErrors.DomainNameNotFound);

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal(SynologyDdnsResponses.NoHost, response);
    }

    [Fact]
    public void GetSynologyResponse_Error1Null()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse(null);

        // Act
        Assert.Throws<ArgumentException>(nameof(namecheapResponse), () => NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse));
    }

    [Fact]
    public void GetSynologyResponse_InvalidIP()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse(NamecheapDdnsErrors.InvalidIP);

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal("911 [Invalid IP]", response);
    }

    [Fact]
    public void GetSynologyResponse_PasswordsDoNotMatch()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse(NamecheapDdnsErrors.PasswordsDoNotMatch);

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal(SynologyDdnsResponses.BadAuth, response);
    }

    [Fact]
    public void GetSynologyResponse_RecordNotFound()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse(NamecheapDdnsErrors.RecordNotFound);

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal(SynologyDdnsResponses.NoHost, response);
    }

    [Fact]
    public void GetSynologyResponse_Success()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = new()
        {
            Command = "SETDNSHOST",
            Errors = new(),
            Language = "eng",
            Responses = [],
            IPAddress = "127.0.0.1",
        };

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal(SynologyDdnsResponses.Good, response);
    }

    [Fact]
    public void GetSynologyResponse_UnknownError()
    {
        // Arrange
        NamecheapDdnsUpdateResponse namecheapResponse = GetUpdateResponse("Some unexpected error");

        // Act
        string response = NamecheapResponseAdapter.GetSynologyResponse(namecheapResponse);

        // Assert
        Assert.Equal("911 [Some unexpected error]", response);
    }

    private static NamecheapDdnsUpdateResponse GetUpdateResponse(string? error)
        => new()
        {
            Command = "SETDNSHOST",
            ErrorCount = 1,
            Errors = new()
            {
                Error1 = error,
            },
            Language = "eng",
            ResponseCount = 1,
            Responses = [],
        };
}
