using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    [SerializeField] private List<TextController> _textControllers;
    [SerializeField] private Character _player;
    [SerializeField] private Transform _initialLookTransform;

    private bool _isReleased;

    private void Start()
    {
        StartSceneRoutine().Forget();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ReleasePlayer();
        }
    }

    private async UniTaskVoid StartSceneRoutine()
    {
        _player.enabled = false;
        _player.SetLookTransform(_initialLookTransform);
        
        for (var i = 0; i < _textControllers.Count; i++)
        {
            await _textControllers[i].PlayAll();
        }

        ReleasePlayer();
        enabled = false;
    }

    private void ReleasePlayer()
    {
        if (_isReleased)
        {
            return;
        }

        _player.enabled = true;
        _player.SetLookTransform(null);
        _isReleased = true;
    }
}
