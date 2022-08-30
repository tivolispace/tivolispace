using System;
using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnUsernameChanged))]
    private string username;
    
    public TextMeshPro nametag;
    
    public override void OnStartLocalPlayer()
    {
        var playerController = gameObject.GetComponent<PlayerController>();
        playerController.enabled = true;

        CmdSetupPlayer(SystemInfo.deviceName);
    }

    [Command]
    private void CmdSetupPlayer(string _username)
    {
        username = _username;
    }
    
    private void OnUsernameChanged(string _old, string _new)
    {
        if (isLocalPlayer) return;
        nametag.text = _new;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            nametag.transform.LookAt(Camera.main.transform);
        }
    }
}