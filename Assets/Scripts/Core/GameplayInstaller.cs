using Reflex.Core;
using UnityEngine;

public class GameplayInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Character _character;
    [SerializeField] private RoundDance _danceController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LightsController _lightsController;
    [SerializeField] private EyesController _eyesController;
    [SerializeField] private DragAnimator _dragAnimator;
    [SerializeField] private GameplaySoundSystem _gameplaySoundSystem;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_mainCamera);
        containerBuilder.AddSingleton(_lightsController);
        containerBuilder.AddSingleton(_character);
        containerBuilder.AddSingleton(_danceController);
        containerBuilder.AddSingleton(_cameraController);
        containerBuilder.AddSingleton(_eyesController);
        containerBuilder.AddSingleton(_dragAnimator);
        containerBuilder.AddSingleton(_gameplaySoundSystem);
    }
}
