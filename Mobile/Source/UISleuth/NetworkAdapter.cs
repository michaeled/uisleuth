using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UISleuth.Networking;

namespace UISleuth.Platform
{
    internal class NetworkAdapter : INetworkAdapter
    {
        public string[] GetIpAddresses()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = new List<string>();

            foreach (var ip in host.AddressList)
            {
                if (IPAddress.IsLoopback(ip))
                {
                    continue;
                }

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }

            return ips.ToArray();
        }
    }
}