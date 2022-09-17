using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Nametag : MonoBehaviour
{
    public Image _profilePicture;
    public Image _textBackground;

    private void Awake()
    {
        StartCoroutine(LoadImage());
    }

    private static Texture2D FastGaussianBlur(Texture2D input)
    {
        const float tau = Mathf.PI * 2f;

        const float directions = 16f;
        const float quality = 8f; // default is 3f
        const float size = 8f;

        var radiusX = size / input.width;
        var radiusY = size / input.height;

        var inputColors = input.GetPixels();
        var inputPixels = new Vector3[inputColors.Length];
        for (var i = 0; i < inputPixels.Length; i++)
        {
            inputPixels[i] = new Vector3(inputColors[i].r / 255f, inputColors[i].g / 255f, inputColors[i].b / 255f);
        }

        var outputPixels = new Vector3[input.width * input.height];

        for (var y = 0; y < input.height; y++)
        {
            for (var x = 0; x < input.width; x++)
            {
                var uvX = (float) x / input.width;
                var uvY = (float) y / input.height;

                var pixelIndex = y * input.width + x;
                outputPixels[pixelIndex] = inputPixels[pixelIndex];

                for (var d = 0f; d < tau; d += tau / directions)
                {
                    for (var i = 1f / quality; i <= 1f; i += 1f / quality)
                    {
                        var sampleNormX = uvX + Mathf.Cos(d) * radiusX * i;
                        var sampleX = Mathf.Clamp(Mathf.RoundToInt(sampleNormX * input.width), 0, input.width - 1);

                        var sampleNormY = uvY + Mathf.Sin(d) * radiusY * i;
                        var sampleY = Mathf.Clamp(Mathf.RoundToInt(sampleNormY * input.height), 0, input.height - 1);

                        var sampleColor = inputPixels[sampleY * input.width + sampleX];
                        outputPixels[pixelIndex] += sampleColor;
                    }
                }

                outputPixels[pixelIndex] /= quality * directions;
            }
        }

        var output = new Texture2D(input.width, input.height, input.format, false);

        var outputColors = new Color[outputPixels.Length];
        for (var i = 0; i < inputPixels.Length; i++)
        {
            outputColors[i] = new Color(
                outputPixels[i].x * 255,
                outputPixels[i].y * 255,
                outputPixels[i].z * 255
            );
        }

        output.SetPixels(outputColors);
        output.Apply();

        return output;
    }

    private IEnumerator LoadImage()
    {
        const string imageUrl =
            "https://cdn.discordapp.com/avatars/72139729285427200/030849ff09ed741ce2d3c7ac8b3cb426.png?size=128";

        var req = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(req.error);
            Debug.Log(req.downloadHandler.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture) req.downloadHandler).texture;
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _profilePicture.sprite = sprite;

            var blurredTexture = FastGaussianBlur(texture);
            var blurredSprite = Sprite.Create(blurredTexture,
                new Rect(0, 0, blurredTexture.width, blurredTexture.height), new Vector2(0.5f, 0.5f));
            _textBackground.sprite = blurredSprite;
        }
    }
}