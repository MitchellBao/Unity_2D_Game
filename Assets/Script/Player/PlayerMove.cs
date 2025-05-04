
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
                if (inputDirection.x != 0)
                    transform.localScale = new Vector3(inputDirection.x, 1, 1);
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
        //if(inputDirection.x!=0)
        //    transform.localScale = new Vector3(inputDirection.x, 1, 1);
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
