using UnityEngine;

namespace DragonBallDefenders
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 6.5f;
        public float minY = -3.8f;
        public float maxY = 3.8f;

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject ssjAuraObject;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite ssjSprite;

        [Header("Collection Settings")]
        public float itemAttractRadius = 3.0f;

        private void Start()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            SetSuperSaiyanState(false);
        }

        private void Update()
        {
            HandleMovement();
            AttractNearbyItems();
        }

        private void HandleMovement()
        {
            float vertical = GetVerticalInput();
            Vector3 pos = transform.position;
            pos.y += vertical * moveSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        private float GetVerticalInput()
        {
            float v = 0f;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed || UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                    v += 1f;
                if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed || UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                    v -= 1f;
                return v;
            }
#endif
            return Input.GetAxisRaw("Vertical");
        }

        public void SetSuperSaiyanState(bool isSSJ)
        {
            if (ssjAuraObject != null)
            {
                ssjAuraObject.SetActive(isSSJ);
            }

            if (spriteRenderer != null)
            {
                if (isSSJ)
                {
                    if (ssjSprite != null) spriteRenderer.sprite = ssjSprite;
                    spriteRenderer.color = new Color(1f, 1f, 0.7f, 1f); // Golden tint
                }
                else
                {
                    if (normalSprite != null) spriteRenderer.sprite = normalSprite;
                    spriteRenderer.color = Color.white;
                }
            }
        }

        private void AttractNearbyItems()
        {
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, itemAttractRadius);
            foreach (var col in items)
            {
                if (col.CompareTag("DropItem"))
                {
                    DropItem drop = col.GetComponent<DropItem>();
                    if (drop != null)
                    {
                        drop.AttractTowards(transform.position);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, itemAttractRadius);
        }
    }
}
