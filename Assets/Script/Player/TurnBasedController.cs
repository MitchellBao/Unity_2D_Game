using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnBasedController : MonoBehaviour
{
    public PointAreaControl p1, p2;
    public PlayerInputControl inputControl; // �������ϵͳ
    public PlayerControl[] players;        // ������ɫ�Ŀ��ƽű�
    private int currentPlayerIndex = 1;    // ��ǰ���ƵĽ�ɫ����
    public int maxRounds = 100;     // ���غ���
    public int currentRound = 0;   // ��ǰ�غ���
    //public Text roundText;          // UI��ʾ�غ�����Text���
    public LayerMask playerLayer; // ��Inspector��������Ҫ���Ĳ㼶
    public int characterIndex01;
    public int characterIndex02;

    [Header("Events")]
    public UnityEvent<TurnBasedController> OnTurnChange;
    public UnityEvent<TurnBasedController> OnPointChange;
    public UnityEvent<TurnBasedController> OnGameOver;

    void Start()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        spawner.SpawnSelectedPlayers(characterIndex01, characterIndex02); // ѡ���ɫ
        OnPointChange?.Invoke(this);
    }
    public void InitializePlayers(PlayerControl[] newPlayers)
    {
        List<PlayerControl> validPlayers = new List<PlayerControl>();

        foreach (var player in newPlayers)
        {
            if (IsInPlayerLayer(player.gameObject))
            {
                validPlayers.Add(player);
            }
        }

        players = validPlayers.ToArray();
    }

    private void Awake()
    {
        inputControl = new PlayerInputControl();

        // ��ȡ����PlayerControl���
        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        List<PlayerControl> filteredPlayers = new List<PlayerControl>();

        foreach (PlayerControl player in allPlayers)
        {
            // �������Ƿ���ָ����
            if (IsInPlayerLayer(player.gameObject))
            {
                filteredPlayers.Add(player);
            }
        }

        players = filteredPlayers.ToArray();
    }

    // �����Ĳ��ⷽ��
    private bool IsInPlayerLayer(GameObject obj)
    {
        // ʹ��λ�����������
        return (playerLayer.value & (1 << obj.layer)) != 0;
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
        OnPointChange?.Invoke(this);
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
        OnTurnChange?.Invoke(this);
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
        
        if (p1.Point == p2.Point)
            Debug.Log($"��Ϸ���� ƽ��");
        else
        {
            int playerid= p1.Point > p2.Point ? 1: 2;
            Debug.Log($"��Ϸ����,���:{playerid}��ʤ�� ");
        }

        OnGameOver?.Invoke(this);
        // ֹͣ��������
        inputControl.Disable();

        // ����������ҿ���
        foreach (var player in players)
        {
            player.SetActive(false);
        }

        
        
    }
}
