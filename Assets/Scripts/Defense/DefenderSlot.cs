using UnityEngine;

namespace DragonBallDefenders
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DefenderSlot : MonoBehaviour
    {
        public bool isOccupied = false;
        public TurretAlly currentAlly;

        [SerializeField] private SpriteRenderer slotIndicator;

        private void Awake()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (slotIndicator == null) slotIndicator = GetComponent<SpriteRenderer>();
        }

        public bool DeployAlly(GameObject allyPrefab)
        {
            if (isOccupied || allyPrefab == null) return false;

            GameObject allyObj = Instantiate(allyPrefab, transform.position, Quaternion.identity);
            currentAlly = allyObj.GetComponent<TurretAlly>();
            isOccupied = true;

            if (slotIndicator != null)
            {
                slotIndicator.color = new Color(1f, 1f, 1f, 0.2f); // Fade out slot outline
            }

            return true;
        }

        private void Update()
        {
            if (isOccupied && currentAlly == null)
            {
                isOccupied = false;
                if (slotIndicator != null)
                {
                    slotIndicator.color = new Color(0.2f, 1f, 0.4f, 0.6f);
                }
            }
        }
    }
}
