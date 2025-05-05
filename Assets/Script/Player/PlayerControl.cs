
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
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
        mainCamera = Camera.main; // 获取主相机
    }

    private void OnEnable()
    {
        inputControl.Enable();
        // 新增E键输入监听
        inputControl.GamePlay.PlaceBlock.performed += _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled += _ => TogglePlacementMode(false);
        // 新增鼠标点击监听
        inputControl.GamePlay.MouseClick.performed += OnMouseClick;
        
    }

    private void OnDisable()
    {
        inputControl.Disable();
        // 移除监听
        inputControl.GamePlay.PlaceBlock.performed -= _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled -= _ => TogglePlacementMode(false);
        inputControl.GamePlay.MouseClick.performed -= OnMouseClick;
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
    [Header("放置方块设置")]
    public GameObject blockPrefab; // 在Inspector中拖入你的方块预制体
    public LayerMask groundLayer; // 设置地面层级（用于鼠标射线检测）
    private bool isPlacingMode = false; // 是否处于放置模式
    private Camera mainCamera;

  

    

    // 切换放置模式
    private void TogglePlacementMode(bool isActive)
    {
        isPlacingMode = isActive;
        
    }
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!isPlacingMode || Point <= 0) return;

        Vector2 mousePos = inputControl.GamePlay.MousePosition.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);

        if (hit.collider != null)
        {
            // 关键修改：确保坐标对齐到网格中心
            Vector2 placePos = new Vector2(
                Mathf.Floor(hit.point.x / gridSize) * gridSize + gridSize * 0.5f,
                Mathf.Floor(hit.point.y / gridSize) * gridSize + gridSize * 0.5f
            );

            // 调试显示网格对齐位置（可视化检查）
            Debug.DrawLine(placePos - Vector2.one * 0.5f, placePos + Vector2.one * 0.5f, Color.green, 2f);

            if (!Physics2D.OverlapCircle(placePos, 0.1f, obstacleLayer))
            {
                Instantiate(blockPrefab, placePos, Quaternion.identity);
                Point--;
            }
        }
    }
}
