namespace Test.Library.Namecheap;

/// <summary>
/// A set of mock responses.
/// </summary>
public static class MockResponseConstants
{
    /// <summary>
    /// The domain name not found response.
    /// </summary>
    public const string DomainNameNotFound = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><ErrCount>1</ErrCount><errors><Err1>Domain name not found</Err1></errors><ResponseCount>1</ResponseCount><responses><response><Description>Domain name not found</Description><ResponseNumber>316153</ResponseNumber><ResponseString>Validation error; not found; domain name(s)</ResponseString></response></responses><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";

    /// <summary>
    /// The invalid ip response.
    /// </summary>
    public const string InvalidIp = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><ErrCount>1</ErrCount><errors><Err1>Invalid IP</Err1></errors><ResponseCount>1</ResponseCount><responses><response><Description>Invalid IP</Description><ResponseNumber>304156</ResponseNumber><ResponseString>Validation error; invalid ; IP Address</ResponseString></response></responses><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";

    /// <summary>
    /// The passwords do not match response.
    /// </summary>
    public const string PasswordsDoNotMatch = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><ErrCount>1</ErrCount><errors><Err1>Passwords do not match</Err1></errors><ResponseCount>1</ResponseCount><responses><response><Description>Passwords do not match</Description><ResponseNumber>304156</ResponseNumber><ResponseString>Validation error; invalid ; password</ResponseString></response></responses><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";

    /// <summary>
    /// The record not found response.
    /// </summary>
    public const string RecordNotFound = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><ErrCount>1</ErrCount><errors><Err1>No Records updated. A record not Found;</Err1></errors><ResponseCount>1</ResponseCount><responses><response><Description>No Records updated. A record not Found;</Description><ResponseNumber>380091</ResponseNumber><ResponseString>No updates; A record not Found;</ResponseString></response></responses><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";

    /// <summary>
    /// The success response.
    /// </summary>
    public const string Success = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><IP>127.0.0.1</IP><ErrCount>0</ErrCount><errors /><ResponseCount>0</ResponseCount><responses /><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";

    /// <summary>
    /// The unexpected error response.
    /// </summary>
    public const string UnexpectedError = "<?xml version=\"1.0\" encoding=\"utf-16\"?><interface-response><Command>SETDNSHOST</Command><Language>eng</Language><ErrCount>1</ErrCount><errors><Err1>Some unexpected error occurred;</Err1></errors><ResponseCount>1</ResponseCount><responses><response><Description>No Records updated. Something happened;</Description><ResponseNumber>0</ResponseNumber><ResponseString>No updates; Something happened;</ResponseString></response></responses><Done>true</Done><debug><![CDATA[]]></debug></interface-response>";
}
