
using System.Collections;
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

    private void Update()
    {
        if (!isMoving && Point > 0)
        {
            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                // ȷ���������ƶ�
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

                // ��ײ��⣺���Ŀ��λ���Ƿ����ϰ���
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
                Point--;
            }
        }
    }
}
