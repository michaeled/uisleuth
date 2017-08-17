using System;

namespace UISleuth.Networking
{
    internal class ServiceEndpoint
    {
        // when the debugger is attached, we want a higher value so we're not disconnected.
        public static readonly TimeSpan PingWaitTime = TimeSpan.FromMinutes(25);

        public const short MobilePort = 9099;
        public const short DesktopPort = 9089;
        public const string MobilePath = "/";
        public const string MobileHeartbeatPath = "/heartbeat";


        public static bool IsValidDnsLabel(string host)
        {
            return Uri.CheckHostName(host) != UriHostNameType.Unknown;
        }


        public static bool IsValidPort(short port)
        {
            return port >= 1024;
        }


        public static string FormatAddress(string host, short port, string path = null)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }

            if (!IsValidDnsLabel(host))
            {
                return null;
            }

            var segment = "/";

            if (path != null)
            {
                segment = path;
            }

            return $"ws://{host}:{port}{segment}";
        }
    }
}