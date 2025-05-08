using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditorInternal.VersionControl.ListControl;

public class ThiefSkill :SkillBase
{
    [Header("潜行技能设置")]
    public float teleportDistance = 2f;  // 瞬移距离（以网格为单位）
    public LayerMask collisionLayer;     // 碰撞检测层
    private Vector2 skillDirection;
    private bool isSelectingDirection = false;
    private Vector2 selectedDirection = Vector2.zero;

    protected override void ExecuteSkill()
    {
        // 进入方向选择模式
        EnterDirectionSelectionMode();
    }

    void EnterDirectionSelectionMode()
    {
        isSelectingDirection = true;
        selectedDirection = Vector2.zero;
        owner.EnterSkillTargetingMode(this);

        // 可以在这里显示方向选择UI提示
        Debug.Log("请使用方向键选择瞬移方向...");
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
        // 检测方向键输入
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.up;
            Debug.Log("选择向上方向");
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.down;
            Debug.Log("选择向下方向");
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.left;
            Debug.Log("选择向左方向");
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            selectedDirection = Vector2.right;
            Debug.Log("选择向右方向");
        }

        // 如果有方向被选择，执行瞬移并退出选择模式
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

        // 连续检测2格路径上的碰撞
        for (int i = 1; i <= 2; i++)
        {
            Vector2 checkPos = (Vector2)owner.transform.position + (direction * i * owner.gridSize);

            // 如果碰到不可穿透的障碍，停在障碍前
            if (Physics2D.OverlapCircle(checkPos, 0.2f, collisionLayer))
            {
                targetPos = checkPos - (direction * 0.1f); // 稍微后退避免卡住
                break;
            }
        }

        return targetPos;
    }

    void PerformTeleport(Vector2 targetPosition)
    {
        // 先禁用碰撞体实现"穿透"
        Collider2D col = owner.GetComponent<Collider2D>();
        bool originalColliderState = col.enabled;
        col.enabled = false;

        // 执行瞬移
        owner.transform.position = targetPosition;

        // 恢复碰撞体
        col.enabled = originalColliderState;
    }


}
