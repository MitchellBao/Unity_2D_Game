using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class GemSpawnData
    {
        public GameObject gemPrefab;
        public DiamondKinds gemKind;
        [Range(0, 100)] public int spawnProbability; // 百分比概率
    }

    [Header("生成设置")]
    public Transform spawnPoint; // 固定生成位置
    public List<GemSpawnData> gemTypes = new List<GemSpawnData>();
    public PointAreaControl[] pointAreas; // 两个得分点


    private int[] lastScores = new int[2];

    private void Start()
    {
        // 初始化记录上次分数
        for (int i = 0; i < pointAreas.Length; i++)
        {
            if (i < pointAreas.Length)
            {
                lastScores[i] = pointAreas[i].Point;
            }
        }
    }

    private void Update()
    {
        // 检查分数变化
        for (int i = 0; i < pointAreas.Length; i++)
        {
            if (i < pointAreas.Length && pointAreas[i] != null)
            {
                if (pointAreas[i].Point != lastScores[i])
                {
                    OnScoreChanged();
                    lastScores[i] = pointAreas[i].Point;
                }
            }
        }
    }

    private void OnScoreChanged()
    {
        if (spawnPoint == null || gemTypes.Count == 0)
        {
            Debug.LogWarning("生成点或宝石类型未设置!");
            return;
        }

        // 计算总概率
        int totalProbability = 0;
        foreach (var gem in gemTypes)
        {
            totalProbability += gem.spawnProbability;
        }

        // 随机选择宝石
        int randomValue = Random.Range(0, totalProbability);
        int cumulativeProbability = 0;
        GameObject gemToSpawn = null;
        DiamondKinds gemKind = DiamondKinds.redDiamond;

        foreach (var gem in gemTypes)
        {
            cumulativeProbability += gem.spawnProbability;
            if (randomValue < cumulativeProbability)
            {
                gemToSpawn = gem.gemPrefab;
                gemKind = gem.gemKind;
                break;
            }
        }

        // 生成宝石
        if (gemToSpawn != null)
        {
            GameObject newGem = Instantiate(gemToSpawn, spawnPoint.position, Quaternion.identity);
            Diamond diamondComp = newGem.GetComponent<Diamond>();
            if (diamondComp == null)
            {
                diamondComp = newGem.AddComponent<Diamond>();
            }
            diamondComp.DiamondKinds = gemKind;

            Debug.Log($"生成了 {gemKind} 宝石在 {spawnPoint.position}");
        }
    }

    
}