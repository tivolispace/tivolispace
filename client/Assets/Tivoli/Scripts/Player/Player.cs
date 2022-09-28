using Mirror;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUserIdChanged))]
        public string userId;

        public Transform nametagTransform;
        public Nametag nametag;

        public override void OnStartLocalPlayer()
        {
            if (DependencyManager.Instance.accountManager.LoggedIn)
            {
                CmdSetupPlayer(DependencyManager.Instance.accountManager.Profile.Id);
            }
            else
            {
                DependencyManager.Instance.accountManager.OnLoggedIn += () =>
                {
                    CmdSetupPlayer(DependencyManager.Instance.accountManager.Profile.Id);
                };
            }
            nametag.gameObject.SetActive(false);
        }

        public override void OnStopLocalPlayer()
        {
        }

        [Command]
        private void CmdSetupPlayer(string newUserId)
        {
            userId = newUserId;
        }
        
        private void OnUserIdChanged(string @old, string @new)
        {
            if (isLocalPlayer) return;
            
            nametag.gameObject.SetActive(true);
            nametag.SetUserId(@new);
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