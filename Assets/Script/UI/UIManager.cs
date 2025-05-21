using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class UIManager : MonoBehaviour
{

    public PlayerStatusBar playerStatusBar01;
    public PlayerStatusBar playerStatusBar02;
   
    [Header("Events Listening")]
    public CharacterEventSO characterEventSO;

    private void OnEnable()
    {
        characterEventSO.OnActionPointEventRaised += OnActionPointChange;
        characterEventSO.OnDiamondStatusEventRaised += OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised += OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised += OnSkillUseChange;

    }

    private void OnDisable()
    {
        characterEventSO.OnActionPointEventRaised -= OnActionPointChange;
        characterEventSO.OnDiamondStatusEventRaised -= OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised -= OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised -= OnSkillUseChange;

    }

    private void OnActionPointChange(PlayerControl character)
    {
        var percentage = (float)character.Point / 6;
        if (character.playerIndex == 5)
        {
            playerStatusBar01.OnActionPointChange(percentage);
        }
        else if (character.playerIndex == 6)
        {
            playerStatusBar02.OnActionPointChange(percentage);
        }
    }

    void OnDiamondStatusChange(PlayerControl character)
    {
        bool hasDiamond = character.isGetDiamond;
        DiamondKinds kind = character.diamondKind;
        if (character.playerIndex == 5)
        {
            playerStatusBar01.OnDiamondStatusChange(hasDiamond, kind);
        }
        else if (character.playerIndex == 6)
        {
            playerStatusBar02.OnDiamondStatusChange(hasDiamond, kind);
        }
        
    }

    void OnFrozenStatusChange(PlayerControl character)
    {
        bool isFrozen = character.isFrozen;
        if (character.playerIndex == 5)
        {
            playerStatusBar01.OnFrozenStatusChange(isFrozen);
        }
        else if (character.playerIndex == 6)
        {
            playerStatusBar02.OnFrozenStatusChange(isFrozen);
        }
        
    }

    void OnSkillUseChange(PlayerControl character)
    {
        bool isSkillUsable = character.isSkillUsable;
        if (character.playerIndex == 5)
        {
            playerStatusBar01.OnSkillUseChange(isSkillUsable);
        }
        else if (character.playerIndex == 6)
        {
            playerStatusBar02.OnSkillUseChange(isSkillUsable);
        }

    }
}
