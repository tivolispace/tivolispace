﻿using Mirror;
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

        public override void OnStartLocalPlayer()
        {
            CmdSetupPlayer(SystemInfo.deviceName + new Random().Next(0, 999));
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
            nametagTransform.GetComponentInChildren<TextMeshPro>().text = @new;
        }
    
        private void Update()
        {
            if (!isLocalPlayer)
            {
                nametagTransform.transform.LookAt(Camera.main.transform);
            }
        }
    }
}