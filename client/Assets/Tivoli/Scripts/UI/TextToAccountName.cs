using TMPro;
using UnityEngine;

namespace Tivoli.Scripts.UI
{
    public class TextToAccountName: MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = DependencyManager.Instance.accountManager.GetUsername();
        }
    }
}