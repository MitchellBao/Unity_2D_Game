using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("Event Listening")]
    public FadeEventSO fadeEvent;

    public Image fadeImage;

    private void OnEnable()
    {
        fadeEvent.OnFadeEventRaised += onFadeEvent;
    }

    private void OnDisable()
    {
        fadeEvent.OnFadeEventRaised -= onFadeEvent;

    }

    private void onFadeEvent(Color target, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
