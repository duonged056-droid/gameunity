using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class KamehamehaBeam : MonoBehaviour
    {
        public int tickDamage = 60;
        public float duration = 1.2f;
        public float damageInterval = 0.12f;

        private float timer = 0f;
        private float tickTimer = 0f;
        private readonly List<EnemyHealth> enemiesInRange = new List<EnemyHealth>();

        private void Awake()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            gameObject.tag = "Projectile";
        }

        private void Start()
        {
            ScreenShake.Instance?.Shake(duration, 0.25f);
            SoundManager.Instance?.PlayKamehamehaSound();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            tickTimer += Time.deltaTime;

            if (tickTimer >= damageInterval)
            {
                tickTimer = 0f;
                DamageAllEnemies();
            }

            // Pulse beam scale
            float scaleY = 1f + 0.15f * Mathf.Sin(Time.time * 40f);
            transform.localScale = new Vector3(transform.localScale.x, scaleY, 1f);

            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }

        private void DamageAllEnemies()
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                if (enemiesInRange[i] != null && enemiesInRange[i].gameObject.activeInHierarchy)
                {
                    enemiesInRange[i].TakeDamage(tickDamage, true);
                }
                else
                {
                    enemiesInRange.RemoveAt(i);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
                if (enemy != null && !enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                    enemy.TakeDamage(tickDamage, true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemiesInRange.Remove(enemy);
                }
            }
        }
    }
}
