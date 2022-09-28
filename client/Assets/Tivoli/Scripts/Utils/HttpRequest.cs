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
        
        public static Task<(UnityWebRequest, Dictionary<string, string>)> Simple(Dictionary<string, string> reqData,
            Dictionary<string, string> data = null)
        {
            var cs = new TaskCompletionSource<(UnityWebRequest, Dictionary<string, string>)>();

            var req = new UnityWebRequest();
            req.method = reqData.GetValueOrDefault("method", "GET");
            req.url = reqData.GetValueOrDefault("url", "");

            req.SetRequestHeader("User-Agent", "TivoliSpace/" + Application.version);
            req.downloadHandler = new DownloadHandlerBuffer();
            
            if (reqData.TryGetValue("auth", out var bearer))
            {
                req.SetRequestHeader("Authorization", "Bearer " + bearer);
            }

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                req.uploadHandler.contentType = "application/json";
            }

            var reqAsync = req.SendWebRequest();
            reqAsync.completed += _ =>
            {
                var result = req.result == UnityWebRequest.Result.Success
                    ? JsonConvert.DeserializeObject<Dictionary<string, string>>(req.downloadHandler.text)
                    : new Dictionary<string, string>();

                cs.SetResult((req, result));
                req.Dispose();
            };

            return cs.Task;
        }
    }
}