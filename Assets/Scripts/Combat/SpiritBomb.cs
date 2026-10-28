using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    public class SpiritBomb : MonoBehaviour
    {
        public int explosionDamage = 450;
        public float blastRadius = 3.5f;
        public float travelSpeed = 8f;

        private Vector3 targetPosition;
        private bool hasDetonated = false;

        public void LaunchTo(Vector3 target)
        {
            targetPosition = target;
            StartCoroutine(TravelToTarget());
        }

        private IEnumerator TravelToTarget()
        {
            Vector3 startPos = transform.position;
            float totalDistance = Vector3.Distance(startPos, targetPosition);
            float duration = totalDistance / travelSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Parabolic trajectory
                Vector3 currentPos = Vector3.Lerp(startPos, targetPosition, t);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;
                transform.position = currentPos;

                // Rotate & scale pulsation
                transform.Rotate(0, 0, 360f * Time.deltaTime);

                yield return null;
            }

            Detonate();
        }

        private void Detonate()
        {
            if (hasDetonated) return;
            hasDetonated = true;

            ScreenShake.Instance?.Shake(0.5f, 0.45f);
            SoundManager.Instance?.PlayExplosionSound(1f);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    EnemyHealth health = col.GetComponent<EnemyHealth>();
                    if (health != null)
                    {
                        health.TakeDamage(explosionDamage, true);
                    }
                }
            }

            // Visual explosion ring
            GameObject ring = new GameObject("SpiritBombShockwave");
            ring.transform.position = transform.position;
            SpriteRenderer sr = ring.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            ring.transform.localScale = Vector3.one * 0.5f;

            // Animate expand and fade
            SpiritBombShockwave shock = ring.AddComponent<SpiritBombShockwave>();
            shock.maxScale = blastRadius * 2f;

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, blastRadius);
        }
    }

    public class SpiritBombShockwave : MonoBehaviour
    {
        public float maxScale = 7f;
        private float duration = 0.5f;
        private float timer = 0f;
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.one * Mathf.Lerp(0.5f, maxScale, t);

            if (sr != null)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f - t);
            }

            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
