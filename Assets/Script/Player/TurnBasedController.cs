using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedController : MonoBehaviour
{
    public PointAreaControl p1, p2;
    public PlayerInputControl inputControl; // �������ϵͳ
    public PlayerControl[] players;        // ������ɫ�Ŀ��ƽű�
    private int currentPlayerIndex = 0;    // ��ǰ���ƵĽ�ɫ����
    public int maxRounds = 10;     // ���غ���
    private int currentRound = 0;   // ��ǰ�غ���
    //public Text roundText;          // UI��ʾ�غ�����Text���
    public LayerMask playerLayer; // ��Inspector��������Ҫ���Ĳ㼶
    void Start()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        spawner.SpawnSelectedPlayers(1, 3); // ѡ���ɫ
    }
    //private void Awake()
    //{
    //    inputControl = new PlayerInputControl();
    //    players = FindObjectsOfType<PlayerControl>(); // ��ȡ���н�ɫ
    //}

    public void InitializePlayers(PlayerControl[] newPlayers)
    {
        players = newPlayers;
        currentPlayerIndex = 0;
        currentRound = 1;

        // ��ʼ����һ�����
        if (players.Length > 0)
        {
            players[0].OnRoundStart();
            UpdateControlState();

            // ������ɫ
        }
    }

    private void Awake()
    {
        inputControl = new PlayerInputControl();

        // ֱ�ӻ�ȡ"Player"������루Ҳ������Inspector���ã�
        int targetLayerMask = LayerMask.GetMask("player");

        // ����ʹ������Inspector���õ�playerLayer��ȷ��������ȷ��
        // int targetLayerMask = playerLayer.value;

        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        List<PlayerControl> validPlayers = new List<PlayerControl>();

        foreach (var player in allPlayers)
        {
            // ������㼶�Ƿ���Ŀ��㼶������
            if ((targetLayerMask & (1 << player.gameObject.layer)) != 0)
            {
                validPlayers.Add(player);
            }
        }

        players = validPlayers.ToArray();
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
        if (players[currentPlayerIndex].Point <= 0&& !players[currentPlayerIndex].isFrozen&& !players[currentPlayerIndex].duringSkill)
        {
            SwitchCharacter();
        }
    }
    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.GamePlay.FinishRound.performed -= _ => SwitchCharacter();
    }

    //�л���ɫ����Ȩ
    private void SwitchCharacter()
    {
        // ����Ƿ񶳽�
        if (players[currentPlayerIndex].isFrozen)
        {
            players[currentPlayerIndex].isFrozen = false;
            Debug.Log($"{name} �ⶳ��!");
        }
        int previousIndex = currentPlayerIndex;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        if (currentPlayerIndex == 0)
        {
            currentRound++;
            UpdateRoundUI();

            // ���غ�����
            if (currentRound >= maxRounds)
            {
                GameOver();
                return;
            }
        }

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
    private void UpdateRoundUI()
    {
        //if (roundText != null)
        //    roundText.text = $"�غ�: {currentRound}/{maxRounds}";
    }

    private void GameOver()
    {
        if(p1.Point == p2.Point)
            Debug.Log($"��Ϸ���� ƽ��");
        else
        {
            int playerid= p1.Point > p2.Point ? 1: 2;
            Debug.Log($"��Ϸ����,���:{playerid}��ʤ�� ");
        }

        // ֹͣ��������
        inputControl.Disable();

        // ����������ҿ���
        foreach (var player in players)
        {
            player.SetActive(false);
        }

        // ������������ʾ������棺UIManager.Instance.ShowGameOver();
    }
}
