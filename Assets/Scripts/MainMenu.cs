using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip _menuBgMusic;
    [SerializeField] private GameObject _authorsPanel;
    [SerializeField] private Button _authorsButton;
    [SerializeField] private Button _closeAuthorsButton;

    private SoundSystem _soundSystem;
    
    [Inject]
    private void Inject(SoundSystem soundSystem)
    {
        _soundSystem = soundSystem;
    }
    
    private void Start()
    {
        if (!_soundSystem.IsPlayingClip(_menuBgMusic))
        {
            _soundSystem.PlayMusicClip(_menuBgMusic);
        }
        
        _authorsButton.onClick.AddListener(OpenAuthorsPanel);
        _closeAuthorsButton.onClick.AddListener(CloseAuthorsPanel);
    }

    private void OpenAuthorsPanel()
    {
        _authorsPanel.gameObject.SetActive(true);
    }

    private void CloseAuthorsPanel()
    {
        _authorsPanel.gameObject.SetActive(false);
    }
}
