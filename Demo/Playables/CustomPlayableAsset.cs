using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CustomPlayableAsset : PlayableAsset
{

    public int test;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<CustomPlayableBehaviour>.Create(graph);
        return playable;
    }
}
