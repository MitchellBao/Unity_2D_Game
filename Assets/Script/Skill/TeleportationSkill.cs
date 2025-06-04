using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEditorInternal.VersionControl.ListControl;

public class TeleportationSkill : SkillBase
{
    [Header("瞬移技能设置")]
    public float teleportDistance = 3f;  // 瞬移距离（以网格为单位）
    public LayerMask collisionLayer;     // 碰撞检测层
    public LayerMask obstacleLayer;      // 可摧毁障碍物层
    public LayerMask obstacleBkLayer;    // 背景障碍物层
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
        owner.duringSkill=true;
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

        // 检查路径上的每一格
        for (int i = 1; i <= teleportDistance; i++)
        {
            Vector2 checkPos = (Vector2)owner.transform.position + (direction * i * owner.gridSize);

            // 检查可摧毁障碍物
            Collider2D destroyableObstacle = Physics2D.OverlapCircle(checkPos, 0.2f, obstacleLayer);
            if (destroyableObstacle != null)
            {
                // 摧毁障碍物并可以继续前进
                Obstacle obstacle = destroyableObstacle.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.TakeDamage();
                }
                finalPosition = checkPos;
                continue;
            }

            // 检查背景障碍物
            if (Physics2D.OverlapCircle(checkPos, 0.2f, obstacleBkLayer))
            {
                // 遇到背景障碍物就停在当前位置
                return finalPosition;
            }

            // 检查是否超出移动范围
            if (i == teleportDistance)
            {
                finalPosition = checkPos;
                break;
            }

            // 更新最终位置
            finalPosition = checkPos;
        }

        return finalPosition;
    }

    void PerformTeleport(Vector2 targetPosition)
    {
        // 直接瞬移到目标位置
        owner.transform.position = targetPosition;
    }
}

