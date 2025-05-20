using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StealSkill : SkillBase
{
    [Header("偷取设置")]
    public int stealRange = 5; // 5x5范围
    public LayerMask gemLayer; // 宝石层级
    public LayerMask playerLayer; // 玩家层级

    private void Awake()
    {
        skillName = "宝石偷取";
        description = "5x5范围内偷取宝石或对手携带的宝石";
        cooldownRounds = 3;
        actionPointCost = 1;
    }

    protected override void ExecuteSkill()
    {
        // 获取5x5范围内的所有宝石和玩家
        Collider2D[] gems = Physics2D.OverlapBoxAll(
            owner.transform.position,
            new Vector2(stealRange * owner.gridSize, stealRange * owner.gridSize),
            0,
            gemLayer
        );

        Collider2D[] players = Physics2D.OverlapBoxAll(
            owner.transform.position,
            new Vector2(stealRange * owner.gridSize, stealRange * owner.gridSize),
            0,
            playerLayer
        );

        // 优先偷取地面宝石
        foreach (Collider2D gemCol in gems)
        {
            if (gemCol.TryGetComponent<Diamond>(out Diamond gem) && !owner.isGetDiamond)
            {
                StealGem(gem);
                return; // 每次只偷一个
            }
        }

        // 偷取对手携带的宝石
        foreach (Collider2D playerCol in players)
        {
            if (playerCol.gameObject != owner.gameObject &&
                playerCol.TryGetComponent<PlayerControl>(out PlayerControl target) &&
                target.isGetDiamond &&
                !owner.isGetDiamond)
            {
                StealFromPlayer(target);
                return;
            }
        }

        Debug.Log("范围内没有可偷取的宝石");
    }

    private void StealGem(Diamond gem)
    {
        owner.isGetDiamond = true;
        owner.diamondKind = gem.DiamondKinds;
        Destroy(gem.gameObject);
        Debug.Log($"偷取了地面宝石: {gem.DiamondKinds}");
    }

    private void StealFromPlayer(PlayerControl target)
    {
        owner.isGetDiamond = true;
        owner.diamondKind = target.diamondKind;
        target.LoseGem();
        Debug.Log($"从 {target.name} 偷取了宝石");
    }
}
