using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class GameplaySoundLoop : MonoBehaviour
{
    [SerializeField] private AudioSource[] _tracks;

    private const float UNMUTE_TIME = 0.5f;

    private CancellationTokenSource _cancellationTokenSource;

    public void SetActiveTracks(params int[] tracks)
    {
        if (tracks.Length == 0)
        {
            MuteAll();
            return;
        }
        
        for (var i = 0; i < tracks.Length; i++)
        {
            Assert.IsTrue(_tracks.Length > tracks[i]);
        }
        
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new ();
        var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, destroyCancellationToken);
        for (int i = 0; i < _tracks.Length; i++)
        {
            if (tracks.Contains(i))
            {
                UnMuteTrack(i, linkedTokenSource.Token).Forget();
            }
            else
            {
                _tracks[i].volume = 0f;
            }
        }
    }

    private void MuteAll()
    {
        for (int i = 0; i < _tracks.Length; i++)
        {
            _tracks[i].volume = 0f;
        }
    }

    private async UniTaskVoid UnMuteTrack(int index, CancellationToken token)
    {
        var track = _tracks[index];
        if (track.volume.Equals(1f))
        {
            return;
        }
        
        float time = 0f;
        while (time < UNMUTE_TIME)
        {
            _tracks[index].volume = Mathf.Lerp(0f, 1f, time / UNMUTE_TIME);
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, token);
            time += Time.deltaTime;
        }

        _tracks[index].volume = 1f;
    }

    private void OnEnable()
    {
        MuteAll();
    }
}
