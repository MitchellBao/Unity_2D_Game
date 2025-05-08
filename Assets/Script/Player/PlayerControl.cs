
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class PlayerControl : MonoBehaviour
{
    public int Point = 100; // 行动点
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    public Rigidbody2D rb;
    public float gridSize = 1f; // 每格的大小
    public LayerMask obstacleLayer; // 障碍物层级（在Unity编辑器中设置）
    private bool isMoving = false; // 是否正在移动
    public bool isGetDiamond;

    private float inputCooldown = 0.2f; // 输入缓冲时间 避免切换角色时错误输入
    private float lastSwitchTime;

    private Vector2 _lastMoveDirection = Vector2.right; // 默认朝右
    public Vector2 LastMoveDirection => _lastMoveDirection; // 公开只读属性

    
    private Vector2 SnapTo4Directions(Vector2 dir)//移动方向锁定到四个方向
    {
        // 锁定到上下左右四个方向
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0);
        else
            return new Vector2(0, Mathf.Sign(dir.y));
    }
    void AttackObstacle()
    {
        Vector2 vec2 = new Vector2(1, 0);
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            _lastMoveDirection,
            gridSize,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.TakeDamage();
                Point--; // 消耗行动点
            }
        }
    }

    // 自动触发Instant障碍
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("InstantObstacle"))
        {
            col.gameObject.GetComponent<Obstacle>()?.TakeDamage();
        }
    }

    //private void UpdateLastDirection(Vector2 currentInput)// 更新方向
    //{
    //    // 只有有输入时才更新方向（避免静止时覆盖）
    //    if (currentInput != Vector2.zero)
    //    {
    //        _lastMoveDirection = currentInput.normalized; // 存储标准化方向
    //    }
    //}

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
    private bool isActive = false;

    public void SetActive(bool active)
    {
        
        isActive = active;
        GetComponent<SpriteRenderer>().color = active ? Color.white : Color.gray;
        if (active)
        {
            Point += 3;
            if (Point > 6)
            {
                Point = 6;
            }

            lastSwitchTime = Time.time;// 记录激活时间
        }
    }
    private void Update()
    {
        //回合检测+冷却
        if (!isActive || Time.time - lastSwitchTime < inputCooldown)
            return;

        // 攻击检测（独立冷却）
        if (Input.GetKeyDown(KeyCode.Space) && Point > 0)
        {
            AttackObstacle();
            return; // 攻击后跳过移动检测
        }
        if (!isMoving && Point > 0)
        {
            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
            
            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                // 确保单方向移动
                inputDirection=SnapTo4Directions(inputDirection);
                _lastMoveDirection = inputDirection;
                _lastMoveDirection = inputDirection.normalized;

                // 移动逻辑
                Vector2 targetPos = rb.position + _lastMoveDirection * gridSize;
                if (inputDirection.x != 0)
                    transform.localScale = new Vector3(inputDirection.x, 1, 1);
                if (!Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer))
                {
                    StartCoroutine(MoveOneStep(targetPos));
                }
                //inputDirection = inputDirection.normalized;
                //Vector2 targetPosition = rb.position + inputDirection * gridSize;
                //// 碰撞检测：检查目标位置是否有障碍物
                //if (!Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer))
                //{
                //    StartCoroutine(MoveOneStep(targetPosition));
                //}

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
    //private void OnMouseClick(InputAction.CallbackContext context)
    //{
    //    if (!isPlacingMode || Point <= 0) return;

    //    Vector2 mousePos = inputControl.GamePlay.MousePosition.ReadValue<Vector2>();
    //    Ray ray = mainCamera.ScreenPointToRay(mousePos);
    //    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);

    //    if (hit.collider != null)
    //    {
    //        // 关键修改：确保坐标对齐到网格中心
    //        Vector2 placePos = new Vector2(
    //            Mathf.Floor(hit.point.x / gridSize) * gridSize + gridSize * 0.5f,
    //            Mathf.Floor(hit.point.y / gridSize) * gridSize + gridSize * 0.5f
    //        );

    //        // 调试显示网格对齐位置（可视化检查）
    //        Debug.DrawLine(placePos - Vector2.one * 0.5f, placePos + Vector2.one * 0.5f, Color.green, 2f);

    //        if (!Physics2D.OverlapCircle(placePos, 0.1f, obstacleLayer))
    //        {
    //            Instantiate(blockPrefab, placePos, Quaternion.identity);
    //            Point-=2;
    //        }
    //    }
    //}
    Vector2 GetGridPosition(Vector2 worldPos)
    {
        float gridSize = 1.0f; // 需与你的移动格距一致
        return new Vector2(
            Mathf.Floor(worldPos.x / gridSize) * gridSize + gridSize * 0.5f,
            Mathf.Floor(worldPos.y / gridSize) * gridSize + gridSize * 0.5f
        );
    }
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!isPlacingMode || Point <= 0) return;

        Vector2 mousePos = inputControl.GamePlay.MousePosition.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);

        if (hit.collider != null)
        {
            Vector2 placePos = GetGridPosition(hit.point);

            if (!Physics2D.OverlapCircle(placePos, 0.1f, obstacleLayer))
            {
                
                GameObject newBox = Instantiate(blockPrefab, placePos, Quaternion.identity);
                SetRandomHardness(newBox); // 设置随机硬度
                newBox.GetComponent<Obstacle>().UpdateAppearance();
                Point -=2;
            }
        }
    }

    // 设置箱子随机硬度
    private void SetRandomHardness(GameObject box)
    {
        Obstacle obstacle = box.GetComponent<Obstacle>();
        if (obstacle == null) return;

        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= 0.7f) // 70%概率硬度1
        {
            obstacle.hardness = Obstacle.HardnessLevel.Fragile;
            obstacle.health = 1;
        }
        else // 30%概率硬度2
        {
            obstacle.hardness = Obstacle.HardnessLevel.Sturdy;
            obstacle.health = 2;
        }

        // 更新外观（如果有不同贴图）
        obstacle.UpdateAppearance();
    }
}
