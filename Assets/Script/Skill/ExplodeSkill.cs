using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodeSkill : SkillBase
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleBackgroundLayer;
    private float gridSize = 1f;
    private Vector2 boxSize;
    private void Start()
    {
        boxSize = new Vector2(0.9f, 0.9f);
    }
    public void Check5x5Obstacles()
    {
        Vector2 center = transform.position;
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                Vector2 checkPos = center + new Vector2(i * gridSize, j * gridSize);
                Collider2D obstacle = Physics2D.OverlapBox(checkPos, boxSize, 0, obstacleLayer);
                if (obstacle != null)
                {
                    Destroy(obstacle.gameObject);
                }
            }
        }

    }

    public void Check5x5Player()
    {
        Vector2 center = transform.position;
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                Vector2 checkPos = center + new Vector2(i * gridSize, j * gridSize);
                Collider2D player = Physics2D.OverlapBox(checkPos, boxSize, 0, playerLayer);
                if (player != null)
                {
                    PlayerControl enimePlayer = player.gameObject.GetComponent<PlayerControl>();
                    if (enimePlayer.isGetDiamond)
                    {
                        enimePlayer.isGetDiamond = false;
                        int nums = 0;
                        Vector2[] possiblePositions = Get3x3Position(player.transform.position, out nums);
                        Vector2 spawnPos = possiblePositions[Random.Range(0, nums-1)];

                        Instantiate(enimePlayer.GetGemPrefabByKind(enimePlayer.diamondKind),
                            spawnPos, Quaternion.identity);

                    }
                }
            }
        }

    }

    private Vector2[] Get3x3Position(Vector2 center, out int nums)
    {
        Vector2[] result = new Vector2[8];
        int index = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; //         
                Vector2 checkPos = center + new Vector2(i * gridSize, j * gridSize);
                Collider2D player = Physics2D.OverlapBox(checkPos, boxSize, 0, obstacleBackgroundLayer);
                if (player != null) continue;
                result[index] = center + new Vector2(i * gridSize, j * gridSize);
                index++;
            }
        }
        nums = index;
        return result;
    }

    protected override void ExecuteSkill()
    {
        Check5x5Obstacles();
        Check5x5Player();
    }

}
