using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [Header("技能设置")]
    public string skillName;
    [TextArea] public string description;
    public int cooldownRounds = 1;
    public int actionPointCost = 1;
    public Sprite icon;

    [HideInInspector] public PlayerControl owner;
    private int currentCooldown = 0;
    //设置技能拥有者
    public void Initialize(PlayerControl player)
    {
        owner = player;
    }
    //技能能否使用
    public bool CanUse()
    {
        return currentCooldown <= 0 &&
               owner.Point >= actionPointCost;
    }
    //使用方法
    public void Use()
    {
        if (!CanUse()) return;

        owner.SpendActionPoints(actionPointCost);
        currentCooldown = cooldownRounds;
        ExecuteSkill();
    }
    //子类技能使用方法
    protected abstract void ExecuteSkill();

    public void UpdateCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }
    //技能冷却百分比，UI使用
    public float GetCooldownPercentage()
    {
        return cooldownRounds > 0 ? (float)currentCooldown / cooldownRounds : 0;
    }

}
