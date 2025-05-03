//using System.Collections;
//using System.Collections.Generic;
//using System.Drawing.Text;
//using UnityEngine;
//using UnityEngine.InputSystem;


//public class PlayerMove : MonoBehaviour
//{
//    public int Point=100;//�ж���
//    public PlayerInputControl inputControl;
//    public Vector2 inputDirection;
//    public Rigidbody2D rb;
//    public float gridSize = 1f; // ÿ��Ĵ�С
//    private bool isMoving = false; // �Ƿ������ƶ�

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
//        // ֻ��û���ƶ�ʱ�������
//        if (!isMoving)
//        {
//            inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

//            // ����Ƿ��з�������루����б���ƶ���
//            if (Point>0&&(inputDirection.x != 0 || inputDirection.y != 0))
//            {
//                // ȷ��ֻ��һ�������ƶ�������б���ƶ���
//                if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
//                {
//                    inputDirection.y = 0;
//                }
//                else
//                {
//                    inputDirection.x = 0;
//                }

//                // ��һ������ȷ��ÿ���ƶ�һ��
//                inputDirection = inputDirection.normalized;
//                StartCoroutine(MoveOneStep());
//            }
//        }
//    }

//    // Э�̣��ƶ�һ��
//    IEnumerator MoveOneStep()
//    {
//        isMoving = true;
//        Point--;
//        Vector2 startPosition = rb.position;
//        Vector2 targetPosition = startPosition + inputDirection * gridSize;

//        float moveTime = 0.2f; // �ƶ�ʱ��
//        float elapsedTime = 0f;

//        while (elapsedTime < moveTime)
//        {
//            rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        rb.position = targetPosition; // ȷ������λ��׼ȷ
//        isMoving = false;
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
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
