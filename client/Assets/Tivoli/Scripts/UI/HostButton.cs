using Tivoli.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class HostButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                DependencyManager.Instance.connectionManager.StartHosting();
            });
        }
    }
}
