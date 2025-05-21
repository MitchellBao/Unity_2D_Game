
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControl : MonoBehaviour
{
    public bool duringSkill = false;
    [Header("�����������")]
    public int playerIndex = 0; // 0=���1, 1=���2
    public PlayerInputControl inputControl;//�������
    public float gridSize = 1f;// ÿ��Ĵ�С
    public int Point = 100; // �ж���
    [Header("�ϰ���㼶����")]
    public LayerMask obstacleLayer; // �ϰ���㼶����Unity�༭�������ã�
    public LayerMask playerLayer;//��Ҳ㼶
    public LayerMask gemLayer;//��ʯ�㼶
    [Header("���б�ʯ״̬����")]
    public bool isGetDiamond;
    public DiamondKinds diamondKind;//���б�ʯ���

    private bool isMoving = false; // �Ƿ������ƶ�
    public Vector2 inputDirection;
    public Rigidbody2D rb;
    
    private float inputCooldown = 0.2f; // ���뻺��ʱ�� �����л���ɫʱ��������
    private float lastSwitchTime;

    private Vector2 _lastMoveDirection = Vector2.right; // Ĭ�ϳ���
    public Vector2 LastMoveDirection => _lastMoveDirection; // ����ֻ������

    void Start()
    {
        // �Զ���ȡ�����Ӷ����еļ���
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
        mainCamera = Camera.main; // ��ȡ�����
        InitializeSkills();
    }
   
    private void OnEnable()
    {
        inputControl.Enable();
        // ����E���������
        inputControl.GamePlay.PlaceBlock.performed += _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled += _ => TogglePlacementMode(false);
        // �������������
        inputControl.GamePlay.MouseClick.performed += OnMouseClick;
        //���ܼ���
        if (playerIndex == 0) // ���1
        {
            inputControl.GamePlay.UseSkill1.performed += _ => TryUseSkill(0);
            inputControl.GamePlay.UseSkill3.performed += _ => TryUseSkill(1);
        }
        else if (playerIndex == 1) // ���2
        {
            inputControl.GamePlay.UseSkill2.performed += _ => TryUseSkill(0);
            inputControl.GamePlay.UseSkill4.performed += _ => TryUseSkill(1);
        }
        //͵��ʯ���
        inputControl.GamePlay.GetDiamond.performed += _ => TryKnockdown();
    }
    private void OnDisable()
    {
        inputControl.Disable();
        // �Ƴ�����
        inputControl.GamePlay.PlaceBlock.performed -= _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled -= _ => TogglePlacementMode(false);
        inputControl.GamePlay.MouseClick.performed -= OnMouseClick;
        inputControl.GamePlay.UseSkill1.performed -= _ => TryUseSkill(0);
        inputControl.GamePlay.UseSkill2.performed -= _ => TryUseSkill(0);
        inputControl.GamePlay.UseSkill3.performed -= _ => TryUseSkill(1);
        inputControl.GamePlay.UseSkill4.performed -= _ => TryUseSkill(1);
        inputControl.GamePlay.GetDiamond.performed -= _ => TryKnockdown();
    }

    //�ƶ����
    private Vector2 SnapTo4Directions(Vector2 dir)//�ƶ������������ĸ�����
    {
        // ���������������ĸ�����
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


    //��ʯ���
    [Header("��ʯԤ��������")]
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
                        Point--; // ֻ�гɹ���������ĵ���
                        return; // ÿ��ֻ����һ�����
                    }
                }
            }
        }
    }

  
    // ��������ת��������
    private Vector2 WorldToGridIndex(Vector2 worldPos)
    {
        return new Vector2(
            Mathf.Floor(worldPos.x / gridSize),
            Mathf.Floor(worldPos.y / gridSize)
        );
    }

    // ��������ת��������(���ĵ�)
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

        // ���1��ֱ������(����1)
        if (distance <= gridSize * 1.1f)
        {
            // ֱ�ӴӶԷ���ȡ��ʯ
            otherPlayer.LoseGem();
            isGetDiamond = true;
            diamondKind = otherGemKind;
            return true;
        }

        // ���2��ֱ�߾���2�����ڵ�
        if (distance <= gridSize * 2.1f &&
            IsStraightLine(myPos, otherPos) &&
            !Physics2D.Linecast(myPos, otherPos, obstacleLayer))
        {
            // �����м�����
            Vector2 midGrid = new Vector2(
                Mathf.Round((myGrid.x + otherGrid.x) / 2),
                Mathf.Round((myGrid.y + otherGrid.y) / 2)
            );
            otherPlayer.LoseGem();
            DropGem(GridIndexToWorldCenter(midGrid), otherGemKind);
            return true;
        }

        // ���3��б������(�����2)
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

        // ���4�����ڵ�(�κξ���)
        if (!Physics2D.Linecast(myPos, otherPos, obstacleLayer))
        {
            otherPlayer.LoseGem();
            DropGem(myPos, otherGemKind);
            return true;
        }
        Debug.Log("û�������κλ�������");
        return false;
    }
    private bool IsStraightLine(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Approximately(pos1.x, pos2.x) ||
               Mathf.Approximately(pos1.y, pos2.y);
    }

    private Vector2[] GetRemainingGridCorners(Vector2 gridIndex1, Vector2 gridIndex2)
    {
        // ����������ҵ���Է�λ
        bool targetIsRight = gridIndex2.x > gridIndex1.x;
        bool targetIsAbove = gridIndex2.y > gridIndex1.y;

        // ���ݷ�λȷ��ʣ����������
        if (targetIsRight && targetIsAbove) // Ŀ��������
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // ����
            new Vector2(gridIndex2.x, gridIndex1.y)  // ����
            };
        }
        else if (targetIsRight && !targetIsAbove) // Ŀ��������
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // ����
            new Vector2(gridIndex2.x, gridIndex1.y)  // ����
            };
        }
        else if (!targetIsRight && targetIsAbove) // Ŀ��������
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // ����
            new Vector2(gridIndex2.x, gridIndex1.y)  // ����
            };
        }
        else // Ŀ��������
        {
            return new Vector2[]
            {
            new Vector2(gridIndex1.x, gridIndex2.y), // ����
            new Vector2(gridIndex2.x, gridIndex1.y)  // ����
            };
        }
    }
    
    private Vector2? FindValidDropPosition(Vector2[] gridIndices)
    {
        // �ռ�������Чλ��
        List<Vector2> validPositions = new List<Vector2>();

        foreach (Vector2 gridIndex in gridIndices)
        {
            Vector2 worldPos = GridIndexToWorldCenter(gridIndex);

            // ����λ���Ƿ��ϰ����������ʯռ��
            bool positionBlocked = Physics2D.OverlapCircle(worldPos, 0.2f, obstacleLayer) != null ||
                                  Physics2D.OverlapCircle(worldPos, 0.2f, gemLayer) != null;

            if (!positionBlocked)
            {
                validPositions.Add(gridIndex);
            }
        }

        // ���ѡ��һ����Чλ��
        if (validPositions.Count > 0)
        {
            return validPositions[Random.Range(0, validPositions.Count)];
        }
        Debug.LogWarning("û���ҵ���Ч�ĵ���λ��");
        return null;
    }
    

    // ���������ȡ��Ӧ��ʯԤ����
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
            // ��Ӹ�������...
            default:
                Debug.LogWarning($"δ֪��ʯ����: {kind}, ʹ�ú�ɫ��ʯ��ΪĬ��");
                return redGemPrefab;
        }
    }
    //��ʯ����
    private void DropGem(Vector2 position, DiamondKinds kind)
    {
        GameObject prefab = GetGemPrefabByKind(kind);

        if (prefab != null)
        {
            GameObject newGem = Instantiate(prefab, position, Quaternion.identity);

            // ȷ����Diamond�������������(��ʹԤ��������)
            Diamond diamondComp = newGem.GetComponent<Diamond>();
            if (diamondComp == null)
            {
                diamondComp = newGem.AddComponent<Diamond>();
            }
            diamondComp.DiamondKinds = kind;

            // ȷ������ײ��
            if (newGem.GetComponent<Collider2D>() == null)
            {
                newGem.AddComponent<BoxCollider2D>();
            }

        }
        else
        {
            Debug.LogError($"{kind}��ʯԤ����δ���䣡����Inspector������");
        }
    }
    public void LoseGem()
    {
        isGetDiamond = false;
    }
    void AttackObstacle()//�ݻ�����
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
                Point--; // �����ж���
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
            inputControl.Disable(); // �ؼ������÷ǻ�Ծ��ҵ�����
        }
        lastSwitchTime = Time.time;// ��¼����ʱ��
    }

    private void Update()
    {
        

        //û��ui֮ǰ�Ŀ��ӻ���ʯ����
        if (isActive && isGetDiamond) GetComponent<SpriteRenderer>().color = Color.cyan;
        if (isActive && !isGetDiamond) GetComponent<SpriteRenderer>().color = Color.white;
        if (!isActive && !isGetDiamond) GetComponent<SpriteRenderer>().color = Color.gray;
        if (!isActive && isGetDiamond) GetComponent<SpriteRenderer>().color = Color.black;
        //
        if (isFrozen) GetComponent<SpriteRenderer>().color = Color.blue;
        //
        //���ⱻ���ָ��Ժ���
        //EnterSkillTargetingMode(currentActiveSkill);
        //ExitSkillTargetingMode();

        //�غϼ��+��ȴ
        if (!isActive || Time.time - lastSwitchTime < inputCooldown)
            return;
        
        if (isFrozen)
        {
            //isFrozen = false;
            return;
        }
        //���ܼ��
        if (isInSkillTargetingMode)
        {
            HandleSkillTargeting();
            return;
        }
        

        // ������⣨������ȴ��
        if (Input.GetKeyDown(KeyCode.Space) && Point > 0)
        {
            AttackObstacle();
            return; // �����������ƶ����
        }
        if (!isMoving && Point > 0)
        {
            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
            
            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                // ȷ���������ƶ�
                inputDirection=SnapTo4Directions(inputDirection);
                _lastMoveDirection = inputDirection;
                _lastMoveDirection = inputDirection.normalized;

                // �ƶ��߼�
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

    
    [Header("���÷�������")]
    public GameObject blockPrefab; // ��Inspector��������ķ���Ԥ����
    public LayerMask groundLayer; // ���õ���㼶������������߼�⣩
    private bool isPlacingMode = false; // �Ƿ��ڷ���ģʽ
    private Camera mainCamera;

    // �л�����ģʽ
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
            // �ؼ��޸ģ�ȷ��������뵽��������
            Vector2 placePos = new Vector2(
                Mathf.Floor(hit.point.x / gridSize) * gridSize + gridSize * 0.5f,
                Mathf.Floor(hit.point.y / gridSize) * gridSize + gridSize * 0.5f
            );

            // ������ʾ�������λ�ã����ӻ���飩
            Debug.DrawLine(placePos - Vector2.one * 0.5f, placePos + Vector2.one * 0.5f, Color.green, 2f);

            if (!Physics2D.OverlapCircle(placePos, 0.1f, obstacleLayer))
            {
                Instantiate(blockPrefab, placePos, Quaternion.identity);
                Point-=2;
            }
        }
    }

    [Header("����ϵͳ")]
    public List<SkillBase> skills = new List<SkillBase>(); // �����б�
    private SkillBase currentActiveSkill; // ��ǰ����ļ���(����Ŀ��ѡ��ģʽ)
    private bool isInSkillTargetingMode = false; // �Ƿ��ڼ���Ŀ��ѡ��ģʽ
    private bool movementEnabled = true;//�����ͷ��Ƿ������ƶ�
    //���ܳ�ʼ��
    void InitializeSkills()
    {
        // ��ȡ�����Ӷ����еļ������
        SkillBase[] childSkills = GetComponentsInChildren<SkillBase>();
        skills.AddRange(childSkills);

        // ��ʼ��ÿ������
        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }
    // ���뼼��Ŀ��ѡ��ģʽ
    public void EnterSkillTargetingMode(SkillBase skill)
    {
        isInSkillTargetingMode = true;
        currentActiveSkill = skill;
        movementEnabled = false; // ������ͨ�ƶ�
    }

    // �˳�����Ŀ��ѡ��ģʽ
    public void ExitSkillTargetingMode()
    {
        isInSkillTargetingMode = false;
        currentActiveSkill = null;
        movementEnabled = true; // �ָ��ƶ�
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
        // ���ﴦ����Ŀ��ѡ��ľ����߼�
        // ���繤��ʦ����ѡ���ϰ�����
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.CycleObstacleSelection();
        //}

        //// ȷ�ϼ����ͷ�
        //if (Input.GetMouseButtonDown(0))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.ConfirmPlacement();
        //}

        //// ȡ������
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    (currentActiveSkill as EngineerQuickBuildSkill)?.CancelSkill();
        //}
    }
    public void OnRoundStart()
    {
        // �������м�����ȴ
        foreach (var skill in skills)
        {
            skill.UpdateCooldown();
        }

        // ����UI
        //UIManager.Instance.UpdateActionPoints(currentActionPoints);
        //UIManager.Instance.UpdateSkillCooldowns();
    }
    public void SpendActionPoints(int amount)
    {
        Point = Mathf.Max(0, Point - amount);
        // �������������UI����
    }
    [Header("����״̬")]
    public bool isFrozen = false;
    public bool NegativeImpact = false;

}
