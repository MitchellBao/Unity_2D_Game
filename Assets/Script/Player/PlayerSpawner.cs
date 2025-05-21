using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int characterID;
        public GameObject characterPrefab;
    }

    [Header("角色配置")]
    public List<PlayerData> availableCharacters = new List<PlayerData>();
    public Transform[] spawnPoints = new Transform[2]; // 两个固定生成位置

    [Header("层级设置")]
    public string baseLayerName = "PlayerBase";
    public string playerLayerName = "Player";

    private void Start()
    {
        // 确保有两个生成点
        if (spawnPoints.Length != 2)
        {
            Debug.LogError("需要设置2个生成点！");
            return;
        }
    }
    private void InitializeTurnController()
    {
        TurnBasedController controller = FindObjectOfType<TurnBasedController>();
        if (controller != null)
        {
            // 获取所有PlayerControl组件
            PlayerControl[] players = FindObjectsOfType<PlayerControl>();

            // 确保只获取我们生成的玩家
            if (players.Length >= 2)
            {
                controller.InitializePlayers(players);
            }
            else
            {
                Debug.LogError("未找到足够的玩家对象！");
            }
        }
    }
    //外部调用的生成方法
    public void SpawnSelectedPlayers(int player1ID, int player2ID)
    {
        // 清除场景中可能存在的旧角色
        //ClearExistingPlayers();

        // 生成玩家1,2
        SpawnPlayer(player1ID, 0);
        SpawnPlayer(player2ID, 1);
        // 通知回合控制器初始化
        InitializeTurnController();
    }

    private void SpawnPlayer(int characterID, int spawnIndex)
    {
        // 查找对应的预制体
        GameObject prefabToSpawn = null;
        foreach (var character in availableCharacters)
        {
            if (character.characterID == characterID)
            {
                prefabToSpawn = character.characterPrefab;
                break;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError($"找不到ID为 {characterID} 的角色预制体");
            return;
        }

        if (spawnIndex >= spawnPoints.Length || spawnPoints[spawnIndex] == null)
        {
            Debug.LogError($"生成点 {spawnIndex} 无效");
            return;
        }

        // 实例化角色
        GameObject newPlayer = Instantiate(
            prefabToSpawn,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation);

        // 设置层级
        SetLayerRecursively(newPlayer, playerLayerName);

        Debug.Log($"生成角色ID {characterID} 在位置 {spawnIndex}");
    }

    // 递归设置层级
    private void SetLayerRecursively(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"层级 '{layerName}' 不存在！");
            return;
        }

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerName);
        }
    }

    private void ClearExistingPlayers()
    {
        // 查找所有现有玩家对象
        PlayerControl[] existingPlayers = FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl player in existingPlayers)
        {
            Destroy(player.gameObject);
        }
    }
}