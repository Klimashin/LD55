using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private List<Replica> _replicas;
    [SerializeField] private TextMeshPro _worldSpaceText;

    public async UniTask PlayAll()
    {
        _worldSpaceText.enabled = true;
        
        foreach (var replica in _replicas)
        {
            await TypeReplica(replica);
        }

        _worldSpaceText.enabled = false;
    }

    private async UniTask TypeReplica(Replica replica)
    {
        _worldSpaceText.text = replica.Text;
        for (int i = 0; i <= replica.Text.Length; i++)
        {
            _worldSpaceText.maxVisibleCharacters = i;
            await UniTask.Delay(TimeSpan.FromSeconds(replica.TypeSpeed), DelayType.DeltaTime, PlayerLoopTiming.Update,
                destroyCancellationToken);
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(replica.PostTypeDelay), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);
    }
}

[Serializable]
public class Replica
{
    public string Text;
    public float TypeSpeed = 0.05f;
    public float PostTypeDelay = 3f;
}
