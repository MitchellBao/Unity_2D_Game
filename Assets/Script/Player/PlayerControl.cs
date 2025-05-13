
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControl : MonoBehaviour
{
    public int Point = 100; // �ж���
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    public Rigidbody2D rb;
    public float gridSize = 1f; // ÿ��Ĵ�С
    public LayerMask obstacleLayer; // �ϰ���㼶����Unity�༭�������ã�
    private bool isMoving = false; // �Ƿ������ƶ�
    public bool isGetDiamond;
    public DiamondKinds diamondKind;
    private float inputCooldown = 0.2f; // ���뻺��ʱ�� �����л���ɫʱ��������
    private float lastSwitchTime;

    private Vector2 _lastMoveDirection = Vector2.right; // Ĭ�ϳ���
    public Vector2 LastMoveDirection => _lastMoveDirection; // ����ֻ������
   

    private Vector2 SnapTo4Directions(Vector2 dir)//�ƶ������������ĸ�����
    {
        // ���������������ĸ�����
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
                Point--; // �����ж���
            }
        }
    }

    // �Զ�����Instant�ϰ�
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("InstantObstacle"))
        {
            col.gameObject.GetComponent<Obstacle>()?.TakeDamage();
        }
    }

    //private void UpdateLastDirection(Vector2 currentInput)// ���·���
    //{
    //    // ֻ��������ʱ�Ÿ��·��򣨱��⾲ֹʱ���ǣ�
    //    if (currentInput != Vector2.zero)
    //    {
    //        _lastMoveDirection = currentInput.normalized; // �洢��׼������
    //    }
    //}

    private void Awake()
    {
        inputControl = new PlayerInputControl();
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
        inputControl.GamePlay.UseSkill1.performed += _ => TryUseSkill(0); // ����1 -> ����1
    }

    private void OnDisable()
    {
        inputControl.Disable();
        // �Ƴ�����
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

            lastSwitchTime = Time.time;// ��¼����ʱ��
        }
    }
    private void Update()
    {
        //�غϼ��+��ȴ
        if (!isActive || Time.time - lastSwitchTime < inputCooldown)
            return;
        //���ܼ��
        if(isInSkillTargetingMode)
        {
            HandleSkillTargeting();
            return;
        }
        // ��Ӽ��ܿ�ݼ� (ʾ�������ּ�1-4)
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryUseSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryUseSkill(1);

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
                //inputDirection = inputDirection.normalized;
                //Vector2 targetPosition = rb.position + inputDirection * gridSize;
                //// ��ײ��⣺���Ŀ��λ���Ƿ����ϰ���
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
        if (!isPlacingMode || Point <= 0) return;

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
    void Start()
    {
        // �Զ���ȡ�����Ӷ����еļ���
        skills.AddRange(GetComponentsInChildren<SkillBase>());

        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }
}
