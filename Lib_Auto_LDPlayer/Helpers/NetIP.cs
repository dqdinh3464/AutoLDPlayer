using Auto_LDPlayer.Enums;
using Leaf.xNet;
using System.Linq;
using System.Net;
using Titanium.Web.Proxy.Models;

namespace Auto_LDPlayer.Helpers
{
    public class NetIP
    {
        private ProxyServer proxyServer { get; set; }

        public string HostName { get; set; }
        public string Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public NetIPOption NetIPOption { get; set; }
        public int PortForward { get; set; } = 0;
        public string HostNamePortforward { get; set; } = "127.0.0.1";

        public NetIP(string netIP, NetIPOption netIPOption)
        {
            var arr = netIP.Split(':');
            if (arr.Length >= 2)
            {
                HostName = arr[0];
                Port = arr[1];
            }
            if (arr.Length >= 4)
            {
                Username = arr[2];
                Password = arr[3];
            }
            NetIPOption = netIPOption;
        }

        public bool StartInternalServer()
        {
            try
            {
                PortForward = ProxyServer.GetNextFreePort();
                proxyServer = new ProxyServer();
                proxyServer.HttpPort = PortForward;

                if (!string.IsNullOrEmpty(HostName) && !string.IsNullOrEmpty(Port))
                {
                    proxyServer.ExternalProxy = new ExternalProxy
                    {
                        HostName = HostName,
                        Port = int.Parse(Port),
                    };
                    proxyServer.ExternalProxy.ProxyType = NetIPOption == NetIPOption.Proxies ? ExternalProxyType.Http : ExternalProxyType.Socks5;
                    
                    if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                    {
                        proxyServer.ExternalProxy.UserName = Username;
                        proxyServer.ExternalProxy.Password = Password;
                    }
                    proxyServer.StartProxyServer();

                    return true;
                }
            }
            catch { }

            return false;
        }

        public void StopInternalServer()
        {
            proxyServer.StopProxyServer();
        }

        public string CheckIPAddress()
        {
            using (var http = new HttpRequest())
            {
                if (PortForward > 0)
                {
                    http.Proxy = ProxyClient.Parse(ProxyType.HTTP, $"{HostNamePortforward}:{PortForward}");
                }

                try
                {
                    var ipAddress = http.Get("https://api64.ipify.org/").ToString();
                    return ipAddress;
                }
                catch { }
            }

            return string.Empty;
        }

        public string GetIpHost()
        {
            try
            {
                return Dns.GetHostAddresses(HostName).FirstOrDefault().ToString();
            }
            catch { return string.Empty; }
        }
    }
}