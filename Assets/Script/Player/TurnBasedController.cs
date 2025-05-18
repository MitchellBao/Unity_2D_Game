using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedController : MonoBehaviour
{
    public PlayerInputControl inputControl; // 你的输入系统
    public PlayerControl[] players;        // 两个角色的控制脚本
    private int currentPlayerIndex = 0;    // 当前控制的角色索引

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        players = FindObjectsOfType<PlayerControl>(); // 获取所有角色
    }

    private void OnEnable()
    {
        inputControl.Enable();
        inputControl.GamePlay.FinishRound.performed += _ => SwitchCharacter();//主动结束
        UpdateControlState(); // 初始化控制状态
    }
    private void Update()
    {
        // 持续检测当前角色行动点是否耗尽
        if (players[currentPlayerIndex].Point <= 0)
        {
            SwitchCharacter();
        }
    }
    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.GamePlay.FinishRound.performed -= _ => SwitchCharacter();
    }

    // 切换角色控制权
    private void SwitchCharacter()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        PlayerControl nextPlayer = players[currentPlayerIndex];

        // 检查是否冻结
        if (nextPlayer.isFrozen)
        {
            // 减少冻结回合数
            nextPlayer.frozenRoundsRemaining--;

            // 如果冻结结束，恢复状态
            if (nextPlayer.frozenRoundsRemaining <= 0)
            {
                nextPlayer.isFrozen = false;
                Debug.Log($"{nextPlayer.name} 解冻！");
            }
            else
            {
                Debug.Log($"{nextPlayer.name} 仍被冻结，剩余回合: {nextPlayer.frozenRoundsRemaining}");
            }

            // 直接跳过冻结玩家的回合，继续切换
            SwitchCharacter();
            return;
        }

        // 正常回合逻辑
        nextPlayer.OnRoundStart();
        UpdateControlState();
        Debug.Log($"切换到角色: {currentPlayerIndex + 1}");
    }
    // 更新角色输入状态
    private void UpdateControlState()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i == currentPlayerIndex); // 只有当前角色可操作
        }
    }
}
