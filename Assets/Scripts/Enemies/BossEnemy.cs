using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(EnemyHealth))]
    public class BossEnemy : MonoBehaviour
    {
        public string bossName = "Frieza";
        public float attackInterval = 2.5f;
        public int attackDamage = 40;
        public GameObject bossProjectilePrefab;

        private float attackTimer = 0f;
        private EnemyHealth health;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowBossWarning(bossName);
            }
        }

        private void Update()
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                ShootAtDefenders();
            }
        }

        private void ShootAtDefenders()
        {
            Vector3 spawnPos = transform.position + Vector3.left * 0.8f;
            Vector3 targetPos = DragonBallAltar.Instance != null ? DragonBallAltar.Instance.transform.position : transform.position + Vector3.left * 10f;

            // Target Goku if nearby
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null && Random.value < 0.6f)
            {
                targetPos = player.transform.position;
            }

            Vector2 dir = (targetPos - spawnPos).normalized;

            GameObject proj = new GameObject("BossEnergyBlast");
            proj.transform.position = spawnPos;
            proj.transform.localScale = Vector3.one * 0.8f;

            SpriteRenderer sr = proj.AddComponent<SpriteRenderer>();
            sr.color = new Color(1f, 0.2f, 0.5f, 0.9f); // Pink/purple death beam or red blast
            sr.sortingOrder = 12;

            CircleCollider2D col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            BossProjectile bp = proj.AddComponent<BossProjectile>();
            bp.Initialize(dir, attackDamage);

            SoundManager.Instance?.PlayKiBlastSound(0.2f);
        }
    }

    public class BossProjectile : MonoBehaviour
    {
        private Vector2 direction;
        private int damage;
        private float speed = 7f;

        public void Initialize(Vector2 dir, int dmg)
        {
            direction = dir;
            damage = dmg;
            Destroy(gameObject, 4f);
        }

        private void Update()
        {
            transform.Translate(direction * (speed * Time.deltaTime), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Base"))
            {
                DragonBallAltar altar = collision.GetComponent<DragonBallAltar>();
                if (altar != null) altar.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (collision.CompareTag("Defender"))
            {
                TurretAlly ally = collision.GetComponent<TurretAlly>();
                if (ally != null) ally.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
