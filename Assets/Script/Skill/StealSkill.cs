using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StealSkill : SkillBase
{
    [Header("͵ȡ����")]
    public int stealRange = 5; // 5x5��Χ
    public LayerMask gemLayer; // ��ʯ�㼶
    public LayerMask playerLayer; // ��Ҳ㼶

    private void Awake()
    {
        skillName = "��ʯ͵ȡ";
        description = "5x5��Χ��͵ȡ��ʯ�����Я���ı�ʯ";
        cooldownRounds = 3;
        actionPointCost = 1;
    }

    protected override void ExecuteSkill()
    {
        // ��ȡ5x5��Χ�ڵ����б�ʯ�����
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

        // ����͵ȡ���汦ʯ
        foreach (Collider2D gemCol in gems)
        {
            if (gemCol.TryGetComponent<Diamond>(out Diamond gem) && !owner.isGetDiamond)
            {
                StealGem(gem);
                return; // ÿ��ֻ͵һ��
            }
        }

        // ͵ȡ����Я���ı�ʯ
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

        Debug.Log("��Χ��û�п�͵ȡ�ı�ʯ");
    }

    private void StealGem(Diamond gem)
    {
        owner.isGetDiamond = true;
        owner.diamondKind = gem.DiamondKinds;
        Destroy(gem.gameObject);
        Debug.Log($"͵ȡ�˵��汦ʯ: {gem.DiamondKinds}");
    }

    private void StealFromPlayer(PlayerControl target)
    {
        owner.isGetDiamond = true;
        owner.diamondKind = target.diamondKind;
        target.LoseGem();
        Debug.Log($"�� {target.name} ͵ȡ�˱�ʯ");
    }
}
