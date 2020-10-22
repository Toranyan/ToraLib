using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableTest : MonoBehaviour
{

    public PlayableBehaviour _targetPlayable;

    public Animator _targetAnimator;
    public AnimationClip _animationClip;

    PlayableGraph playableGraph;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayAnimationClip() {

        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "AnimationOutput", _targetAnimator);

        //wrap clip
        var clipPlayable = AnimationClipPlayable.Create(playableGraph, _animationClip);

        //connect to output
        playableOutput.SetSourcePlayable(clipPlayable);

        playableGraph.Play();
    }


    public void LeanTweenScale() {
        
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var ltPlayable = ScriptPlayable<LeanTweenPlayable>.Create(playableGraph);
        var ltdesc = LeanTween.scale(_targetAnimator.gameObject, Vector3.zero, 1);
        
        ltPlayable.GetBehaviour().SetTween(ltdesc);

        var playableOutput = ScriptPlayableOutput.Create(playableGraph, "ScriptPlayableOutpuut");
        playableOutput.SetSourcePlayable(ltPlayable);

        playableGraph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            PlayAnimationClip();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            LeanTweenScale();
        }
    }
}
