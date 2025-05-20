using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class TimefreezeSkill : SkillBase
{
    [Header("时间冻结设置")]
    private PlayerControl frozenEnemy; // 被冻结的对手
    private void Awake()
    {
        // 初始化技能属性
        skillName = "时间冻结";
        description = "冻结对手1回合行动，自己下回合少获得1行动点";
        cooldownRounds = 3; // 冷却3回合
        actionPointCost = 3; // 消耗3行动点
    }
   
    protected override void ExecuteSkill()
    {
        //找到对手玩家
        PlayerControl opponent = FindOpponent();
        if (opponent.isFrozen)
        {
            Debug.LogWarning("对手已被冻结，不能重复冻结");
            return;
        }
        // 冻结对手
        FreezeOpponent(opponent);
        // 设置自己下回合的行动点惩罚
        ApplyNextRoundPenalty();

    }
    private PlayerControl FindOpponent()
    {
        // 获取所有玩家并找到不是自己的那个
        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl player in allPlayers)
        {
            if (player != owner)
            {
                return player;
            }
        }
        return null;
    }
    private void FreezeOpponent(PlayerControl opponent)
    {
        // 获取对手的PlayerControl组件
        opponent.isFrozen = true;

        Debug.Log($"{owner.name} 冻结了 {opponent.name}!");
    }

    private void ApplyNextRoundPenalty()
    {
        // 设置下回合行动点惩罚
        owner.NegativeImpact = true;
        Debug.Log($"下回合将减少 1 点行动点");
    }


}
