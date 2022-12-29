namespace Synology.Namecheap.Adapter.Library;

internal enum NamecheapDdnsUpdateError
{
    Unknown,

    DomainNameNotFound,

    InvalidIP,

    PasswordsDoNotMatch,

    RecordNotFound,
}
