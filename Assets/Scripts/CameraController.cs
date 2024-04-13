using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _characterCamera;
    [SerializeField] private CinemachineVirtualCamera _danceCamera;

    public enum CameraType
    {
        Dance,
        Character
    }

    public void SetCamera(CameraType type)
    {
        _characterCamera.gameObject.SetActive(type == CameraType.Character);
        _danceCamera.gameObject.SetActive(type == CameraType.Dance);
    }
}
