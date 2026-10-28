using UnityEngine;

public class PlayerDefender : MonoBehaviour
{
    [Header("Di chuyển Tuyến phòng thủ")]
    public float moveSpeed = 7f;
    public float minY = -3.5f;
    public float maxY = 3.5f;

    [Header("Tấn công")]
    public GameObject kiBlastPrefab;
    public Transform firePoint;
    private float nextFireTime = 0f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        float verticalInput = 0f;
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed || UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                verticalInput += 1f;
            if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed || UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                verticalInput -= 1f;
        }
        else
#endif
        {
            try
            {
                verticalInput = Input.GetAxisRaw("Vertical");
            }
            catch (System.InvalidOperationException) { }
        }

        Vector3 newPos = transform.position + new Vector3(0, verticalInput * moveSpeed * Time.deltaTime, 0);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;
    }

    void HandleAttack()
    {
        bool fire = false;
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed)
            fire = true;
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
            fire = true;
        if (UnityEngine.InputSystem.Keyboard.current == null && UnityEngine.InputSystem.Mouse.current == null)
#endif
        {
            try
            {
                fire = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
            }
            catch (System.InvalidOperationException) { }
        }

        float effectiveFireRate = GameManager.instance != null ? GameManager.instance.fireRate : 0.4f;
        if (fire && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + effectiveFireRate;
            ShootKiBlast();
        }
    }

    void ShootKiBlast()
    {
        // Kích hoạt Animation vung tay bắn chưởng của Goku
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + new Vector3(0.6f, 0.1f, 0);
        if (kiBlastPrefab != null)
        {
            Instantiate(kiBlastPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // Procedural fallback if prefab is not assigned
            GameObject blastObj = new GameObject("KiBlast");
            blastObj.transform.position = spawnPos;
            SpriteRenderer sr = blastObj.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/KiBlast_0") ?? Resources.Load<Sprite>("Sprites/KiBlast");
            sr.sortingLayerName = "Gameplay";
            CircleCollider2D col = blastObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            blastObj.AddComponent<KiBlast>();
        }
    }
}
