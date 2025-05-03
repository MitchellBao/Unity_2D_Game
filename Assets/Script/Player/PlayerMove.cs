//using System.Collections;
//using System.Collections.Generic;
//using System.Drawing.Text;
//using UnityEngine;
//using UnityEngine.InputSystem;


//public class PlayerMove : MonoBehaviour
//{
//    public int Point=100;//行动点
//    public PlayerInputControl inputControl;
//    public Vector2 inputDirection;
//    public Rigidbody2D rb;
//    public float gridSize = 1f; // 每格的大小
//    private bool isMoving = false; // 是否正在移动

//    private void Awake()
//    {
//        inputControl = new PlayerInputControl();
//    }

//    private void OnEnable()
//    {
//        inputControl.Enable();
//    }

//    private void OnDisable()
//    {
//        inputControl.Disable();
//    }

//    private void Update()
//    {
//        // 只在没有移动时检测输入
//        if (!isMoving)
//        {
//            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

//            // 检查是否有方向键输入（避免斜向移动）
//            if (Point>0&&(inputDirection.x != 0 || inputDirection.y != 0))
//            {
//                // 确保只朝一个方向移动（避免斜向移动）
//                if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
//                {
//                    inputDirection.y = 0;
//                }
//                else
//                {
//                    inputDirection.x = 0;
//                }

//                // 归一化方向（确保每次移动一格）
//                inputDirection = inputDirection.normalized;
//                StartCoroutine(MoveOneStep());
//            }
//        }
//    }

//    // 协程：移动一格
//    IEnumerator MoveOneStep()
//    {
//        isMoving = true;
//        Point--;
//        Vector2 startPosition = rb.position;
//        Vector2 targetPosition = startPosition + inputDirection * gridSize;

//        float moveTime = 0.2f; // 移动时间
//        float elapsedTime = 0f;

//        while (elapsedTime < moveTime)
//        {
//            rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        rb.position = targetPosition; // 确保最终位置准确
//        isMoving = false;
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public int Point = 100; // 行动点
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    public Rigidbody2D rb;
    public float gridSize = 1f; // 每格的大小
    public LayerMask obstacleLayer; // 障碍物层级（在Unity编辑器中设置）
    private bool isMoving = false; // 是否正在移动

    private void Awake()
    {
        inputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        if (!isMoving && Point > 0)
        {
            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                // 确保单方向移动
                if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
                {
                    inputDirection.y = 0;
                }
                else
                {
                    inputDirection.x = 0;
                }

                inputDirection = inputDirection.normalized;
                Vector2 targetPosition = rb.position + inputDirection * gridSize;

                // 碰撞检测：检查目标位置是否有障碍物
                if (!Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer))
                {
                    StartCoroutine(MoveOneStep(targetPosition));
                }
            }
        }
    }

    IEnumerator MoveOneStep(Vector2 targetPosition)
    {
        isMoving = true;
        Point--;

        Vector2 startPosition = rb.position;
        float moveTime = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = targetPosition;
        isMoving = false;
    }
}
