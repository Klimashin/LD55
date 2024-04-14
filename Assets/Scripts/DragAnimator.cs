using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class DragAnimator : MonoBehaviour
{
    [SerializeField] private DragHand[] _dragHands;
    
    public async UniTask AnimateDrag(Vector3 center, List<GameObject> obj)
    {
        Assert.IsTrue(obj.Count <= _dragHands.Length);
        List<UniTask> tasks = new();
        for (var i = 0; i < obj.Count; i++)
        {
            var startPosition = (obj[i].transform.position - center).normalized * 15f;
            tasks.Add(_dragHands[i].Drag(startPosition, obj[i]));
        }

        await UniTask.WhenAll(tasks);
    }
}
