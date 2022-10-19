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

        private const float SEND_IK_INTERVAL = 1f / 30f;
        private float _sendIkTimer;

        private IkDataNetworkCompanion _ikDataNetworkCompanion = new();

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
        private void SendIkData(short[] compressed)
        {
            RpcReceiveIkData(compressed);
        }

        [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
        private void RpcReceiveIkData(short[] compressed)
        {
            _ikDataNetworkCompanion.ReceiveCompressed(compressed);
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                _sendIkTimer += Time.deltaTime;
                if (_sendIkTimer >= SEND_IK_INTERVAL)
                {
                    SendIkData(_ikDataNetworkCompanion.Compress(vrPlayerController.GetIkData()));
                    _sendIkTimer -= SEND_IK_INTERVAL;
                }
            }
            else
            {
                var ikData = _ikDataNetworkCompanion.Update();
                if (ikData != null)
                {
                    vrPlayerIkController.UpdateWithIkData(ikData);
                }

                nametagTransform.LookAt(DependencyManager.Instance.UIManager.GetMainCamera().transform);
            }
        }
    }
}