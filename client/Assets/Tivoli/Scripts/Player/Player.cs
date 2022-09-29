using Mirror;
using Tivoli.Scripts.UI;
using Tivoli.Scripts.Managers;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUserIdChanged))]
        public string userId;

        public Transform nametagTransform;
        public Nametag nametag;

        public override async void OnStartLocalPlayer()
        {
            await DependencyManager.Instance.accountManager.WhenLoggedIn();
            nametag.gameObject.SetActive(false);
            CmdSetupPlayer(DependencyManager.Instance.accountManager.Profile.Id);
        }

        public override void OnStopLocalPlayer() { }

        [Command]
        private void CmdSetupPlayer(string newUserId)
        {
            userId = newUserId;
        }

        private void OnUserIdChanged(string @old, string @new)
        {
            if (isLocalPlayer)
                return;

            nametag.gameObject.SetActive(true);
            nametag.GetComponent<Nametag>().UserId = @new;
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                nametagTransform.LookAt(Camera.main.transform);
            }
        }
    }
}
