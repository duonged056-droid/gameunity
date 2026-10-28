using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragonBallDefenders
{
    public enum AllyType
    {
        BulmaTurret,
        Vegeta,
        Piccolo
    }

    public class TurretAlly : MonoBehaviour
    {
        [Header("Ally Info")]
        public AllyType allyType;
        public string allyName = "Bulma Turret";
        public int maxHealth = 300;
        public int currentHealth;

        [Header("Combat Stats")]
        public float attackRange = 7.5f;
        public float attackRate = 1.8f; // Attacks per sec
        public int attackDamage = 30;
        public GameObject projectilePrefab;
        public Transform firePoint;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float nextAttackTime = 0f;
        private Transform currentTarget;

        private void Awake()
        {
            gameObject.tag = "Defender";
            currentHealth = maxHealth;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            FindTarget();

            if (currentTarget != null && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + (1f / attackRate);
                AttackTarget();
            }
        }

        private void FindTarget()
        {
            if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
            {
                float dist = Vector2.Distance(transform.position, currentTarget.position);
                if (dist <= attackRange) return;
            }

            currentTarget = null;
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            float closestDist = float.MaxValue;

            foreach (var col in enemies)
            {
                if (col.CompareTag("Enemy"))
                {
                    float d = Vector2.Distance(transform.position, col.transform.position);
                    if (d < closestDist)
                    {
                        closestDist = d;
                        currentTarget = col.transform;
                    }
                }
            }
        }

        private void AttackTarget()
        {
            if (currentTarget == null) return;

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.right * 0.5f;
            Vector2 dir = (currentTarget.position - spawnPos).normalized;

            if (projectilePrefab != null)
            {
                GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                KiBlast blast = proj.GetComponent<KiBlast>();
                if (blast != null)
                {
                    blast.Initialize(dir, attackDamage);
                }
            }
            else
            {
                // Procedural projectile
                GameObject proj = new GameObject("AllyKiBlast");
                proj.transform.position = spawnPos;
                proj.transform.localScale = Vector3.one * 0.7f;

                SpriteRenderer sr = proj.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 10;
                sr.color = allyType == AllyType.Vegeta ? new Color(0.2f, 0.4f, 1f, 1f) : 
                           (allyType == AllyType.Piccolo ? new Color(1f, 0.9f, 0.1f, 1f) : new Color(0.3f, 1f, 0.5f, 1f));

                CircleCollider2D col = proj.AddComponent<CircleCollider2D>();
                col.isTrigger = true;

                KiBlast blast = proj.AddComponent<KiBlast>();
                blast.speed = 13f;
                blast.Initialize(dir, attackDamage);
            }

            SoundManager.Instance?.PlayKiBlastSound(0.15f);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            DamagePopup.Create(transform.position, damage);

            if (currentHealth <= 0)
            {
                SoundManager.Instance?.PlayExplosionSound(0.5f);
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
