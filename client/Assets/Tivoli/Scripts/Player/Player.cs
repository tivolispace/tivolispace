using Mirror;
using Tivoli.Scripts.Managers;
using Tivoli.Scripts.UI;
using Tivoli.Scripts.Utils;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUserIdChanged))]
        public string userId;

        public Transform nametagTransform;
        public Nametag nametag;

        public VrPlayerController vrPlayerController;
        public VrPlayerIkController vrPlayerIkController;

        private const float SEND_IK_INTERVAL = 1f / 60f;
        private float _sendIkTimer;

        public override async void OnStartLocalPlayer()
        {
            nametag.gameObject.SetActive(false);

            vrPlayerController.enabled = true;

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

        private void OnUserIdChanged(string oldUserId, string newUserId)
        {
            if (isLocalPlayer)
                return;

            nametag.gameObject.SetActive(true);
            nametag.GetComponent<Nametag>().UserId = newUserId;
        }

        [Command(channel = Channels.Unreliable, requiresAuthority = true)]
        public void SendIkData(short[] compressed)
        {
            RpcReceiveIkData(compressed);
        }

        [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
        private void RpcReceiveIkData(short[] compressed)
        {
            vrPlayerIkController.UpdateWithIkData(IkDataCompression.Decompress(compressed));
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                _sendIkTimer += Time.deltaTime;
                if (_sendIkTimer >= SEND_IK_INTERVAL)
                {
                    SendIkData(IkDataCompression.Compress(vrPlayerController.GetIkData()));
                    _sendIkTimer -= SEND_IK_INTERVAL;
                }
            }
            else
            {
                nametagTransform.LookAt(DependencyManager.Instance.UIManager.GetMainCamera().transform);
            }
        }
    }
}