using Mirror;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUsernameChanged))]
        public string username;

        public Transform nametagTransform;
        public Nametag nametag;

        public override void OnStartLocalPlayer()
        {
            CmdSetupPlayer(DependencyManager.Instance.accountManager.GetMyUsername());
            nametag.gameObject.SetActive(false);
        }

        public override void OnStopLocalPlayer()
        {
        }

        [Command]
        private void CmdSetupPlayer(string newUsername)
        {
            username = newUsername;
        }
        
        private void OnUsernameChanged(string @old, string @new)
        {
            if (isLocalPlayer) return;
            
            nametag.gameObject.SetActive(true);
            nametag.SetUsername(@new);
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