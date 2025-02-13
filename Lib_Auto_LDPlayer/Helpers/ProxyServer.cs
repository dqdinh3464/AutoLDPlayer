using System.Net.Sockets;
using System.Net;
using Titanium.Web.Proxy.Models;

namespace Auto_LDPlayer.Helpers
{
    public class ProxyServer
    {
        private Titanium.Web.Proxy.ProxyServer proxyServer;

        public ExternalProxy ExternalProxy { get; set; } = null;
        public IPEndPoint IPEndPoint { get; set; } = null;
        public int HttpPort { get; set; } = -1;
        public int SocksPort { get; set; } = -1;

        public bool StartProxyServer()
        {
            try
            {
                proxyServer = new Titanium.Web.Proxy.ProxyServer();
                if (ExternalProxy != null)
                {
                    proxyServer.UpStreamHttpProxy = ExternalProxy;
                    proxyServer.UpStreamHttpsProxy = ExternalProxy;
                }
                if (IPEndPoint != null)
                {
                    proxyServer.UpStreamEndPoint = IPEndPoint;
                }

                if (HttpPort != -1)
                {
                    var httpEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, HttpPort, false);
                    proxyServer.AddEndPoint(httpEndPoint);
                }
                else if (SocksPort != -1)
                {
                    var socksEndPoint = new SocksProxyEndPoint(IPAddress.Any, SocksPort, false);
                    proxyServer.AddEndPoint(socksEndPoint);
                }
                proxyServer.Start();

                return true;
            }
            catch { }
            return false;
        }
        public bool StopProxyServer()
        {
            try
            {
                proxyServer.RestoreOriginalProxySettings();
                proxyServer.Stop();

                return true;
            }
            catch { }

            return false;
        }

        public static int GetNextFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }
    }
}
