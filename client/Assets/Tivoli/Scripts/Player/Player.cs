using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUsernameChanged))]
        public SteamId steamId;

        public Transform nametagTransform;
        public Nametag nametag;

        public override void OnStartLocalPlayer()
        {
            CmdSetupPlayer(DependencyManager.Instance.steamManager.GetMySteamID().Value);
            nametag.gameObject.SetActive(false);
        }

        public override void OnStopLocalPlayer()
        {
        }

        [Command]
        private void CmdSetupPlayer(ulong steamIdUlong)
        {
            steamId = steamIdUlong;
        }
        
        private void OnUsernameChanged(SteamId @old, SteamId @new)
        {
            if (isLocalPlayer) return;
            
            nametag.gameObject.SetActive(true);
            nametag.SetSteamId(@new);
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