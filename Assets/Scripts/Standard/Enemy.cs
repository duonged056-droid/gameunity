using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Lộ trình Điểm đầu -> Điểm cuối")]
    public Transform startPoint;
    public Transform endPoint;
    public float baseSpeed = 2.5f;
    private float currentSpeed;

    [Header("Chỉ số & Đặc tính Quái")]
    public int maxHP = 50;
    private int currentHP;
    public bool isBomber = false; // Tích chọn nếu là Saibamen mang bom nổ
    public int damageToBase = 10;
    public int rewardGold = 15;

    void Start()
    {
        currentHP = maxHP;
        currentSpeed = baseSpeed;
        if (startPoint != null) transform.position = startPoint.position;
    }

    void Update()
    {
        if (endPoint == null) return;

        // Quái mang bom nổ di chuyển với tốc độ nhanh - chậm thất thường
        if (isBomber)
        {
            currentSpeed = baseSpeed + Mathf.Sin(Time.time * 6f) * 1.5f;
        }

        transform.position = Vector2.MoveTowards(transform.position, endPoint.position, currentSpeed * Time.deltaTime);
    }

    public void SetupEnemy(Transform start, Transform end, float healthMultiplier, float speedBonus)
    {
        startPoint = start;
        endPoint = end;
        if (startPoint != null) transform.position = startPoint.position;

        maxHP = Mathf.RoundToInt(maxHP * healthMultiplier);
        currentHP = maxHP;
        baseSpeed += speedBonus;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddResource(rewardGold);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Base"))
        {
            int finalDamage = isBomber ? damageToBase * 3 : damageToBase;
            if (GameManager.instance != null)
            {
                GameManager.instance.TakeBaseDamage(finalDamage);
            }
            Destroy(gameObject);
        }
    }
}
