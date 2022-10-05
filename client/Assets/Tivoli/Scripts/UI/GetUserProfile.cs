using Tivoli.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class GetUserProfile : MonoBehaviour
    {
        public TextMeshProUGUI displayName;
        public Image profilePicture;

        private bool _getSelf;
        public bool getSelf
        {
            get => _getSelf;
            set {
                _getSelf = value;
                UpdateInfo();
            }
        }
        
        private string _userId;
        public string userId
        {
            get => _userId;
            set {
                _userId = value;
                UpdateInfo();
            }
        }

        private bool _ready;

        private void Start()
        {
            UpdateInfo();
        }

        private async void UpdateInfo()
        {
            await DependencyManager.Instance.AccountManager.WhenLoggedIn();
            
            if (!getSelf && userId == "") return;
                
            var profile = getSelf
                ? DependencyManager.Instance.AccountManager.Profile
                : await DependencyManager.Instance.AccountManager.GetProfile(userId);

            var sprite = Sprite.Create(profile.profilePicture,
                new Rect(0, 0, profile.profilePicture.width, profile.profilePicture.height), new Vector2(0.5f, 0.5f));

            displayName.text = profile.displayName;
            profilePicture.sprite = sprite;
        }
    }
}