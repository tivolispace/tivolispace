using System.Collections;
using System.Collections.Generic;
using Tivoli.Scripts;
using Tivoli.Scripts.Managers;
using Tivoli.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;

public class GetAllOnlineUserProfiles : MonoBehaviour
{
    public GameObject userGameObject;

    private List<GameObject> _userGameObjects = new();

    private async void Start()
    {
        userGameObject.SetActive(false);
        await DependencyManager.Instance.AccountManager.WhenLoggedIn();
        RefreshUsers();
    }

    public async void RefreshUsers()
    {
        var allOnlineUsers = await DependencyManager.Instance.AccountManager.GetAllOnlineUsers();

        foreach (var currentUserGameObject in _userGameObjects)
        {
            Destroy(currentUserGameObject);
        }
        
        _userGameObjects.Clear();
        
        foreach (var userId in allOnlineUsers.userIds)
        {
            var currentUserGameObject = Instantiate(userGameObject, transform, true);
            _userGameObjects.Add(currentUserGameObject);

            var getUserProfile = currentUserGameObject.GetComponent<GetUserProfile>();
            getUserProfile.userId = userId;
            
            currentUserGameObject.SetActive(true);
        }
    }
}
