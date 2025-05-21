using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{

    // 现有事件
    public UnityAction<PlayerControl> OnActionPointEventRaised;

    // 新增事件
    public UnityAction<PlayerControl> OnDiamondStatusEventRaised; // 钻石状态变化
    public UnityAction<PlayerControl> OnFrozenStatusEventRaised;               // 冻结状态变化
    public UnityAction<PlayerControl> OnSkillEventRaised;                   // 技能使用变化

    // 事件触发方法
    public void RaiseActionPointEvent(PlayerControl character)
    {
        OnActionPointEventRaised?.Invoke(character);
    }

    public void RaiseDiamondStatusChange(PlayerControl character)
    {
        OnDiamondStatusEventRaised?.Invoke(character);
    }

    public void RaiseFrozenStatusChange(PlayerControl character)
    {
        OnFrozenStatusEventRaised?.Invoke(character);
    }

    public void RaiseSkillUseChange(PlayerControl character)
    {
        OnSkillEventRaised?.Invoke(character);
    }

}
