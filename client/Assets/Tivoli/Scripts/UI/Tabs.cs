using UnityEngine;
using UnityEngine.UI;

public class Tabs : MonoBehaviour
{
    public TabButton[] tabButtons;
    public GameObject[] tabs;

    private void CloseAllButOne(int indexToOpen)
    {
        for (var i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].SetOpen(i == indexToOpen);
            tabs[i].SetActive(i == indexToOpen);
        }
    }

    private void Start()
    {
        if (tabButtons.Length != tabs.Length)
        {
            Debug.LogError("Tab buttons don't match tabs");
        }
        
        for (var i = 0; i < tabButtons.Length; i++)
        {
            var index = i;
            
            // make first open by default
            tabs[i].SetActive(i == 0);
            tabButtons[i].SetOpen(i == 0);
            
            tabButtons[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                CloseAllButOne(index);
            });
        }
    }
}
