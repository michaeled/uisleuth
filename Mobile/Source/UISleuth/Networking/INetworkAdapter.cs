namespace UISleuth.Networking
{
    /// <summary>
    /// .NET PCL's do not have the ability to query a network adapter for an IP address.
    /// UI Sleuth's designers and clients should implement this interface.
    /// </summary>
    internal interface INetworkAdapter
    {
        string[] GetIpAddresses();
    }
}