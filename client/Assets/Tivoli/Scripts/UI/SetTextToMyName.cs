using TMPro;
using UnityEngine;

namespace Tivoli.Scripts.UI
{
    public class SetTextToMyName: MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = DependencyManager.Instance.steamManager.GetMyName();
        }
    }
}