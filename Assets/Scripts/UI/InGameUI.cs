using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Canvas _pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_pauseMenu.gameObject.activeSelf)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

    private void OpenMenu()
    {
        _pauseMenu.gameObject.SetActive(true);
    }
    
    private void CloseMenu()
    {
        _pauseMenu.gameObject.SetActive(false);
    }
}
