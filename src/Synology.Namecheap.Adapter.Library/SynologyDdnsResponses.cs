namespace Synology.Namecheap.Adapter.Library;

using System.Text;

internal static class SynologyDdnsResponses
{
    public const string BadAuth = "badauth";

    public const string Good = "good";

    public const string NoHost = "nohost";

    public static readonly CompositeFormat UnknownFormatter = CompositeFormat.Parse("911 [{0}]");
}
