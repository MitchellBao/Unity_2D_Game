using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/TurnEventSO")]
public class TurnEventSO : ScriptableObject
{
    // �����¼�
    public UnityAction<TurnBasedController> OnTurnEventRaised;
    public UnityAction<TurnBasedController> OnPointEventRaised;
    public UnityAction<TurnBasedController> OnGameOverEventRaised;


    public void RaiseTurnEvent(TurnBasedController turner)
    {
        OnTurnEventRaised?.Invoke(turner);
    }
    public void RaisePointEvent(TurnBasedController turner)
    {
        OnPointEventRaised?.Invoke(turner);
    }

    public void RaiseGameOverEvent(TurnBasedController turner)
    {
        OnGameOverEventRaised?.Invoke(turner);
    }

}
