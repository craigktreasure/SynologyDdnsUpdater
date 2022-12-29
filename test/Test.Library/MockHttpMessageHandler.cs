namespace Test.Library;

/// <summary>
/// Class MockHttpMessageHandler.
/// Implements the <see cref="HttpMessageHandler" />
/// </summary>
/// <seealso cref="HttpMessageHandler" />
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> sendAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
    /// </summary>
    /// <param name="sendAction">The send action.</param>
    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendAction)
        => this.sendAction = sendAction;

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(this.sendAction(request));
}
