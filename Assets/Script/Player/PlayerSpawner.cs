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

    [Header("��ɫ����")]
    public List<PlayerData> availableCharacters = new List<PlayerData>();
    public Transform[] spawnPoints = new Transform[2]; // �����̶�����λ��

    [Header("�㼶����")]
    public string baseLayerName = "PlayerBase";
    public string playerLayerName = "Player";

    private void Start()
    {
        // ȷ�����������ɵ�
        if (spawnPoints.Length != 2)
        {
            Debug.LogError("��Ҫ����2�����ɵ㣡");
            return;
        }
    }
    private void InitializeTurnController()
    {
        TurnBasedController controller = FindObjectOfType<TurnBasedController>();
        if (controller != null)
        {
            // ��ȡ����PlayerControl���
            PlayerControl[] players = FindObjectsOfType<PlayerControl>();

            // ȷ��ֻ��ȡ�������ɵ����
            if (players.Length >= 2)
            {
                controller.InitializePlayers(players);
            }
            else
            {
                Debug.LogError("δ�ҵ��㹻����Ҷ���");
            }
        }
    }
    //�ⲿ���õ����ɷ���
    public void SpawnSelectedPlayers(int player1ID, int player2ID)
    {
        // ��������п��ܴ��ڵľɽ�ɫ
        //ClearExistingPlayers();

        // �������1,2
        SpawnPlayer(player1ID, 0);
        SpawnPlayer(player2ID, 1);
        // ֪ͨ�غϿ�������ʼ��
        InitializeTurnController();
    }

    private void SpawnPlayer(int characterID, int spawnIndex)
    {
        // ���Ҷ�Ӧ��Ԥ����
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
            Debug.LogError($"�Ҳ���IDΪ {characterID} �Ľ�ɫԤ����");
            return;
        }

        if (spawnIndex >= spawnPoints.Length || spawnPoints[spawnIndex] == null)
        {
            Debug.LogError($"���ɵ� {spawnIndex} ��Ч");
            return;
        }

        // ʵ������ɫ
        GameObject newPlayer = Instantiate(
            prefabToSpawn,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation);

        // ���ò㼶
        SetLayerRecursively(newPlayer, playerLayerName);

        Debug.Log($"���ɽ�ɫID {characterID} ��λ�� {spawnIndex}");
    }

    // �ݹ����ò㼶
    private void SetLayerRecursively(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"�㼶 '{layerName}' �����ڣ�");
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
        // ��������������Ҷ���
        PlayerControl[] existingPlayers = FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl player in existingPlayers)
        {
            Destroy(player.gameObject);
        }
    }
}