using System;
using UnityEngine;
using UnityEngine.UI;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DragonBallAltar : MonoBehaviour
    {
        public static DragonBallAltar Instance { get; private set; }

        [Header("Health Settings")]
        public int maxHealth = 1000;
        public int currentHealth;

        [Header("Visual & Effects")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private SpriteRenderer altarRenderer;
        [SerializeField] private GameObject dragonBallsGlowEffect;

        public event Action<int, int> OnHealthChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            gameObject.tag = "Base";

            currentHealth = maxHealth;
        }

        private void Start()
        {
            UpdateUI();
        }

        public void TakeDamage(int damage)
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.GameOver) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            ScreenShake.Instance?.Shake(0.25f, 0.25f);
            SoundManager.Instance?.PlayExplosionSound(0.7f);

            UpdateUI();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            UpdateUI();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            SoundManager.Instance?.PlaySenzuHealSound();
        }

        private void UpdateUI()
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
                int damage = 50;
                if (enemy != null)
                {
                    damage = enemy.baseDamage;
                    enemy.DespawnWithoutDrops();
                }
                else
                {
                    Destroy(collision.gameObject);
                }

                TakeDamage(damage);
            }
        }

        private void Die()
        {
            SoundManager.Instance?.PlayExplosionSound(1f);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.GameOver);
            }
        }
    }
}
