using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LeanTweenPlayable : PlayableBehaviour
{
    private LTDescr _ltDescr;
    private Action _finishCallback;

    public event Action PlayFinished;
    
    public void SetTween(LTDescr ltDescriptor) {
        _ltDescr = ltDescriptor;
        //_ltDescr.pause();
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info) {
        _ltDescr.reset();
        _ltDescr.resume();
    }

    public override void OnGraphStop(Playable playable) {
        PlayFinished?.Invoke();
        base.OnGraphStop(playable);
    }

}
