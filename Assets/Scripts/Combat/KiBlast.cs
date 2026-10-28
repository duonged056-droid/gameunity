using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class KiBlast : MonoBehaviour
    {
        [Header("Projectile Stats")]
        public float speed = 14f;
        public int damage = 25;
        public bool isCritical = false;
        public float lifetime = 3.5f;

        [Header("Effects")]
        [SerializeField] private GameObject impactEffectPrefab;

        private Vector2 moveDirection = Vector2.right;

        private void Awake()
        {
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            gameObject.tag = "Projectile";
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        public void Initialize(Vector2 direction, int dmg, bool crit = false)
        {
            moveDirection = direction.normalized;
            damage = dmg;
            isCritical = crit;

            // Rotate sprite towards direction
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Update()
        {
            transform.Translate(moveDirection * (speed * Time.deltaTime), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, isCritical);
                }

                SpawnImpactEffect();
                Destroy(gameObject);
            }
        }

        private void SpawnImpactEffect()
        {
            SoundManager.Instance?.PlayExplosionSound(0.25f);

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                // Procedural flash impact
                GameObject flashObj = new GameObject("KiHitFlash");
                flashObj.transform.position = transform.position;
                SpriteRenderer sr = flashObj.AddComponent<SpriteRenderer>();
                sr.color = isCritical ? new Color(1f, 0.9f, 0.2f, 0.9f) : new Color(0.3f, 0.9f, 1f, 0.9f);
                flashObj.transform.localScale = Vector3.one * 0.8f;
                Destroy(flashObj, 0.15f);
            }
        }
    }
}
