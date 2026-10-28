using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHealth = 100;
        public int currentHealth;
        public int baseDamage = 50; // Damage dealt to base if reached
        public int zeniReward = 25;
        public int scoreReward = 100;

        [Header("Drop Chances")]
        [Range(0f, 1f)] public float senzuDropChance = 0.12f;
        [Range(0f, 1f)] public float kiOrbDropChance = 0.35f;

        [Header("Drop Prefabs")]
        public GameObject zeniDropPrefab;
        public GameObject senzuDropPrefab;
        public GameObject kiOrbDropPrefab;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform healthBarFill;

        private Color originalColor = Color.white;
        private bool isDead = false;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) originalColor = spriteRenderer.color;
            currentHealth = maxHealth;
        }

        public void InitializeHealth(int baseHp, float multiplier)
        {
            maxHealth = Mathf.RoundToInt(baseHp * multiplier);
            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        public void TakeDamage(int damage, bool isCritical = false)
        {
            if (isDead) return;

            currentHealth -= damage;
            DamagePopup.Create(transform.position, damage, isCritical);
            StartCoroutine(FlashEffect());
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private IEnumerator FlashEffect()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.08f);
                spriteRenderer.color = originalColor;
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBarFill != null)
            {
                float ratio = Mathf.Clamp01((float)currentHealth / maxHealth);
                healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            SoundManager.Instance?.PlayExplosionSound(0.4f);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreReward);
                GameManager.Instance.AddZeni(zeniReward);
            }

            SpawnDrops();

            if (WaveSpawner.Instance != null)
            {
                WaveSpawner.Instance.OnEnemyKilled(gameObject);
            }

            Destroy(gameObject);
        }

        public void DespawnWithoutDrops()
        {
            if (isDead) return;
            isDead = true;

            if (WaveSpawner.Instance != null)
            {
                WaveSpawner.Instance.OnEnemyKilled(gameObject);
            }

            Destroy(gameObject);
        }

        private void SpawnDrops()
        {
            Vector3 pos = transform.position;

            // Spawn Zeni coin
            if (zeniDropPrefab != null)
            {
                Instantiate(zeniDropPrefab, pos + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0), Quaternion.identity);
            }
            else
            {
                DropItem.CreateProceduralDrop(pos, DropType.Zeni, zeniReward);
            }

            // Chance for Senzu Bean
            if (Random.value < senzuDropChance)
            {
                if (senzuDropPrefab != null)
                {
                    Instantiate(senzuDropPrefab, pos + new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.2f, 0.2f), 0), Quaternion.identity);
                }
                else
                {
                    DropItem.CreateProceduralDrop(pos + Vector3.up * 0.4f, DropType.SenzuBean, 150);
                }
            }

            // Chance for Ki Orb
            if (Random.value < kiOrbDropChance)
            {
                if (kiOrbDropPrefab != null)
                {
                    Instantiate(kiOrbDropPrefab, pos + new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.2f, 0.2f), 0), Quaternion.identity);
                }
                else
                {
                    DropItem.CreateProceduralDrop(pos - Vector3.up * 0.4f, DropType.KiOrb, 30);
                }
            }
        }
    }
}
