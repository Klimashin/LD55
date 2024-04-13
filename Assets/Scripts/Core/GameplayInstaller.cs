using Reflex.Core;
using UnityEngine;

public class GameplayInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Character _character;
    [SerializeField] private RoundDance _danceController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MovementZone _movementZone;
    [SerializeField] private LightsController _lightsController;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_mainCamera);
        containerBuilder.AddSingleton(_movementZone);
        containerBuilder.AddSingleton(_lightsController);
        containerBuilder.AddSingleton(_character);
        containerBuilder.AddSingleton(_danceController);
        containerBuilder.AddSingleton(_cameraController);
    }
}
