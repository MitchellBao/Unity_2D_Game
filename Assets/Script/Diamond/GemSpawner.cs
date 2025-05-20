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
        [Range(0, 100)] public int spawnProbability; // �ٷֱȸ���
    }

    [Header("��������")]
    public Transform spawnPoint; // �̶�����λ��
    public List<GemSpawnData> gemTypes = new List<GemSpawnData>();
    public PointAreaControl[] pointAreas; // �����÷ֵ�


    private int[] lastScores = new int[2];

    private void Start()
    {
        // ��ʼ����¼�ϴη���
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
        // �������仯
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
            Debug.LogWarning("���ɵ��ʯ����δ����!");
            return;
        }

        // �����ܸ���
        int totalProbability = 0;
        foreach (var gem in gemTypes)
        {
            totalProbability += gem.spawnProbability;
        }

        // ���ѡ��ʯ
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

        // ���ɱ�ʯ
        if (gemToSpawn != null)
        {
            GameObject newGem = Instantiate(gemToSpawn, spawnPoint.position, Quaternion.identity);
            Diamond diamondComp = newGem.GetComponent<Diamond>();
            if (diamondComp == null)
            {
                diamondComp = newGem.AddComponent<Diamond>();
            }
            diamondComp.DiamondKinds = gemKind;

            Debug.Log($"������ {gemKind} ��ʯ�� {spawnPoint.position}");
        }
    }

    
}