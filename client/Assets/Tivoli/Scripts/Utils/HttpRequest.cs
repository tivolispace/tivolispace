using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Tivoli.Scripts.Utils
{
    public static class HttpRequest
    {
        // TODO: currying!!!!!!!!
        // TODO: OMG SERIOUSLY THIS NEEDS TO BE BETTER

        public static Task<(UnityWebRequest, Dictionary<string, string>)> Simple(Dictionary<string, string> reqData,
            string jsonString = null, bool jsonSerialize = true)
        {
            var cs = new TaskCompletionSource<(UnityWebRequest, Dictionary<string, string>)>();

            var req = new UnityWebRequest();
            req.method = reqData.GetValueOrDefault("method", "GET");
            req.url = reqData.GetValueOrDefault("url", "");

            req.SetRequestHeader("User-Agent", "TivoliSpace/" + Application.version);
            req.downloadHandler = new DownloadHandlerBuffer();

            if (reqData.TryGetValue("auth", out var bearer) && bearer != "")
            {
                req.SetRequestHeader("Authorization", "Bearer " + bearer);
            }

            if (jsonString != null)
            {
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonString));
                req.uploadHandler.contentType = "application/json";
            }

            var reqAsync = req.SendWebRequest();
            reqAsync.completed += _ =>
            {
                var result = new Dictionary<string, string>();
                if (jsonSerialize && req.result == UnityWebRequest.Result.Success)
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(req.downloadHandler.text);
                }

                cs.SetResult((req, result));
                req.Dispose();
            };

            return cs.Task;
        }

        public static Task<Texture2D> Texture(string url)
        {
            var cs = new TaskCompletionSource<Texture2D>();

            var req = new UnityWebRequest();
            req.method = "GET";
            req.url = url;

            req.SetRequestHeader("User-Agent", "TivoliSpace/" + Application.version);

            var downloadHandler = new DownloadHandlerTexture();
            req.downloadHandler = downloadHandler;

            var reqAsync = req.SendWebRequest();
            reqAsync.completed += _ =>
            {
                cs.SetResult(downloadHandler.texture);
                req.Dispose();
            };

            return cs.Task;
        }
    }
}