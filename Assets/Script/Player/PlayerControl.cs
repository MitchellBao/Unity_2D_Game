
using System.Collections;
using System.Collections.Generic;
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
    public bool isGetDiamond;
    public DiamondKinds diamondKind;
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
        InitializeSkills();
    }
    private void OnEnable()
    {
        inputControl.Enable();
        // 新增E键输入监听
        inputControl.GamePlay.PlaceBlock.performed += _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled += _ => TogglePlacementMode(false);
        // 新增鼠标点击监听
        inputControl.GamePlay.MouseClick.performed += OnMouseClick;
        inputControl.GamePlay.UseSkill1.performed += _ => TryUseSkill(0); // 数字1 -> 技能1
    }

    private void OnDisable()
    {
        inputControl.Disable();
        // 移除监听
        inputControl.GamePlay.PlaceBlock.performed -= _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled -= _ => TogglePlacementMode(false);
        inputControl.GamePlay.MouseClick.performed -= OnMouseClick;
        inputControl.GamePlay.UseSkill1.performed -= _ => TryUseSkill(0);
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
        //技能检测
        if(isInSkillTargetingMode)
        {
            HandleSkillTargeting();
            return;
        }
        // 添加技能快捷键 (示例：数字键1-4)
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryUseSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryUseSkill(1);

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
                Point-=2;
            }
        }
    }

    [Header("技能系统")]
    public List<SkillBase> skills = new List<SkillBase>(); // 技能列表
    private SkillBase currentActiveSkill; // 当前激活的技能(用于目标选择模式)
    private bool isInSkillTargetingMode = false; // 是否处于技能目标选择模式
    private bool movementEnabled = true;//技能释放是否允许移动
    //技能初始化
    void InitializeSkills()
    {
        // 获取所有子对象中的技能组件
        SkillBase[] childSkills = GetComponentsInChildren<SkillBase>();
        skills.AddRange(childSkills);

        // 初始化每个技能
        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }
    // 进入技能目标选择模式
    public void EnterSkillTargetingMode(SkillBase skill)
    {
        isInSkillTargetingMode = true;
        currentActiveSkill = skill;
        movementEnabled = false; // 禁用普通移动
    }

    // 退出技能目标选择模式
    public void ExitSkillTargetingMode()
    {
        isInSkillTargetingMode = false;
        currentActiveSkill = null;
        movementEnabled = true; // 恢复移动
    }
    void TryUseSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < skills.Count)
        {
            skills[skillIndex].Use();
        }
    }

    void HandleSkillTargeting()
    {
        // 这里处理技能目标选择的具体逻辑
        // 例如工程师技能选择障碍类型
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.CycleObstacleSelection();
        //}

        //// 确认技能释放
        //if (Input.GetMouseButtonDown(0))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.ConfirmPlacement();
        //}

        //// 取消技能
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.CancelSkill();
        //}
    }
    public void OnRoundStart()
    {
        // 更新所有技能冷却
        foreach (var skill in skills)
        {
            skill.UpdateCooldown();
        }

        // 更新UI
        //UIManager.Instance.UpdateActionPoints(currentActionPoints);
        //UIManager.Instance.UpdateSkillCooldowns();
    }
    public void SpendActionPoints(int amount)
    {
        Point = Mathf.Max(0, Point - amount);
        // 可以在这里添加UI更新
    }
    void Start()
    {
        // 自动获取所有子对象中的技能
        skills.AddRange(GetComponentsInChildren<SkillBase>());

        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }
}
