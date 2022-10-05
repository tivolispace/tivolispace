using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using Random = System.Random;
using Timer = System.Timers.Timer;

namespace Tivoli.Scripts.Networking
{
    public class TivoliHolepunch
    {
        // private readonly IPEndPoint _tivoliHolepunchServer = new(Dns.GetHostAddresses("tivoli.space")[0], 5971);
        private readonly IPEndPoint _tivoliHolepunchServer = new(IPAddress.Parse("142.93.250.50"), 5971);

        private readonly UdpClient _udpClient;
        private readonly Timer _timer;

        private bool _hosting;
        private readonly CancellationTokenSource _hostingCts = new();

        public TivoliHolepunch()
        {
            _udpClient = new UdpClient();

            _timer = new Timer();
            _timer.Elapsed += OnTimer;
            _timer.Interval = 1000;
            _timer.Enabled = true;
        }

        public void OnDestroy()
        {
            _timer.Enabled = false;
            _timer.Stop();
            _timer.Dispose();

            _hosting = false;

            _udpClient.Close();
        }

        private static IPEndPoint HolepunchServerBytesToIp(byte[] result)
        {
            var data = Encoding.UTF8.GetString(result);
            var address = data.Split(' ');
            var endpoint = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
            return endpoint;
        }

        private void SendGarbageToHolepunch(IPEndPoint endpoint)
        {
            var i = 0;
            // var random = new Random();
            // var bytes = new byte[16];

            var timer = new Timer();
            timer.Elapsed += (_, _) =>
            {
                i++;
                
                // random.NextBytes(bytes);
                // _udpClient.SendAsync(bytes, bytes.Length, endpoint);

                _udpClient.SendAsync(new byte[] { 0 }, 1, endpoint);
                
                Debug.Log("garbage sent");

                if (i < 10) return;
                timer.Enabled = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Interval = 25;
            timer.Enabled = true;
        }

        // doing this for now because its really hard to use the same port
        // but will conflict on symmetrical nats which arent working anyway
        private IPEndPoint EndpointPlusOnePort(IPEndPoint endpoint)
        {
            return new IPEndPoint(endpoint.Address, endpoint.Port + 1);
        }

        private void StartHostReceivingClients()
        {
            if (!_hosting) return;
            Task.Run(async () =>
            {
                while (_hosting)
                {
                    try
                    {
                        var result = await _udpClient.ReceiveAsync();
                        if (!Equals(result.RemoteEndPoint, _tivoliHolepunchServer)) continue;

                        var endpoint = HolepunchServerBytesToIp(result.Buffer);
                        Debug.Log("holepunch got, client: " + endpoint);
                        endpoint = EndpointPlusOnePort(endpoint);
                        Debug.Log("holepunch sending garbage to: " + endpoint);
                        SendGarbageToHolepunch(endpoint);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogWarning("failed to receive from heartbeat\n" + exception);
                    }
                }
            }, _hostingCts.Token);
        }
        
        private static string GetLocalIpAddress()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint?.Address.ToString();
        }

        public async Task<IPEndPoint> StartHost(string instanceId)
        {
            var message = "host " + instanceId + " " + GetLocalIpAddress();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, _tivoliHolepunchServer);
            Debug.Log("holepunch sent: " + message);

            var result = await _udpClient.ReceiveAsync();
            var endpoint = HolepunchServerBytesToIp(result.Buffer);
            Debug.Log("holepunch got: host " + endpoint);
            endpoint = EndpointPlusOnePort(endpoint);
            Debug.Log("holepunch: host instead use port: " + endpoint.Port);

            _hosting = true;
            StartHostReceivingClients();
            

            return endpoint;
        }

        public void StopHost()
        {
            _hosting = false;
            _hostingCts.Cancel();

            // should reset better but its just proof of concept for now
        }

        public async Task<(IPEndPoint, IPEndPoint)> StartClient(string instanceId)
        {
            var message = "client " + instanceId + " " + GetLocalIpAddress();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, _tivoliHolepunchServer);
            Debug.Log("holepunch sent: " + message);

            var myResult = await _udpClient.ReceiveAsync();
            var myEndpoint = HolepunchServerBytesToIp(myResult.Buffer);
            Debug.Log("holepunch got: client " + myEndpoint);
            myEndpoint = EndpointPlusOnePort(myEndpoint);
            Debug.Log("holepunch: client instead use port: " + myEndpoint.Port);
            
            var hostResult = await _udpClient.ReceiveAsync();
            var hostEndpoint = HolepunchServerBytesToIp(hostResult.Buffer);
            Debug.Log("holepunch got: host " + hostEndpoint);
            hostEndpoint = EndpointPlusOnePort(hostEndpoint);
            Debug.Log("holepunch: host instead use port: " + hostEndpoint.Port);
            
            Debug.Log("holepunch sending garbage to: " + hostEndpoint);
            SendGarbageToHolepunch(hostEndpoint);

            return (myEndpoint, hostEndpoint);
        }

        public void StopClient()
        {
        }

        private async void OnTimer(object sender, ElapsedEventArgs e)
        {
            await _udpClient.SendAsync(new byte[] {0}, 1, _tivoliHolepunchServer);
            Debug.Log("holepunch sent: heartbeat");
        }
    }
}