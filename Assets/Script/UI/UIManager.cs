using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public PlayerStatusBar playerStatusBar01;
    public PlayerStatusBar playerStatusBar02;
   
    [Header("Events Listening")]
    public CharacterEventSO characterEventSO;

    private void OnEnable()
    {
        characterEventSO.OnActionPointEventRaised += OnActionPointChange01;
        characterEventSO.OnDiamondStatusEventRaised += OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised += OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised += OnSkillUseChange;
    }

    private void OnDisable()
    {
        characterEventSO.OnActionPointEventRaised -= OnActionPointChange01;
        characterEventSO.OnDiamondStatusEventRaised -= OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised -= OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised -= OnSkillUseChange;
    }

    private void OnActionPointChange01(PlayerControl character)
    {
        var percentage = (float)character.Point / 6;
        playerStatusBar01.OnActionPointChange(percentage);
    }

    void OnDiamondStatusChange(PlayerControl character)
    {
        bool hasDiamond = character.isGetDiamond;
        DiamondKinds kind = character.diamondKind;
        playerStatusBar01.OnDiamondStatusChange(hasDiamond, kind);
    }

    void OnFrozenStatusChange(PlayerControl character)
    {
        bool isFrozen = character.isFrozen;
        playerStatusBar01.OnFrozenStatusChange(isFrozen);
    }

    void OnSkillUseChange(PlayerControl character)
    {
        
        
    }
}
