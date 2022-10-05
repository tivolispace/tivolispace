using Mirror;
using Tivoli.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class HostButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(async () =>
            {
                var connectionManager = DependencyManager.Instance.connectionManager;
                if (NetworkServer.active)
                {
                    await connectionManager.StopHosting();
                }
                await connectionManager.StartHosting();
            });
        }
    }
}
