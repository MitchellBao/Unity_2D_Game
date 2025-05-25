using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SelectorEventSO")]
public class SelectorEventSO : ScriptableObject
{
    public UnityAction<SelectButton> OnSelectionFinishedEventRaised;

    public void RaiseSelectionFinishedEvent(SelectButton selectButton)
    {
        OnSelectionFinishedEventRaised?.Invoke(selectButton);
    }

}
