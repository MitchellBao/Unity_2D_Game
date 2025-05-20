using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class TimefreezeSkill : SkillBase
{
    [Header("ʱ�䶳������")]
    private PlayerControl frozenEnemy; // ������Ķ���
    private void Awake()
    {
        // ��ʼ����������
        skillName = "ʱ�䶳��";
        description = "�������1�غ��ж����Լ��»غ��ٻ��1�ж���";
        cooldownRounds = 3; // ��ȴ3�غ�
        actionPointCost = 3; // ����3�ж���
    }
   
    protected override void ExecuteSkill()
    {
        //�ҵ��������
        PlayerControl opponent = FindOpponent();
        if (opponent.isFrozen)
        {
            Debug.LogWarning("�����ѱ����ᣬ�����ظ�����");
            return;
        }
        // �������
        FreezeOpponent(opponent);
        // �����Լ��»غϵ��ж���ͷ�
        ApplyNextRoundPenalty();

    }
    private PlayerControl FindOpponent()
    {
        // ��ȡ������Ҳ��ҵ������Լ����Ǹ�
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
        // ��ȡ���ֵ�PlayerControl���
        opponent.isFrozen = true;

        Debug.Log($"{owner.name} ������ {opponent.name}!");
    }

    private void ApplyNextRoundPenalty()
    {
        // �����»غ��ж���ͷ�
        owner.NegativeImpact = true;
        Debug.Log($"�»غϽ����� 1 ���ж���");
    }


}
