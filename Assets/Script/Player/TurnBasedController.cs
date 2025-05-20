using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedController : MonoBehaviour
{
    public PlayerInputControl inputControl; // �������ϵͳ
    public PlayerControl[] players;        // ������ɫ�Ŀ��ƽű�
    private int currentPlayerIndex = 0;    // ��ǰ���ƵĽ�ɫ����

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        players = FindObjectsOfType<PlayerControl>(); // ��ȡ���н�ɫ
    }

    private void OnEnable()
    {
        inputControl.Enable();
        inputControl.GamePlay.FinishRound.performed += _ => SwitchCharacter();//��������
        UpdateControlState(); // ��ʼ������״̬
    }
    private void Update()
    {
        // ������⵱ǰ��ɫ�ж����Ƿ�ľ�
        if (players[currentPlayerIndex].Point <= 0&& !players[currentPlayerIndex].isFrozen)
        {
            SwitchCharacter();
        }
    }
    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.GamePlay.FinishRound.performed -= _ => SwitchCharacter();
    }

    // �л���ɫ����Ȩ
    private void SwitchCharacter()
    {
        // ����Ƿ񶳽�
        if (players[currentPlayerIndex].isFrozen)
        {
            players[currentPlayerIndex].isFrozen = false;
            Debug.Log($"{name} �ⶳ��!");
        }
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        PlayerControl nextPlayer = players[currentPlayerIndex];
        // �����غ��߼�
        nextPlayer.OnRoundStart();
        UpdateControlState();
        Debug.Log($"�л�����ɫ: {currentPlayerIndex + 1}");
    }
    // ���½�ɫ����״̬
    private void UpdateControlState()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i == currentPlayerIndex); // ֻ�е�ǰ��ɫ�ɲ���
        }
    }
}
