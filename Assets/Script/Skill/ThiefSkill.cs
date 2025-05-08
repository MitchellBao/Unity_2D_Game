using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditorInternal.VersionControl.ListControl;

public class ThiefSkill :SkillBase
{
    [Header("Ǳ�м�������")]
    public float teleportDistance = 2f;  // ˲�ƾ��루������Ϊ��λ��
    public LayerMask collisionLayer;     // ��ײ����
    private Vector2 skillDirection;
    private bool isSelectingDirection = false;
    private Vector2 selectedDirection = Vector2.zero;

    protected override void ExecuteSkill()
    {
        // ���뷽��ѡ��ģʽ
        EnterDirectionSelectionMode();
    }

    void EnterDirectionSelectionMode()
    {
        isSelectingDirection = true;
        selectedDirection = Vector2.zero;
        owner.EnterSkillTargetingMode(this);

        // ������������ʾ����ѡ��UI��ʾ
        Debug.Log("��ʹ�÷����ѡ��˲�Ʒ���...");
    }

    void Update()
    {
        if (isSelectingDirection)
        {
            HandleDirectionInput();
        }
    }

    void HandleDirectionInput()
    {
        // ��ⷽ�������
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.up;
            Debug.Log("ѡ�����Ϸ���");
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.down;
            Debug.Log("ѡ�����·���");
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.left;
            Debug.Log("ѡ��������");
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.right;
            Debug.Log("ѡ�����ҷ���");
        }

        // ����з���ѡ��ִ��˲�Ʋ��˳�ѡ��ģʽ
        if (selectedDirection != Vector2.zero)
        {
            Vector2 targetPos = CalculateTeleportPosition(selectedDirection);
            PerformTeleport(targetPos);
            ExitDirectionSelectionMode();
        }
    }

    void ExitDirectionSelectionMode()
    {
        isSelectingDirection = false;
        selectedDirection = Vector2.zero;
        owner.ExitSkillTargetingMode();
    }
    Vector2 CalculateTeleportPosition(Vector2 direction)
    {
        Vector2 targetPos = (Vector2)owner.transform.position + (direction * teleportDistance * owner.gridSize);

        // �������2��·���ϵ���ײ
        for (int i = 1; i <= 2; i++)
        {
            Vector2 checkPos = (Vector2)owner.transform.position + (direction * i * owner.gridSize);

            // ����������ɴ�͸���ϰ���ͣ���ϰ�ǰ
            if (Physics2D.OverlapCircle(checkPos, 0.2f, collisionLayer))
            {
                targetPos = checkPos - (direction * 0.1f); // ��΢���˱��⿨ס
                break;
            }
        }

        return targetPos;
    }

    void PerformTeleport(Vector2 targetPosition)
    {
        // �Ƚ�����ײ��ʵ��"��͸"
        Collider2D col = owner.GetComponent<Collider2D>();
        bool originalColliderState = col.enabled;
        col.enabled = false;

        // ִ��˲��
        owner.transform.position = targetPosition;

        // �ָ���ײ��
        col.enabled = originalColliderState;
    }


}
