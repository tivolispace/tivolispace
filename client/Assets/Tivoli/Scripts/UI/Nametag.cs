using Tivoli.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class Nametag : MonoBehaviour
    {
        private string _userId;
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                Refresh();
            }
        }
    
        public Image profilePicture;
        public Image textBackground;
        public TextMeshProUGUI nameText;

        private async void Refresh()
        {
            var profile = await DependencyManager.Instance.accountManager.GetProfile(_userId);
            var displayName = profile.displayName;
            if (displayName == null) return;

            var size = nameText.GetPreferredValues(displayName);

            // update name tag width
            const float imageWidth = 5f;
            const float padding = 5f;

            var textWidth = size.x + padding;
            if (textWidth < 15) textWidth = 15;

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(textWidth + imageWidth, rectTransform.sizeDelta.y);

            // set image height to width so background covers
            textBackground.rectTransform.sizeDelta = new Vector2(0, textWidth);

            // set text
            nameText.text = displayName;

            // update profile picture
            SetImage(profile.profilePicture);
        }

        private static Texture2D GpuScale(Texture2D src, int width, int height, FilterMode filterMode)
        {
            var texture = new Texture2D(src.width, src.height);
            texture.SetPixels(src.GetPixels());
            texture.filterMode = filterMode;
            texture.Apply(true);

            var rtt = new RenderTexture(width, height, 32);
            Graphics.SetRenderTarget(rtt);
            GL.LoadPixelMatrix(0, 1, 1, 0);
            GL.Clear(true, true, new Color(0, 0, 0, 0));

            Graphics.DrawTexture(new Rect(0, 0, 1, 1), texture);

            texture.Reinitialize(width, height);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0, true);
            texture.Apply(true);

            return texture;
        }

        private static Texture2D BlurProfilePicture(Texture2D src)
        {
            var input = GpuScale(src, 128, 128, FilterMode.Bilinear);

            const float tau = Mathf.PI * 2f;

            const float directions = 16f;
            const float quality = 8f; // default is 3f
            const float size = 16f;

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

        private void SetImage(Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            profilePicture.sprite = sprite;

            var blurredTexture = BlurProfilePicture(texture);
            var blurredSprite = Sprite.Create(blurredTexture,
                new Rect(0, 0, blurredTexture.width, blurredTexture.height), new Vector2(0.5f, 0.5f));
            textBackground.sprite = blurredSprite;
        }
    }
}