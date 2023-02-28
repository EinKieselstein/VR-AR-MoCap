using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartAnim : MonoBehaviour
{
    public PlayableDirector playableDirector;

    public void StartAnimation()
    {
        playableDirector.Play();
    }
}
