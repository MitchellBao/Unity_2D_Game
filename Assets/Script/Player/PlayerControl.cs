
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControl : MonoBehaviour
{
    public bool duringSkill = false;
    [Header("玩家索引设置")]
    public int playerIndex = 0; // 0=玩家1, 1=玩家2
    public PlayerInputControl inputControl;//输入控制
    public float gridSize = 1f;// 每格的大小
    public int Point = 100; // 行动点
    [Header("障碍物层级设置")]
    public LayerMask obstacleLayer; // 障碍物层级（在Unity编辑器中设置）
    public LayerMask playerLayer;//玩家层级
    public LayerMask gemLayer;//宝石层级
    [Header("持有宝石状态设置")]
    public bool isGetDiamond;
    public DiamondKinds diamondKind;//持有宝石类别

    private bool isMoving = false; // 是否正在移动
    public Vector2 inputDirection;
    public Rigidbody2D rb;
    
    private float inputCooldown = 0.2f; // 输入缓冲时间 避免切换角色时错误输入
    private float lastSwitchTime;

    private Vector2 _lastMoveDirection = Vector2.right; // 默认朝右
    public Vector2 LastMoveDirection => _lastMoveDirection; // 公开只读属性

    void Start()
    {
        // 自动获取所有子对象中的技能
        skills.AddRange(GetComponentsInChildren<SkillBase>());

        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }
    private void Awake()
    {
        inputControl = new PlayerInputControl();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        //技能监听
        if (playerIndex == 0) // 玩家1
        {
            inputControl.GamePlay.UseSkill1.performed += _ => TryUseSkill(0);
            inputControl.GamePlay.UseSkill3.performed += _ => TryUseSkill(1);
        }
        else if (playerIndex == 1) // 玩家2
        {
            inputControl.GamePlay.UseSkill2.performed += _ => TryUseSkill(0);
            inputControl.GamePlay.UseSkill4.performed += _ => TryUseSkill(1);
        }
        //偷宝石检测
        inputControl.GamePlay.GetDiamond.performed += _ => TryKnockdown();
    }
    private void OnDisable()
    {
        inputControl.Disable();
        // 移除监听
        inputControl.GamePlay.PlaceBlock.performed -= _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled -= _ => TogglePlacementMode(false);
        inputControl.GamePlay.MouseClick.performed -= OnMouseClick;
        inputControl.GamePlay.UseSkill1.performed -= _ => TryUseSkill(0);
        inputControl.GamePlay.UseSkill2.performed -= _ => TryUseSkill(0);
        inputControl.GamePlay.UseSkill3.performed -= _ => TryUseSkill(1);
        inputControl.GamePlay.UseSkill4.performed -= _ => TryUseSkill(1);
        inputControl.GamePlay.GetDiamond.performed -= _ => TryKnockdown();
    }

    //移动相关
    private Vector2 SnapTo4Directions(Vector2 dir)//移动方向锁定到四个方向
    {
        // 锁定到上下左右四个方向
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0);
        else
            return new Vector2(0, Mathf.Sign(dir.y));
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


    //宝石相关
    [Header("宝石预制体设置")]
    public GameObject redGemPrefab;
    public GameObject blueGemPrefab;
    public GameObject greenGemPrefab;
    public GameObject poisonousGemPrefab;

    private SpriteRenderer spriteRenderer;

    private void TryKnockdown()
    {
        if (Point <= 0 ) return;

        Collider2D[] players = Physics2D.OverlapCircleAll(
            transform.position,
            gridSize * 2.5f,
            playerLayer);

        foreach (Collider2D playerCol in players)
        {
            if (playerCol.gameObject != gameObject)
            {
                PlayerControl otherPlayer = playerCol.GetComponent<PlayerControl>();
                if (otherPlayer != null && otherPlayer.isGetDiamond)
                {
                    if (TryKnockdownGemFromPlayer(otherPlayer))
                    {
                        Point--; // 只有成功击落才消耗点数
                        return; // 每次只处理一个玩家
                    }
                }
            }
        }
    }

  
    // 世界坐标转网格索引
    private Vector2 WorldToGridIndex(Vector2 worldPos)
    {
        return new Vector2(
            Mathf.Floor(worldPos.x / gridSize),
            Mathf.Floor(worldPos.y / gridSize)
        );
    }

    // 网格索引转世界坐标(中心点)
    private Vector2 GridIndexToWorldCenter(Vector2 gridIndex)
    {
        return new Vector2(
            gridIndex.x * gridSize + gridSize * 0.5f,
            gridIndex.y * gridSize + gridSize * 0.5f
        );
    }
    private bool TryKnockdownGemFromPlayer(PlayerControl otherPlayer)
    {
        Vector2 myPos = transform.position;
        Vector2 otherPos = otherPlayer.transform.position;
        Vector2 myGrid = WorldToGridIndex(myPos);
        Vector2 otherGrid = WorldToGridIndex(otherPos);

        float distance = Vector2.Distance(myPos, otherPos);
        DiamondKinds otherGemKind = otherPlayer.diamondKind;

        // 情况1：直接相邻(距离1)
        if (distance <= gridSize * 1.1f)
        {
            // 直接从对方获取宝石
            otherPlayer.LoseGem();
            isGetDiamond = true;
            diamondKind = otherGemKind;
            return true;
        }

        // 情况2：直线距离2格无遮挡
        if (distance <= gridSize * 2.1f &&
            IsStraightLine(myPos, otherPos) &&
            !Physics2D.Linecast(myPos, otherPos, obstacleLayer))
        {
            // 计算中间网格
            Vector2 midGrid = new Vector2(
                Mathf.Round((myGrid.x + otherGrid.x) / 2),
                Mathf.Round((myGrid.y + otherGrid.y) / 2)
            );
            otherPlayer.LoseGem();
            DropGem(GridIndexToWorldCenter(midGrid), otherGemKind);
            return true;
        }

        // 情况3：斜向相邻(距离√2)
        if (distance <= gridSize * 1.5f)
        {
            Vector2[] possibleDrops = GetRemainingGridCorners(myGrid, otherGrid);
            Vector2? validDropPos = FindValidDropPosition(possibleDrops);

            if (validDropPos.HasValue)
            {
                otherPlayer.LoseGem();
                DropGem(GridIndexToWorldCenter(validDropPos.Value), otherGemKind);
                return true;
            }
            return false;
        }

        // 情况4：无遮挡(任何距离)
        if (!Physics2D.Linecast(myPos, otherPos, obstacleLayer))
        {
            otherPlayer.LoseGem();
            DropGem(myPos, otherGemKind);
            return true;
        }
        Debug.Log("没有满足任何击落条件");
        return false;
    }
    private bool IsStraightLine(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Approximately(pos1.x, pos2.x) ||
               Mathf.Approximately(pos1.y, pos2.y);
    }

    private Vector2[] GetRemainingGridCorners(Vector2 gridIndex1, Vector2 gridIndex2)
    {
        // 计算两个玩家的相对方位
        bool targetIsRight = gridIndex2.x > gridIndex1.x;
        bool targetIsAbove = gridIndex2.y > gridIndex1.y;

        // 根据方位确定剩余两个角落
        if (targetIsRight && targetIsAbove) // 目标在右上
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // 左上
            new Vector2(gridIndex2.x, gridIndex1.y)  // 右下
            };
        }
        else if (targetIsRight && !targetIsAbove) // 目标在右下
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // 左下
            new Vector2(gridIndex2.x, gridIndex1.y)  // 右上
            };
        }
        else if (!targetIsRight && targetIsAbove) // 目标在左上
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // 左下
            new Vector2(gridIndex2.x, gridIndex1.y)  // 右上
            };
        }
        else // 目标在左下
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // 左上
            new Vector2(gridIndex2.x, gridIndex1.y)  // 右下
            };
        }
    }
    
    private Vector2? FindValidDropPosition(Vector2[] gridIndices)
    {
        // 收集所有有效位置
        List<Vector2> validPositions = new List<Vector2>();

        foreach (Vector2 gridIndex in gridIndices)
        {
            Vector2 worldPos = GridIndexToWorldCenter(gridIndex);

            // 检查该位置是否被障碍物或其他宝石占据
            bool positionBlocked = Physics2D.OverlapCircle(worldPos, 0.2f, obstacleLayer) != null ||
                                  Physics2D.OverlapCircle(worldPos, 0.2f, gemLayer) != null;

            if (!positionBlocked)
            {
                validPositions.Add(gridIndex);
            }
        }

        // 随机选择一个有效位置
        if (validPositions.Count > 0)
        {
            return validPositions[Random.Range(0, validPositions.Count)];
        }
        Debug.LogWarning("没有找到有效的掉落位置");
        return null;
    }
    

    // 根据种类获取对应宝石预制体
    public GameObject GetGemPrefabByKind(DiamondKinds kind)
    {
        switch (kind)
        {
            case DiamondKinds.redDiamond:
                return redGemPrefab;
            case DiamondKinds.blueDiamond:
                return blueGemPrefab;
            case DiamondKinds.greenDiamond:
                return greenGemPrefab;
            case DiamondKinds.poisonousDiamond:
                return poisonousGemPrefab;
            // 添加更多种类...
            default:
                Debug.LogWarning($"未知宝石种类: {kind}, 使用红色宝石作为默认");
                return redGemPrefab;
        }
    }
    //宝石掉落
    private void DropGem(Vector2 position, DiamondKinds kind)
    {
        GameObject prefab = GetGemPrefabByKind(kind);

        if (prefab != null)
        {
            GameObject newGem = Instantiate(prefab, position, Quaternion.identity);

            // 确保有Diamond组件并设置种类(即使预制体已有)
            Diamond diamondComp = newGem.GetComponent<Diamond>();
            if (diamondComp == null)
            {
                diamondComp = newGem.AddComponent<Diamond>();
            }
            diamondComp.DiamondKinds = kind;

            // 确保有碰撞体
            if (newGem.GetComponent<Collider2D>() == null)
            {
                newGem.AddComponent<BoxCollider2D>();
            }

        }
        else
        {
            Debug.LogError($"{kind}宝石预制体未分配！请在Inspector中设置");
        }
    }
    public void LoseGem()
    {
        isGetDiamond = false;
    }
    void AttackObstacle()//摧毁箱子
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

    private bool isActive = false;
    public void SetActive(bool active)
    {
        isActive = active;
        if (active && !isFrozen)
        {
            inputControl.Enable();
            if (!NegativeImpact)
                Point += 3;
            else
            {
                Point += 2;
                NegativeImpact = false;
            }
            if (Point > 6)
            {
                Point = 6;
            }
        }
        else
        {
            inputControl.Disable(); // 关键！禁用非活跃玩家的输入
        }
        lastSwitchTime = Time.time;// 记录激活时间
    }

    private void Update()
    {
        

        //没做ui之前的可视化宝石持有
        if (isActive && isGetDiamond) GetComponent<SpriteRenderer>().color = Color.cyan;
        if (isActive && !isGetDiamond) GetComponent<SpriteRenderer>().color = Color.white;
        if (!isActive && !isGetDiamond) GetComponent<SpriteRenderer>().color = Color.gray;
        if (!isActive && isGetDiamond) GetComponent<SpriteRenderer>().color = Color.black;
        //
        if (isFrozen) GetComponent<SpriteRenderer>().color = Color.blue;
        //
        //避免被冻恢复以后卡主
        //EnterSkillTargetingMode(currentActiveSkill);
        //ExitSkillTargetingMode();

        //回合检测+冷却
        if (!isActive || Time.time - lastSwitchTime < inputCooldown)
            return;
        
        if (isFrozen)
        {
            //isFrozen = false;
            return;
        }
        //技能检测
        if (isInSkillTargetingMode)
        {
            HandleSkillTargeting();
            return;
        }
        

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

            }

        }
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
        if (!isPlacingMode || Point-2 < 0) return;

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
    [Header("冻结状态")]
    public bool isFrozen = false;
    public bool NegativeImpact = false;

}
