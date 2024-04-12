using Reflex.Attributes;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip _menuBgMusic;

    private SoundSystem _soundSystem;
    
    [Inject]
    private void Inject(SoundSystem soundSystem)
    {
        _soundSystem = soundSystem;
    }
    
    void Start()
    {
        _soundSystem.PlayMusicClip(_menuBgMusic);
    }
}
