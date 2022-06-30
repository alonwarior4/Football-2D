using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class Animation_SS : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] int framePerSec;
    [SerializeField] bool PlayOnEnable;

    public Action OnAnimationEnds;
    CancellationToken cToken;
    bool isStop = false;

    int spIndex;
    SpriteRenderer spRenderer;
    int frameTimeMiliSec;

    private void Awake()
    {
        cToken = this.GetCancellationTokenOnDestroy();

        spRenderer = GetComponent<SpriteRenderer>();
        frameTimeMiliSec = (int)((1f / framePerSec) * 1000);
    }

    private void OnEnable()
    {
        isStop = false;
        spIndex = 0;

        if (PlayOnEnable)
            Play();
    }

    public void Play()
    {
        PlayAnimationAsync().Forget();
    }

    private async UniTaskVoid PlayAnimationAsync()
    {
        while (!isStop && !IsAnimationFinished(spIndex))
        {
            spRenderer.sprite = sprites[spIndex];
            spIndex++;

            await UniTask.Delay(frameTimeMiliSec, delayTiming: PlayerLoopTiming.Update , cancellationToken : cToken);
        }

        spRenderer.sprite = null;
        OnAnimationEnds?.Invoke();
    }

    bool IsAnimationFinished(int spIndex) => spIndex == sprites.Length - 1;    

    private void OnDisable()
    {
        isStop = true;
    }
}
