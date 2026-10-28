using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(EnemyMovement))]
    [RequireComponent(typeof(EnemyHealth))]
    public class BomberEnemy : MonoBehaviour
    {
        [Header("Detonation Settings")]
        public float detonationDistanceToBase = 3.5f;
        public float defenseLineXThreshold = -3.5f;
        public float blastRadius = 2.8f;
        public int explosionDamage = 180;
        public float chargeTime = 0.85f;

        private EnemyMovement movement;
        private EnemyHealth health;
        private SpriteRenderer spriteRenderer;
        private bool isPrimed = false;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            health = GetComponent<EnemyHealth>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isPrimed) return;

            // Check if reached defense perimeter or close to base
            bool reachedDefenseLine = transform.position.x <= defenseLineXThreshold;
            bool nearAltar = DragonBallAltar.Instance != null && 
                             Vector2.Distance(transform.position, DragonBallAltar.Instance.transform.position) <= detonationDistanceToBase;

            if (reachedDefenseLine || nearAltar)
            {
                StartCoroutine(DetonationSequence());
            }
        }

        private IEnumerator DetonationSequence()
        {
            isPrimed = true;
            movement.SetMoving(false);

            SoundManager.Instance?.PlayAlarmSound();

            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            // Beep and swell
            while (elapsed < chargeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / chargeTime;

                // Scale pulse
                float pulse = 1f + 0.35f * Mathf.Sin(t * 30f);
                transform.localScale = originalScale * pulse;

                // Rapid flash between red and yellow
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = (Mathf.Sin(t * 35f) > 0) ? Color.red : Color.yellow;
                }

                yield return null;
            }

            Explode();
        }

        private void Explode()
        {
            ScreenShake.Instance?.Shake(0.35f, 0.35f);
            SoundManager.Instance?.PlayExplosionSound(0.9f);

            // Damage Base
            if (DragonBallAltar.Instance != null)
            {
                float distToBase = Vector2.Distance(transform.position, DragonBallAltar.Instance.transform.position);
                if (distToBase <= blastRadius + 1.5f)
                {
                    DragonBallAltar.Instance.TakeDamage(explosionDamage);
                }
            }

            // Damage nearby allies
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Defender"))
                {
                    TurretAlly ally = col.GetComponent<TurretAlly>();
                    if (ally != null) ally.TakeDamage(explosionDamage / 2);
                }
            }

            health.DespawnWithoutDrops();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blastRadius);
        }
    }
}
