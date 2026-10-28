using UnityEngine;

public class KiBlast : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                int dmg = GameManager.instance != null ? GameManager.instance.attackDamage : 25;
                enemy.TakeDamage(dmg);
            }
            Destroy(gameObject);
        }
    }
}
