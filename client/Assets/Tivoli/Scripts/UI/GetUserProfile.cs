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
            var accountManager = DependencyManager.Instance.accountManager;

            if (accountManager.LoggedIn)
            {
                _ready = true;
                UpdateInfo();
            }
            else
            {
                accountManager.OnLoggedIn += () =>
                {
                    _ready = true;
                    UpdateInfo();
                };
            }
        }

        private async void UpdateInfo()
        {
            if (!getSelf && userId == "") return;
                
            var profile = getSelf
                ? DependencyManager.Instance.accountManager.Profile
                : await DependencyManager.Instance.accountManager.GetProfile(userId);

            var sprite = Sprite.Create(profile.ProfilePicture,
                new Rect(0, 0, profile.ProfilePicture.width, profile.ProfilePicture.height), new Vector2(0.5f, 0.5f));

            displayName.text = profile.DisplayName;
            profilePicture.sprite = sprite;
        }
    }
}