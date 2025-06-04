using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEditorInternal.VersionControl.ListControl;

public class TeleportationSkill : SkillBase
{
    [Header("˲�Ƽ�������")]
    public float teleportDistance = 3f;  // ˲�ƾ��루������Ϊ��λ��
    public LayerMask collisionLayer;     // ��ײ����
    public LayerMask obstacleLayer;      // �ɴݻ��ϰ����
    public LayerMask obstacleBkLayer;    // �����ϰ����
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
        owner.duringSkill=true;
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
            owner.duringSkill = false;
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
        Vector2 finalPosition = (Vector2)owner.transform.position;

        // ���·���ϵ�ÿһ��
        for (int i = 1; i <= teleportDistance; i++)
        {
            Vector2 checkPos = (Vector2)owner.transform.position + (direction * i * owner.gridSize);

            // ���ɴݻ��ϰ���
            Collider2D destroyableObstacle = Physics2D.OverlapCircle(checkPos, 0.2f, obstacleLayer);
            if (destroyableObstacle != null)
            {
                // �ݻ��ϰ��ﲢ���Լ���ǰ��
                Obstacle obstacle = destroyableObstacle.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.TakeDamage();
                }
                finalPosition = checkPos;
                continue;
            }

            // ��鱳���ϰ���
            if (Physics2D.OverlapCircle(checkPos, 0.2f, obstacleBkLayer))
            {
                // ���������ϰ����ͣ�ڵ�ǰλ��
                return finalPosition;
            }

            // ����Ƿ񳬳��ƶ���Χ
            if (i == teleportDistance)
            {
                finalPosition = checkPos;
                break;
            }

            // ��������λ��
            finalPosition = checkPos;
        }

        return finalPosition;
    }

    void PerformTeleport(Vector2 targetPosition)
    {
        // ֱ��˲�Ƶ�Ŀ��λ��
        owner.transform.position = targetPosition;
    }
}

