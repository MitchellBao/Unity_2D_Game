using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = (int)hardness;
        ForceUpdateAppearance(); // 初始化时强制更新
    }
    // 新增方法：强制更新外观
    public void ForceUpdateAppearance()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        switch (hardness)
        {
            case HardnessLevel.Fragile:
                spriteRenderer.sprite = Sprite1;
                break;
            case HardnessLevel.Sturdy:
                spriteRenderer.sprite = (health >= 2) ? Sprite2 : damagedSprite;
                break;
        }
    }

    public void UpdateAppearance()
    {
        if (hardness == HardnessLevel.Sturdy && health == 1)
        {
            spriteRenderer.sprite = damagedSprite;
        }
        else
        {
            ForceUpdateAppearance(); // 其他情况也强制更新
        }
    }
    public enum HardnessLevel
    {
        Fragile = 1,  // 1次攻击
        Sturdy = 2    // 2次攻击
    }

    [Header("基础设置")]
    public HardnessLevel hardness = HardnessLevel.Fragile;
    public int health { get; set; }

    [Header("视觉效果")]
    public Sprite Sprite1;    // 1完整状态
    public Sprite Sprite2;    // 2完整状态
    public Sprite damagedSprite;   // 受损状态（仅Sturdy需要）
    private SpriteRenderer spriteRenderer;

    [Header("交互效果")]
    public ParticleSystem destroyEffect;
    public AudioClip destroySound;
    // Obstacle.cs
    
    
    

    // 被攻击或踩踏时调用
    public void TakeDamage()
    {
        if (health <= 0) return;

        health--;
        UpdateAppearance();

        if (health <= 0)
        {
            DestroyObstacle();
        }
    }

    

    void DestroyObstacle()
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        if (destroySound != null)
            AudioSource.PlayClipAtPoint(destroySound, transform.position);

        Destroy(gameObject);
    }

    // 将世界坐标转换为网格中心坐标
    Vector2 GetGridPosition(Vector2 worldPos)
    {
        float gridSize = 1.0f; // 需与你的移动格距一致
        return new Vector2(
            Mathf.Floor(worldPos.x / gridSize) * gridSize + gridSize * 0.5f,
            Mathf.Floor(worldPos.y / gridSize) * gridSize + gridSize * 0.5f
        );
    }
}
