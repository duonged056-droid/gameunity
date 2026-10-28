using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    public class DefenderAttack : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public GameObject kiBlastPrefab;
        public Transform firePoint;

        [Header("Combat Stats")]
        public float fireRate = 3.5f; // Blasts per second
        public int baseDamage = 35;
        public float criticalChance = 0.15f;

        [Header("Super Saiyan Buff")]
        public bool isSuperSaiyan = false;
        public float ssjDamageMultiplier = 2.0f;
        public float ssjFireRateMultiplier = 1.8f;

        [Header("Special Skills")]
        public GameObject kamehamehaPrefab;
        public GameObject spiritBombPrefab;

        public float kamehamehaCost = 40f;
        public float spiritBombCost = 60f;
        public float superSaiyanCost = 50f;

        public float kamehamehaCooldown = 8f;
        public float spiritBombCooldown = 15f;
        public float superSaiyanDuration = 12f;

        private float nextFireTime = 0f;
        private float kamehamehaTimer = 0f;
        private float spiritBombTimer = 0f;
        private Coroutine ssjCoroutine;

        public float KamehamehaCooldownRatio => Mathf.Clamp01(kamehamehaTimer / kamehamehaCooldown);
        public float SpiritBombCooldownRatio => Mathf.Clamp01(spiritBombTimer / spiritBombCooldown);

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing && GameManager.Instance.currentState != GameState.WaveBreak)
                return;

            // Cooldown timers
            if (kamehamehaTimer > 0) kamehamehaTimer -= Time.deltaTime;
            if (spiritBombTimer > 0) spiritBombTimer -= Time.deltaTime;

            HandleInput();
        }

        private void HandleInput()
        {
            // Auto or manual fire Ki Blast
            if (CheckFireInput())
            {
                TryFireKiBlast();
            }

            // Kamehameha (Q key)
            if (CheckKeyInput(KeyCode.Q))
            {
                TryCastKamehameha();
            }

            // Spirit Bomb (E key)
            if (CheckKeyInput(KeyCode.E))
            {
                TryCastSpiritBomb();
            }

            // Super Saiyan (R key)
            if (CheckKeyInput(KeyCode.R))
            {
                TryActivateSuperSaiyan();
            }
        }

        public void TryFireKiBlast()
        {
            float effectiveFireRate = isSuperSaiyan ? fireRate * ssjFireRateMultiplier : fireRate;

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + (1f / effectiveFireRate);
                FireKiBlast();
            }
        }

        private void FireKiBlast()
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.right * 0.8f;
            Vector3 mouseWorld = Camera.main != null ? Camera.main.ScreenToWorldPoint(GetMousePosition()) : spawnPos + Vector3.right;
            mouseWorld.z = 0;

            Vector2 direction = (mouseWorld - spawnPos).normalized;
            if (direction.x < 0.1f) direction = Vector2.right; // Keep firing mostly rightwards

            int effectiveDamage = Mathf.RoundToInt(isSuperSaiyan ? baseDamage * ssjDamageMultiplier : baseDamage);
            bool isCrit = Random.value < (isSuperSaiyan ? criticalChance * 2f : criticalChance);
            if (isCrit) effectiveDamage = Mathf.RoundToInt(effectiveDamage * 1.8f);

            GameObject blastObj = CreateBlastInstance(spawnPos);
            KiBlast blast = blastObj.GetComponent<KiBlast>();
            if (blast != null)
            {
                blast.Initialize(direction, effectiveDamage, isCrit);
            }

            // In Super Saiyan, fire twin blasts!
            if (isSuperSaiyan)
            {
                GameObject blastObj2 = CreateBlastInstance(spawnPos + new Vector3(0, 0.25f, 0));
                KiBlast blast2 = blastObj2.GetComponent<KiBlast>();
                if (blast2 != null)
                {
                    blast2.Initialize(direction, effectiveDamage, isCrit);
                }
            }

            SoundManager.Instance?.PlayKiBlastSound();
        }

        private GameObject CreateBlastInstance(Vector3 pos)
        {
            if (kiBlastPrefab != null)
            {
                return Instantiate(kiBlastPrefab, pos, Quaternion.identity);
            }

            GameObject blastObj = new GameObject("KiBlast");
            blastObj.transform.position = pos;
            blastObj.transform.localScale = Vector3.one * 0.8f;

            SpriteRenderer sr = blastObj.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/KiBlast") ?? Resources.Load<Sprite>("KiBlast");
#if UNITY_EDITOR
            if (sr.sprite == null) sr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KiBlast.png");
#endif
            sr.color = new Color(1f, 0.9f, 0.2f, 1f);
            sr.sortingOrder = 10;

            CircleCollider2D col = blastObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;

            blastObj.AddComponent<KiBlast>();
            return blastObj;
        }

        public bool TryCastKamehameha()
        {
            if (kamehamehaTimer > 0) return false;
            if (GameManager.Instance != null && !GameManager.Instance.ConsumeKi(kamehamehaCost)) return false;

            kamehamehaTimer = kamehamehaCooldown;
            CastKamehameha();
            return true;
        }

        private void CastKamehameha()
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.right * 1f;

            if (kamehamehaPrefab != null)
            {
                GameObject beam = Instantiate(kamehamehaPrefab, spawnPos, Quaternion.identity);
                beam.transform.SetParent(transform); // Moves with Goku
            }
            else
            {
                // Procedural beam
                GameObject beam = new GameObject("KamehamehaBeam");
                beam.transform.position = spawnPos + Vector3.right * 7f;
                beam.transform.localScale = new Vector3(14f, 1.4f, 1f);
                beam.AddComponent<KamehamehaBeam>();
                SpriteRenderer sr = beam.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Sprites/KamehamehaBeam") ?? Resources.Load<Sprite>("KamehamehaBeam");
#if UNITY_EDITOR
                if (sr.sprite == null) sr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KamehamehaBeam.png");
#endif
                sr.color = new Color(0.1f, 0.7f, 1f, 0.85f);
                sr.sortingOrder = 15;
            }
        }

        public bool TryCastSpiritBomb()
        {
            if (spiritBombTimer > 0) return false;
            if (GameManager.Instance != null && !GameManager.Instance.ConsumeKi(spiritBombCost)) return false;

            spiritBombTimer = spiritBombCooldown;
            CastSpiritBomb();
            return true;
        }

        private void CastSpiritBomb()
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 4f, 0);
            Vector3 targetPos = Camera.main != null ? Camera.main.ScreenToWorldPoint(GetMousePosition()) : transform.position + Vector3.right * 5f;
            targetPos.z = 0;

            if (spiritBombPrefab != null)
            {
                GameObject bomb = Instantiate(spiritBombPrefab, spawnPos, Quaternion.identity);
                SpiritBomb sb = bomb.GetComponent<SpiritBomb>();
                if (sb != null) sb.LaunchTo(targetPos);
            }
            else
            {
                GameObject bomb = new GameObject("SpiritBomb");
                bomb.transform.position = spawnPos;
                bomb.transform.localScale = Vector3.one * 1.8f;
                SpriteRenderer sr = bomb.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Sprites/SpiritBomb") ?? Resources.Load<Sprite>("SpiritBomb");
#if UNITY_EDITOR
                if (sr.sprite == null) sr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/SpiritBomb.png");
#endif
                sr.color = new Color(0.2f, 0.9f, 1f, 0.95f);
                sr.sortingOrder = 20;
                SpiritBomb sb = bomb.AddComponent<SpiritBomb>();
                sb.LaunchTo(targetPos);
            }
        }

        public bool TryActivateSuperSaiyan()
        {
            if (isSuperSaiyan) return false;
            if (GameManager.Instance != null && !GameManager.Instance.ConsumeKi(superSaiyanCost)) return false;

            if (ssjCoroutine != null) StopCoroutine(ssjCoroutine);
            ssjCoroutine = StartCoroutine(SuperSaiyanRoutine());
            return true;
        }

        private IEnumerator SuperSaiyanRoutine()
        {
            isSuperSaiyan = true;
            SoundManager.Instance?.PlaySuperSaiyanSound();
            ScreenShake.Instance?.Shake(0.4f, 0.25f);

            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null) pc.SetSuperSaiyanState(true);

            float timer = superSaiyanDuration;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            isSuperSaiyan = false;
            if (pc != null) pc.SetSuperSaiyanState(false);
            ssjCoroutine = null;
        }

        private bool CheckFireInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.isPressed) return true;
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed) return true;
            if (UnityEngine.InputSystem.Mouse.current != null || UnityEngine.InputSystem.Keyboard.current != null)
            {
                return false;
            }
#endif
            try
            {
                return Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private bool CheckKeyInput(KeyCode code)
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (code == KeyCode.Q && UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame) return true;
                if (code == KeyCode.E && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame) return true;
                if (code == KeyCode.R && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) return true;
                return false;
            }
#endif
            try
            {
                return Input.GetKeyDown(code);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private Vector3 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
#endif
            try
            {
                return Input.mousePosition;
            }
            catch (System.InvalidOperationException)
            {
                return Vector3.zero;
            }
        }
    }
}
