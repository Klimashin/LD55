using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationDelay : MonoBehaviour
{
    [SerializeField] private Vector2 _range = new Vector2(1f, 5f);
    [SerializeField] private Animator _animator;
    
    private void Start()
    {
        DelayAnimation().Forget();
    }

    private async UniTaskVoid DelayAnimation()
    {
        gameObject.SetActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_range.x, _range.y)), DelayType.DeltaTime,
            PlayerLoopTiming.Update, destroyCancellationToken);

        gameObject.SetActive(true);
    }
}
