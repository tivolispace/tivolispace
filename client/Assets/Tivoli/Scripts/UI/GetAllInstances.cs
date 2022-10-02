using System.Collections.Generic;
using Tivoli.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tivoli.Scripts.UI
{
    public class GetAllInstances : MonoBehaviour
    {
        public GameObject instanceGameObject;

        private List<GameObject> _instanceGameObjects = new();

        private async void Start()
        {
            instanceGameObject.SetActive(false);
            await DependencyManager.Instance.accountManager.WhenLoggedIn();
            RefreshInstances();
        }

        public async void RefreshInstances()
        {
            var allInstanaces = await DependencyManager.Instance.accountManager.GetAllInstances();

            foreach (var currentInstanceGameObject in _instanceGameObjects)
            {
                Destroy(currentInstanceGameObject);
            }

            _instanceGameObjects.Clear();

            foreach (var instance in allInstanaces)
            {
                var currentInstanceGameObject = Instantiate(instanceGameObject, transform, true);
                currentInstanceGameObject.transform.SetParent(instanceGameObject.transform.parent);
                _instanceGameObjects.Add(currentInstanceGameObject);

                var getUserProfile = currentInstanceGameObject.GetComponentInChildren<TextMeshProUGUI>();
                getUserProfile.text = instance.owner.displayName + "'s Instance";

                var button = currentInstanceGameObject.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    DependencyManager.Instance.connectionManager.Join(instance.connectionUri);
                });

                currentInstanceGameObject.SetActive(true);
            }
        }
    }
}