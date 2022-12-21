namespace Synology.Namecheap.Adapter.Library;

using global::Namecheap.Library.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static SynologyDdnsResponses;

/// <summary>
/// Represents an object used to adapt Namecheap DDNS responses to responses appropriate for Synology DDNS update providers.
/// </summary>
public static class NamecheapResponseAdapter
{
    /// <summary>
    /// Gets the DDNS update response appropriate for Synology DDNS update providers.
    /// </summary>
    /// <param name="namecheapResponse">The Namecheap response.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string GetSynologyResponse(NamecheapDdnsUpdateResponse namecheapResponse)
    {
        Argument.NotNull(namecheapResponse);

        if (namecheapResponse.Success)
        {
            return Good;
        }

        NamecheapDdnsUpdateError namecheapError = GetNamecheapError(namecheapResponse);
        SynologyDdnsUpdateError synologyError = GetSynologyError(namecheapError);

        if (namecheapResponse.Errors.Error1 is null)
        {
            throw new ArgumentException("No raw error information available.", nameof(namecheapResponse));
        }

        return GetSynologyResponse(synologyError, namecheapResponse.Errors.Error1);
    }

    private static NamecheapDdnsUpdateError GetNamecheapError(NamecheapDdnsUpdateResponse namecheapResponse)
    {
        return namecheapResponse.Errors.Error1 switch
        {
            NamecheapDdnsErrors.DomainNameNotFound => NamecheapDdnsUpdateError.DomainNameNotFound,
            NamecheapDdnsErrors.InvalidIP => NamecheapDdnsUpdateError.InvalidIP,
            NamecheapDdnsErrors.PasswordsDoNotMatch => NamecheapDdnsUpdateError.PasswordsDoNotMatch,
            NamecheapDdnsErrors.RecordNotFound => NamecheapDdnsUpdateError.RecordNotFound,
            _ => NamecheapDdnsUpdateError.Unknown,
        };
    }

    [ExcludeFromCodeCoverage]
    private static SynologyDdnsUpdateError GetSynologyError(NamecheapDdnsUpdateError namecheapError)
        => namecheapError switch
        {
            NamecheapDdnsUpdateError.Unknown => SynologyDdnsUpdateError.Unknown,
            NamecheapDdnsUpdateError.DomainNameNotFound => SynologyDdnsUpdateError.NoHost,
            NamecheapDdnsUpdateError.InvalidIP => SynologyDdnsUpdateError.Unknown,
            NamecheapDdnsUpdateError.PasswordsDoNotMatch => SynologyDdnsUpdateError.BadAuth,
            NamecheapDdnsUpdateError.RecordNotFound => SynologyDdnsUpdateError.NoHost,
            _ => throw new ArgumentException($"The {nameof(NamecheapDdnsUpdateError)} is not supported: '{namecheapError}'.", nameof(namecheapError)),
        };

    [ExcludeFromCodeCoverage]
    private static string GetSynologyResponse(SynologyDdnsUpdateError synologyError, string rawError)
        => synologyError switch
        {
            SynologyDdnsUpdateError.Unknown => string.Format(CultureInfo.InvariantCulture, UnknownFormatter, rawError),
            SynologyDdnsUpdateError.NoHost => NoHost,
            SynologyDdnsUpdateError.BadAuth => BadAuth,
            _ => throw new ArgumentException($"The {nameof(SynologyDdnsUpdateError)} is not supported: '{synologyError}'.", nameof(synologyError)),
        };
}
