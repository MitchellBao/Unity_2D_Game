using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnEventSO : MonoBehaviour
{
    // 现有事件
    public UnityAction<TurnBasedController> OnTurnEventRaised;
}
