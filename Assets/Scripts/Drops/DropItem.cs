using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    public enum DropType
    {
        Zeni,
        SenzuBean,
        KiOrb
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class DropItem : MonoBehaviour
    {
        public DropType dropType = DropType.Zeni;
        public int amount = 25;
        public float lifetime = 15f;
        public float attractSpeed = 10f;

        private bool isAttracted = false;
        private Vector3 targetPos;
        private Vector3 startPos;
        private float bobOffset;

        private void Awake()
        {
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            gameObject.tag = "DropItem";
            bobOffset = Random.Range(0f, 6.28f);
        }

        private void Start()
        {
            startPos = transform.position;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (isAttracted)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, attractSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPos) < 0.4f)
                {
                    Collect();
                }
            }
            else
            {
                // Gentle floating bob
                float bob = Mathf.Sin(Time.time * 4f + bobOffset) * 0.1f;
                transform.position = startPos + new Vector3(0, bob, 0);
            }
        }

        public void AttractTowards(Vector3 destination)
        {
            isAttracted = true;
            targetPos = destination;
        }

        public void Collect()
        {
            if (GameManager.Instance == null) return;

            switch (dropType)
            {
                case DropType.Zeni:
                    GameManager.Instance.AddZeni(amount);
                    break;

                case DropType.SenzuBean:
                    if (DragonBallAltar.Instance != null)
                    {
                        DragonBallAltar.Instance.Heal(amount);
                    }
                    break;

                case DropType.KiOrb:
                    GameManager.Instance.AddKi(amount);
                    SoundManager.Instance?.PlaySuperSaiyanSound();
                    break;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") || collision.name.Contains("Goku"))
            {
                Collect();
            }
        }

        public static DropItem CreateProceduralDrop(Vector3 pos, DropType type, int value)
        {
            GameObject dropObj = new GameObject("Drop_" + type);
            dropObj.transform.position = pos;
            dropObj.transform.localScale = Vector3.one * 0.7f;

            SpriteRenderer sr = dropObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 8;

            switch (type)
            {
                case DropType.Zeni:
                    sr.color = new Color(1f, 0.85f, 0.1f, 1f); // Bright Gold
                    break;
                case DropType.SenzuBean:
                    sr.color = new Color(0.2f, 0.85f, 0.3f, 1f); // Emerald Green
                    break;
                case DropType.KiOrb:
                    sr.color = new Color(0.2f, 0.7f, 1f, 1f); // Radiant Cyan
                    break;
            }

            CircleCollider2D col = dropObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;

            DropItem item = dropObj.AddComponent<DropItem>();
            item.dropType = type;
            item.amount = value;
            return item;
        }
    }
}
