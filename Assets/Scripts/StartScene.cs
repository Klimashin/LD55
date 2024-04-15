using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    [SerializeField] private List<TextController> _textControllers;
    [SerializeField] private Character _player;

    private void Start()
    {
        StartSceneRoutine().Forget();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _player.enabled = true;
        }
    }

    private async UniTaskVoid StartSceneRoutine()
    {
        _player.enabled = false;
        
        for (var i = 0; i < _textControllers.Count; i++)
        {
            await _textControllers[i].PlayAll();
        }
        
        _player.enabled = true;
        enabled = false;
    }
}
