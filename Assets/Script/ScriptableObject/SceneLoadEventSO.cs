using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{

    public UnityAction<GameSceneSO, bool> LoadRequestEvent;


    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(locationToLoad, fadeScreen);
    }


}
