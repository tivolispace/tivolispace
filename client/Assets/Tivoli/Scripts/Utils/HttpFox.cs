using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Tivoli.Scripts.Utils
{
    public class HttpFox // so fast lmao
    {
        // inspired by https://flurl.dev/docs/fluent-http/
        // thank you

        private UnityWebRequest _req;
        private bool _disposed;
        
        public HttpFox(string url, string method = "GET")
        {
            _req = new UnityWebRequest();
            _req.url = url;
            _req.method = method;
            _req.SetRequestHeader("User-Agent", "TivoliSpace/" + Application.version);
        }

        private void Dispose()
        {
            if (_disposed) return;
            _req?.Dispose();
            _disposed = true;
        }
        
        ~HttpFox()
        {
            Dispose();
        }

        public HttpFox WithHeader(string name, string value)
        {
            _req.SetRequestHeader(name, value);
            return this;
        }

        public HttpFox WithBearerAuth(string bearer)
        {
            _req.SetRequestHeader("Authorization", "Bearer " + bearer);
            return this;
        }
        
        public HttpFox WithJson(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            _req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonString));
            _req.uploadHandler.contentType = "application/json";
            return this;
        }

        private Task<object> SendRequest()
        {
            var cs = new TaskCompletionSource<object>();
            
            var reqAsync = _req.SendWebRequest();
            reqAsync.completed += _ =>
            {
                cs.SetResult(null);
            };

            return cs.Task;
        }

        public async Task ReceiveNothing()
        {
            await SendRequest();
            Dispose();
        }
        
        public async Task<(string, string)> ReceiveString()
        {
            var downloadHandler = new DownloadHandlerBuffer();
            _req.downloadHandler = downloadHandler;
            await SendRequest();
            var text = downloadHandler.text;
            var error = _req.result == UnityWebRequest.Result.Success ? null : _req.error;
            Dispose();
            return (text, error);
        }

        public async Task<(Texture2D, string)> ReceiveTexture()
        {
            var downloadHandler = new DownloadHandlerTexture();
            _req.downloadHandler = downloadHandler;
            await SendRequest();
            var texture = downloadHandler.texture;
            var error = _req.result == UnityWebRequest.Result.Success ? null : _req.error;
            Dispose();
            return (texture, error);
        }

        public async Task<(T, string)> ReceiveJson<T>()
        {
            var (jsonString, error) = await ReceiveString();
            // return (JsonUtility.FromJson<T>(jsonString), error);
            return (JsonConvert.DeserializeObject<T>(jsonString), error);
        }
        
        // will populate anonymously, optionally pass a class
        // var response = ...ReceiveJson(new {
        //     value = "",
        //     nested = new {
        //         value = false
        //     }
        // });
        
        public async Task<(T, string)> ReceiveJson<T>(T type)
        {
            var (jsonString, error) = await ReceiveString();
            return (JsonConvert.DeserializeObject<T>(jsonString), error);
        }
    }
}