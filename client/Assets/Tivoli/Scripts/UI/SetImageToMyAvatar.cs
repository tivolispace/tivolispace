using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class SetImageToMyAvatar: MonoBehaviour
    {
        private void Start()
        {
            var texture = DependencyManager.Instance.steamManager.GetMyAvatar();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            GetComponent<Image>().sprite = sprite;
        }
    }
}