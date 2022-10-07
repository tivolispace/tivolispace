using Mirror;
using Tivoli.Scripts.Managers;
using Tivoli.Scripts.UI;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUserIdChanged))]
        public string userId;

        public Transform nametagTransform;
        public Nametag nametag;

        // private XROrigin _xrOrigin;

        public override async void OnStartLocalPlayer()
        {
            nametag.gameObject.SetActive(false);

            // get xr origin and parent player to it
            // _xrOrigin = DependencyManager.Instance.UIManager.GetXrOrigin();
            // var currentPosition = transform.position;
            // var currentRotation = transform.rotation;
            // transform.SetParent(_xrOrigin.transform);
            // transform.position = Vector3.zero;
            // transform.rotation = Quaternion.identity;
            // _xrOrigin.transform.position = currentPosition;
            // _xrOrigin.transform.rotation = currentRotation;

            await DependencyManager.Instance.AccountManager.WhenLoggedIn();
            
            CmdSetupPlayer(DependencyManager.Instance.AccountManager.Profile.id);
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
            if (isLocalPlayer)
                return;

            nametag.gameObject.SetActive(true);
            nametag.GetComponent<Nametag>().UserId = @new;
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                
            } else {
                nametagTransform.LookAt(DependencyManager.Instance.UIManager.GetMainCamera().transform);
            }
        }
    }
}