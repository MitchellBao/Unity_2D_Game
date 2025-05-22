using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class UIManager : MonoBehaviour
{

    public PlayerStatusBar playerStatusBar01;
    public PlayerStatusBar playerStatusBar02;
    public TurnStatusBar turnStatusBar;
    public GameOverPanel gameOverPanel;
    
   
    [Header("Events Listening")]
    public CharacterEventSO characterEventSO;
    public TurnEventSO turnEventSO;

    private void OnEnable()
    {
        characterEventSO.OnActionPointEventRaised += OnActionPointChange;
        characterEventSO.OnDiamondStatusEventRaised += OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised += OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised += OnSkillUseChange;
        turnEventSO.OnTurnEventRaised += OnTurnChange;
        turnEventSO.OnTurnEventRaised += OnPointChange;
        turnEventSO.OnTurnEventRaised += onGameOverChange;
    }

    private void onGameOverChange(TurnBasedController turner)
    {
        if (turner.p1.Point == turner.p2.Point)
        {
            gameOverPanel.ShowResult("A DRAW!");
        }
        else if (turner.p1.Point < turner.p2.Point)
        {
            gameOverPanel.ShowResult("PLAYER 02 WINS!");
        }
        else
        {
            gameOverPanel.ShowResult("PLAYER 01 WINS!");
        }
    }

    private void OnDisable()
    {
        characterEventSO.OnActionPointEventRaised -= OnActionPointChange;
        characterEventSO.OnDiamondStatusEventRaised -= OnDiamondStatusChange;
        characterEventSO.OnFrozenStatusEventRaised -= OnFrozenStatusChange;
        characterEventSO.OnSkillEventRaised -= OnSkillUseChange;
        turnEventSO.OnTurnEventRaised -= OnTurnChange;
        turnEventSO.OnTurnEventRaised -= OnPointChange;
        turnEventSO.OnTurnEventRaised += onGameOverChange;

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

    private void OnTurnChange(TurnBasedController turner)
    {
        var percentage = (float)turner.currentRound / turner.maxRounds;
        turnStatusBar.OnTurnChange(percentage);
    }

    private void OnPointChange(TurnBasedController turner)
    {
        int p1 = turner.p1.Point;
        int p2 = turner.p2.Point;
        turnStatusBar.OnPointChange(p1, p2);
    }

}

