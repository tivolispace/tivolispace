using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using UnityEngine;

namespace Tivoli.Scripts.Networking
{
    public class TivoliHolepunch
    {
        private readonly UdpClient _udpClient;

        // private readonly IPEndPoint _tivoliHolepunchServer = new(Dns.GetHostAddresses("tivoli.space")[0], 7777);
        private readonly IPEndPoint _tivoliHolepunchServer = new(IPAddress.Parse("143.198.183.209"), 1234);

        public TivoliHolepunch()
        {
            _udpClient = new UdpClient();

            var timer = new Timer();
            timer.Elapsed += OnTimer;
            timer.Interval = 1000;
            timer.Enabled = true;
        }

        ~TivoliHolepunch()
        {
            _udpClient.Close();
        }

        public async Task<IPEndPoint> GetMyIpAndPort()
        {
            await _udpClient.SendAsync(new byte[] {1}, 1, _tivoliHolepunchServer);
            var result = await _udpClient.ReceiveAsync();
            var jsonString = System.Text.Encoding.UTF8.GetString(result.Buffer);
            var json = JsonConvert.DeserializeAnonymousType(jsonString, new {ip = "", port = 0});
            return new IPEndPoint(IPAddress.Parse(json.ip), json.port);
        }

        public UdpClient GetUdpClient()
        {
            return _udpClient;
        }

        public async void ListenToReceive(string note)
        {
            var result = await _udpClient.ReceiveAsync();
            Debug.Log(note+" : "+result.Buffer);
        }

        private async void OnTimer(object sender, ElapsedEventArgs e)
        {
            // _udpClient.Send(new byte[] {0}, 1, _tivoliHolepunchServer);
            var ip = await GetMyIpAndPort();
            Debug.Log("holepunch heartbeat, " + ip);
        }
    }
}