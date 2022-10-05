using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using Random = System.Random;

namespace Tivoli.Scripts.Networking
{
    public class TivoliHolepunch
    {
        // private readonly IPEndPoint _tivoliHolepunchServer = new(Dns.GetHostAddresses("tivoli.space")[0], 5971);
        private readonly IPEndPoint _tivoliHolepunchServer = new(IPAddress.Parse("142.93.250.50"), 5971);

        private readonly UdpClient _udpClient;
        private readonly Timer _timer;

        private bool _hosting;

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

        private static IPEndPoint UdpResultToIPEndPoint(byte[] result)
        {
            var data = Encoding.UTF8.GetString(result);
            var address = data.Split(' ');
            var endpoint = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
            return endpoint;
        }

        private void SendGarbageToHolepunch(IPEndPoint endpoint)
        {
            var i = 0;
            var random = new Random();
            var bytes = new byte[16];

            var timer = new Timer();
            timer.Elapsed += (_, _) =>
            {
                i++;
                
                random.NextBytes(bytes);
                _udpClient.SendAsync(bytes, bytes.Length, endpoint);
                Debug.Log("garbage sent");

                if (i < 10) return;
                timer.Enabled = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Interval = 25;
            timer.Enabled = true;
        }

        private void HostReceivingClientsLoop()
        {
            if (!_hosting) return;

            var holepunchServer = _tivoliHolepunchServer;
            _udpClient.BeginReceive(asyncResult =>
            {
                try
                {
                    var result = _udpClient.EndReceive(asyncResult, ref holepunchServer);
                    var endpoint = UdpResultToIPEndPoint(result);

                    Debug.Log("holepunch got, client: " + endpoint);
                    Debug.Log("holepunch sending garbage to: " + endpoint);
                    SendGarbageToHolepunch(endpoint);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("failed to receive from heartbeat\n" + exception);
                }

                HostReceivingClientsLoop();
            }, this);
        }

        public async Task<IPEndPoint> StartHost(string instanceId)
        {
            var message = Encoding.UTF8.GetBytes("host " + instanceId);
            await _udpClient.SendAsync(message, message.Length, _tivoliHolepunchServer);
            Debug.Log("holepunch sent: host " + instanceId);

            var result = await _udpClient.ReceiveAsync();
            var endpoint = UdpResultToIPEndPoint(result.Buffer);
            Debug.Log("holepunch got: host " + endpoint);

            _hosting = true;
            HostReceivingClientsLoop();

            return endpoint;
        }

        public void StopHost()
        {
            _hosting = false;

            // should reset better but its just proof of concept for now
        }

        public async Task<(IPEndPoint, IPEndPoint)> StartClient(string instanceId)
        {
            var message = Encoding.UTF8.GetBytes("client " + instanceId);
            await _udpClient.SendAsync(message, message.Length, _tivoliHolepunchServer);
            Debug.Log("holepunch sent: client " + instanceId);

            var myResult = await _udpClient.ReceiveAsync();
            var myEndpoint = UdpResultToIPEndPoint(myResult.Buffer);
            Debug.Log("holepunch got: client " + myEndpoint);
            
            var hostResult = await _udpClient.ReceiveAsync();
            var hostEndpoint = UdpResultToIPEndPoint(hostResult.Buffer);
            Debug.Log("holepunch got: host " + hostEndpoint);
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