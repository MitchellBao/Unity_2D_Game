
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


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
    }
    private void OnEnable()
    {
        inputControl.Enable();
        // ����E���������
        inputControl.GamePlay.PlaceBlock.performed += _ => TogglePlacementMode(true);
        inputControl.GamePlay.PlaceBlock.canceled += _ => TogglePlacementMode(false);
        // �������������
        inputControl.GamePlay.MouseClick.performed += OnMouseClick;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        // �Ƴ�����
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

            lastSwitchTime = Time.time;// ��¼����ʱ��
        }
    }
    private void Update()
    {
        //�غϼ��+��ȴ
        if (!isActive || Time.time - lastSwitchTime < inputCooldown)
            return;

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
    //private void OnMouseClick(InputAction.CallbackContext context)
    //{
    //    if (!isPlacingMode || Point <= 0) return;

    //    Vector2 mousePos = inputControl.GamePlay.MousePosition.ReadValue<Vector2>();
    //    Ray ray = mainCamera.ScreenPointToRay(mousePos);
    //    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);

    //    if (hit.collider != null)
    //    {
    //        // �ؼ��޸ģ�ȷ��������뵽��������
    //        Vector2 placePos = new Vector2(
    //            Mathf.Floor(hit.point.x / gridSize) * gridSize + gridSize * 0.5f,
    //            Mathf.Floor(hit.point.y / gridSize) * gridSize + gridSize * 0.5f
    //        );

    //        // ������ʾ�������λ�ã����ӻ���飩
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
        float gridSize = 1.0f; // ��������ƶ����һ��
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
                SetRandomHardness(newBox); // �������Ӳ��
                newBox.GetComponent<Obstacle>().UpdateAppearance();
                Point -=2;
            }
        }
    }

    // �����������Ӳ��
    private void SetRandomHardness(GameObject box)
    {
        Obstacle obstacle = box.GetComponent<Obstacle>();
        if (obstacle == null) return;

        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= 0.7f) // 70%����Ӳ��1
        {
            obstacle.hardness = Obstacle.HardnessLevel.Fragile;
            obstacle.health = 1;
        }
        else // 30%����Ӳ��2
        {
            obstacle.hardness = Obstacle.HardnessLevel.Sturdy;
            obstacle.health = 2;
        }

        // ������ۣ�����в�ͬ��ͼ��
        obstacle.UpdateAppearance();
    }
}
