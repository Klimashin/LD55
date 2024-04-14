using UnityEngine;
using UnityEngine.Assertions;

public class GameplaySoundSystem : MonoBehaviour
{
    [SerializeField] private GameplaySoundLoop[] Loops;

    public void SetLoopTracks(int loopIndex, int[] tracks)
    {
        Assert.IsTrue(Loops.Length > loopIndex);
        for (var i = 0; i < Loops.Length; i++)
        {
            Loops[i].gameObject.SetActive(i == loopIndex);
        }
        
        Loops[loopIndex].SetActiveTracks(tracks);
    }
}
