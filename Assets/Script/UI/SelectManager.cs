using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using static UnityEngine.Rendering.VolumeComponent;

public class SelectManager : MonoBehaviour
{
    [Header("Components")]
    public TurnBasedController TurnBasedController;
    public GameObject characterSelectPanel;
    public Button gameStartButton;

    [Header("Evnets Listening")]
    public SelectorEventSO selectorEventSO;

    public void OnEnable()
    {
        selectorEventSO.OnSelectionFinishedEventRaised += OnSelectionFinished;
    }

    public void OnDisable()
    {
        selectorEventSO.OnSelectionFinishedEventRaised -= OnSelectionFinished;
    }

    private void OnSelectionFinished(SelectButton selectButton)
    {
        int index1 = selectButton.PlayerIndex01;
        int index2 = selectButton.PlayerIndex02;

        // 调用 TurnBasedController 中的方法生成角色
        TurnBasedController.GeneratePlayersFromSelection(index1, index2);

        // 隐藏选择面板，显示地图
        characterSelectPanel.SetActive(false);
        gameStartButton.gameObject.SetActive(false);
    }

}


