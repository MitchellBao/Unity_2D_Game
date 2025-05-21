using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{

    // �����¼�
    public UnityAction<PlayerControl> OnActionPointEventRaised;

    // �����¼�
    public UnityAction<PlayerControl> OnDiamondStatusEventRaised; // ��ʯ״̬�仯
    public UnityAction<PlayerControl> OnFrozenStatusEventRaised;               // ����״̬�仯
    public UnityAction<PlayerControl> OnSkillEventRaised;                   // ����ʹ�ñ仯

    // �¼���������
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
