using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace signalr
{
    public class SocketConnector
    {
        //private CSignalR_TA _SignalRConn;
        private HubConnection _connection;
        private string _IpAddress, _HubName = "hub";
        private int _PortNumber;
        static bool bIgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }
        public delegate void WarehouseEventHandler(Object sender, EventArgs e);
        public event WarehouseEventHandler CallAlert;
        public bool IgnoreCertificateErrorHandler()
        {
            bool rep = false;

            // Istruisce WCF ad accettare anche certificati self signed
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(bIgnoreCertificateErrorHandler);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            return true;
        }
        public SocketConnector(string ipAddress, int portNumber)
        {
            _IpAddress = ipAddress;
            _PortNumber = portNumber;
            //_SignalRConn = new CSignalR_TA(ipAddress, portNumber, hubName);
            string HubFullConnection = $"https://{_IpAddress}:{_PortNumber}/{_HubName}";

            _connection = new HubConnectionBuilder()
               .WithUrl(HubFullConnection, (opts) =>
               {
                   opts.HttpMessageHandlerFactory = (message) =>
                   {
                       if (message is HttpClientHandler clientHandler)
                           // bypass SSL certificate
                           clientHandler.ServerCertificateCustomValidationCallback +=
                               (sender, certificate, chain, sslPolicyErrors) => { return true; };
                       return message;
                   };

               })
               .Build();

            //functions listening
            //_connection.On<Warehouse_Item>("AddToTheWarehouseList", (s1) => AddToWarehouse(s1));
        }
        public async Task<bool> Connect()
        {
            try
            {
                await _connection.StartAsync();
                return _connection.State == HubConnectionState.Connected;
                //return _SignalRConn.ConnectToHub();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HubConnectionState ConnectionState()
        {
            try
            {
                return _connection.State;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Connected()
        {
            try
            {
                return _connection.State == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> Disconnect()
        {
            try
            {
                await _connection.StopAsync();
                return _connection.State == HubConnectionState.Disconnected;
                // return _SignalRConn.DisconnectFromHub();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
