using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedController : MonoBehaviour
{
    public PointAreaControl p1, p2;
    public PlayerInputControl inputControl; // 你的输入系统
    public PlayerControl[] players;        // 两个角色的控制脚本
    private int currentPlayerIndex = 0;    // 当前控制的角色索引
    public int maxRounds = 10;     // 最大回合数
    private int currentRound = 0;   // 当前回合数
    //public Text roundText;          // UI显示回合数的Text组件
    public LayerMask playerLayer; // 在Inspector中设置需要检测的层级
    void Start()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        spawner.SpawnSelectedPlayers(1, 3); // 选择角色
    }
    //private void Awake()
    //{
    //    inputControl = new PlayerInputControl();
    //    players = FindObjectsOfType<PlayerControl>(); // 获取所有角色
    //}

    public void InitializePlayers(PlayerControl[] newPlayers)
    {
        players = newPlayers;
        currentPlayerIndex = 0;
        currentRound = 1;

        // 初始化第一个玩家
        if (players.Length > 0)
        {
            players[0].OnRoundStart();
            UpdateControlState();

            // 调试颜色
        }
    }

    private void Awake()
    {
        inputControl = new PlayerInputControl();

        // 直接获取"Player"层的掩码（也可以在Inspector设置）
        int targetLayerMask = LayerMask.GetMask("player");

        // 或者使用你在Inspector设置的playerLayer（确保设置正确）
        // int targetLayerMask = playerLayer.value;

        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        List<PlayerControl> validPlayers = new List<PlayerControl>();

        foreach (var player in allPlayers)
        {
            // 检查对象层级是否在目标层级掩码中
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
        inputControl.GamePlay.FinishRound.performed += _ => SwitchCharacter();//主动结束
        UpdateControlState(); // 初始化控制状态
    }
    private void Update()
    {
        // 持续检测当前角色行动点是否耗尽
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

    //切换角色控制权
    private void SwitchCharacter()
    {
        // 检查是否冻结
        if (players[currentPlayerIndex].isFrozen)
        {
            players[currentPlayerIndex].isFrozen = false;
            Debug.Log($"{name} 解冻了!");
        }
        int previousIndex = currentPlayerIndex;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        if (currentPlayerIndex == 0)
        {
            currentRound++;
            UpdateRoundUI();

            // 检查回合限制
            if (currentRound >= maxRounds)
            {
                GameOver();
                return;
            }
        }

        PlayerControl nextPlayer = players[currentPlayerIndex];
        // 正常回合逻辑
        nextPlayer.OnRoundStart();
        UpdateControlState();
        Debug.Log($"切换到角色: {currentPlayerIndex + 1}");
    }
    
    // 更新角色输入状态
    private void UpdateControlState()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i == currentPlayerIndex); // 只有当前角色可操作
        }
    }
    private void UpdateRoundUI()
    {
        //if (roundText != null)
        //    roundText.text = $"回合: {currentRound}/{maxRounds}";
    }

    private void GameOver()
    {
        if(p1.Point == p2.Point)
            Debug.Log($"游戏结束 平局");
        else
        {
            int playerid= p1.Point > p2.Point ? 1: 2;
            Debug.Log($"游戏结束,玩家:{playerid}获胜！ ");
        }

        // 停止所有输入
        inputControl.Disable();

        // 禁用所有玩家控制
        foreach (var player in players)
        {
            player.SetActive(false);
        }

        // 这里可以添加显示结算界面：UIManager.Instance.ShowGameOver();
    }
}
