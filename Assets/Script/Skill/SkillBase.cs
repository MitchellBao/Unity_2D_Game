using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [Header("��������")]
    public string skillName;
    [TextArea] public string description;
    public int cooldownRounds = 1;
    public int actionPointCost = 1;
    public Sprite icon;

    [HideInInspector] public PlayerControl owner;
    private int currentCooldown = 0;
    //���ü���ӵ����
    public void Initialize(PlayerControl player)
    {
        owner = player;
    }
    //�����ܷ�ʹ��
    public bool CanUse()
    {
        return currentCooldown <= 0 &&
               owner.Point >= actionPointCost;
    }
    //ʹ�÷���
    public void Use()
    {
        if (!CanUse()) return;

        owner.SpendActionPoints(actionPointCost);
        currentCooldown = cooldownRounds;
        ExecuteSkill();
    }
    //���༼��ʹ�÷���
    protected abstract void ExecuteSkill();

    public void UpdateCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }
    //������ȴ�ٷֱȣ�UIʹ��
    public float GetCooldownPercentage()
    {
        return cooldownRounds > 0 ? (float)currentCooldown / cooldownRounds : 0;
    }

}
