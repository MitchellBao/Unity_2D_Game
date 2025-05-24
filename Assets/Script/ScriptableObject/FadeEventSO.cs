using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float, bool> OnFadeEventRaised;

    // Turn Black
    public void FadeIn(float duration)
    {
        RaiseFadeEvent(Color.black, duration, true);
    }

    // Turn Transparent
    public void FadeOut(float duration) 
    { 
        RaiseFadeEvent(Color.clear, duration, false);
    }

    public void RaiseFadeEvent(Color target, float duration, bool fadeIn)
    {
        OnFadeEventRaised?.Invoke(target, duration, fadeIn);
    }

}
