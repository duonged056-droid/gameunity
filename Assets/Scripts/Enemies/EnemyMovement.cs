using System.Collections.Generic;
using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Movement Stats")]
        public float moveSpeed = 2.0f;
        public float speedMultiplier = 1.0f;

        [Header("Waypoints")]
        public Transform[] waypoints;
        private int currentWaypointIndex = 0;

        private Rigidbody2D rb;
        private bool isMoving = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            gameObject.tag = "Enemy";
        }

        public void SetWaypoints(Transform[] points)
        {
            waypoints = points;
            currentWaypointIndex = 0;
            if (waypoints != null && waypoints.Length > 0)
            {
                transform.position = waypoints[0].position;
            }
        }

        public void SetSpeedMultiplier(float mult)
        {
            speedMultiplier = mult;
        }

        public void SetMoving(bool moving)
        {
            isMoving = moving;
        }

        private void Update()
        {
            if (!isMoving || waypoints == null || waypoints.Length == 0) return;

            Transform targetPoint = waypoints[currentWaypointIndex];
            float effectiveSpeed = moveSpeed * speedMultiplier;

            // Move towards waypoint using Vector2.MoveTowards as specified in DOCX
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, effectiveSpeed * Time.deltaTime);

            // Reached waypoint?
            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    OnReachEndPoint();
                }
            }
        }

        private void OnReachEndPoint()
        {
            // Reached Dragon Ball Altar
            DragonBallAltar altar = DragonBallAltar.Instance;
            if (altar != null)
            {
                EnemyHealth health = GetComponent<EnemyHealth>();
                int dmg = health != null ? health.baseDamage : 50;
                altar.TakeDamage(dmg);
            }

            EnemyHealth eh = GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.DespawnWithoutDrops();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
