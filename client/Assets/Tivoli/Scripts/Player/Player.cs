using Mirror;
using Tivoli.Scripts.Managers;
using Tivoli.Scripts.UI;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    [RequireComponent(typeof(VrPlayerController))]
    [RequireComponent(typeof(VrPlayerIkController))]
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUserIdChanged))]
        public string userId;

        public Transform nametagTransform;
        public Nametag nametag;

        private VrPlayerController _vrPlayerController;
        private VrPlayerIkController _vrPlayerIkController;

        [SyncVar(hook = nameof(OnIkData))]
        private VrPlayerController.IkData _ikData;

        public void Awake()
        {
            _vrPlayerController = GetComponent<VrPlayerController>();
            _vrPlayerIkController = GetComponent<VrPlayerIkController>();
        }

        public override async void OnStartLocalPlayer()
        {
            nametag.gameObject.SetActive(false);

            _vrPlayerController.enabled = true;

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

        private void OnIkData(VrPlayerController.IkData oldIkData, VrPlayerController.IkData newIkData)
        {
            if (isLocalPlayer) return;

            _vrPlayerIkController.UpdateWithIkData(newIkData);
        }

        private void Update()
        {
            if (isLocalPlayer) return;
            
            nametagTransform.LookAt(DependencyManager.Instance.UIManager.GetMainCamera().transform);
        }

        private void LateUpdate()
        {
            if (isLocalPlayer)
            {
                _ikData = _vrPlayerController.GetIkData();
            }
        }
    }
}