using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum HardnessLevel
    {
        Fragile = 1,  // 1�ι���
        Sturdy = 2    // 2�ι���
    }

    [Header("��������")]
    public HardnessLevel hardness = HardnessLevel.Fragile;
    public int health { get; private set; }

    [Header("�Ӿ�Ч��")]
    public Sprite intactSprite;    // ����״̬
    public Sprite damagedSprite;   // ����״̬����Sturdy��Ҫ��
    private SpriteRenderer spriteRenderer;

    [Header("����Ч��")]
    public ParticleSystem destroyEffect;
    public AudioClip destroySound;
    // Obstacle.cs
    
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = (int)hardness; // ����Ӳ�ȵȼ���ʼ��Ѫ��
    }

    // ���������̤ʱ����
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

    void UpdateAppearance()
    {
        if (hardness == HardnessLevel.Sturdy && health == 1)
        {
            spriteRenderer.sprite = damagedSprite;
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

    // ����������ת��Ϊ������������
    Vector2 GetGridPosition(Vector2 worldPos)
    {
        float gridSize = 1.0f; // ��������ƶ����һ��
        return new Vector2(
            Mathf.Floor(worldPos.x / gridSize) * gridSize + gridSize * 0.5f,
            Mathf.Floor(worldPos.y / gridSize) * gridSize + gridSize * 0.5f
        );
    }
}
