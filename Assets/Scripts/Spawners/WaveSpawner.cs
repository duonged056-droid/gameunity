using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragonBallDefenders
{
    public class WaveSpawner : MonoBehaviour
    {
        public static WaveSpawner Instance { get; private set; }

        [Header("Wave Configuration")]
        public int currentWaveIndex = 0;
        public int totalWaves = 10;
        public float timeBetweenWaves = 5.0f;
        public float spawnInterval = 0.85f;

        [Header("Multipliers per Wave")]
        public float healthMultiplierPerWave = 1.22f;
        public float speedMultiplierPerWave = 1.04f;
        public float countMultiplierPerWave = 1.25f;

        [Header("Enemy Prefabs")]
        public GameObject saibamenPrefab; // Bomber
        public GameObject rrRobotPrefab;  // Grunt
        public GameObject rrTankPrefab;   // Tank

        [Header("Boss Prefabs")]
        public GameObject bossVegetaPrefab;
        public GameObject bossFriezaPrefab;
        public GameObject bossCellPrefab;
        public GameObject bossBuuPrefab;

        [Header("Paths / Lanes")]
        public WaypointPath[] paths;

        [Header("Runtime State")]
        public float countdown = 5f;
        public bool isSpawningWave = false;
        public int remainingEnemiesToKill = 0;

        public event Action<int, int> OnWaveChanged;
        public event Action<float> OnCountdownChanged;

        private Coroutine waveCoroutine;

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
        }

        private void Start()
        {
            countdown = timeBetweenWaves;
            waveCoroutine = StartCoroutine(WaveLoop());
        }

        private void Update()
        {
            if (isSpawningWave || currentWaveIndex >= totalWaves) return;

            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.WaveBreak)
            {
                countdown -= Time.deltaTime;
                OnCountdownChanged?.Invoke(Mathf.Max(0f, countdown));
            }
        }

        private IEnumerator WaveLoop()
        {
            while (currentWaveIndex < totalWaves)
            {
                // Break period between waves
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.WaveBreak);
                }

                countdown = timeBetweenWaves;
                while (countdown > 0)
                {
                    yield return null;
                }

                // Start next wave
                currentWaveIndex++;
                OnWaveChanged?.Invoke(currentWaveIndex, totalWaves);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Playing);
                }

                yield return StartCoroutine(SpawnWave(currentWaveIndex));

                // Wait for all spawned enemies in this wave to be eliminated
                while (remainingEnemiesToKill > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                // Wave Cleared bonus
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddZeni(50 + currentWaveIndex * 25);
                    GameManager.Instance.AddScore(500 * currentWaveIndex);
                }

                SoundManager.Instance?.PlayCoinSound();
                yield return new WaitForSeconds(1.0f);
            }

            // All waves cleared -> Victory!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.Victory);
            }
        }

        // Implementation of Coroutine IEnumerator SpawnWave() as required in DOCX
        public IEnumerator SpawnWave(int waveNumber)
        {
            isSpawningWave = true;

            int baseEnemies = 6;
            int enemyCount = Mathf.RoundToInt(baseEnemies * Mathf.Pow(countMultiplierPerWave, waveNumber - 1));
            float hpMult = Mathf.Pow(healthMultiplierPerWave, waveNumber - 1);
            float spdMult = Mathf.Min(1.6f, Mathf.Pow(speedMultiplierPerWave, waveNumber - 1));

            remainingEnemiesToKill = enemyCount;

            // Check if this wave has a boss
            GameObject bossPrefab = GetBossForWave(waveNumber);
            if (bossPrefab != null)
            {
                remainingEnemiesToKill += 1;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandomMinion(waveNumber, hpMult, spdMult);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Spawn Boss at the end of the wave if applicable
            if (bossPrefab != null)
            {
                yield return new WaitForSeconds(1.5f);
                SpawnBoss(bossPrefab, hpMult * 1.5f, spdMult);
            }

            isSpawningWave = false;
        }

        private void SpawnRandomMinion(int wave, float hpMult, float spdMult)
        {
            if (paths == null || paths.Length == 0) return;

            WaypointPath selectedPath = paths[UnityEngine.Random.Range(0, paths.Length)];
            Transform[] waypoints = selectedPath.GetPoints();
            if (waypoints == null || waypoints.Length == 0) return;

            // Pick enemy type: Saibamen (bomber), RR Robot, RR Tank
            GameObject prefabToSpawn;
            float roll = UnityEngine.Random.value;

            if (wave >= 3 && roll < 0.25f && rrTankPrefab != null)
            {
                prefabToSpawn = rrTankPrefab;
            }
            else if (roll < 0.55f && saibamenPrefab != null)
            {
                prefabToSpawn = saibamenPrefab; // Saibamen bomber
            }
            else
            {
                prefabToSpawn = rrRobotPrefab != null ? rrRobotPrefab : saibamenPrefab;
            }

            GameObject enemy;
            if (prefabToSpawn != null)
            {
                enemy = Instantiate(prefabToSpawn, waypoints[0].position, Quaternion.identity);
            }
            else
            {
                enemy = CreateProceduralMinion(roll < 0.5f, waypoints[0].position);
            }

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetWaypoints(waypoints);
                movement.SetSpeedMultiplier(spdMult);
            }

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.InitializeHealth(health.maxHealth, hpMult);
            }
        }

        private GameObject CreateProceduralMinion(bool isBomber, Vector3 spawnPos)
        {
            GameObject go = new GameObject(isBomber ? "Saibamen_Bomber" : "RR_Robot");
            go.transform.position = spawnPos;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            string sName = isBomber ? "Saibamen" : "RRRobot";
            Sprite s = Resources.Load<Sprite>("Sprites/" + sName) ?? Resources.Load<Sprite>(sName);
#if UNITY_EDITOR
            if (s == null) s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{sName}.png");
#endif
            if (s != null)
            {
                sr.sprite = s;
                sr.color = Color.white;
            }
            else
            {
                sr.color = isBomber ? new Color(0.3f, 0.8f, 0.2f, 1f) : new Color(0.85f, 0.2f, 0.2f, 1f);
            }
            sr.sortingOrder = 5;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.45f;

            EnemyMovement mv = go.AddComponent<EnemyMovement>();
            mv.moveSpeed = isBomber ? 3.2f : 1.9f;

            EnemyHealth eh = go.AddComponent<EnemyHealth>();
            eh.maxHealth = isBomber ? 65 : 140;
            eh.currentHealth = eh.maxHealth;
            eh.baseDamage = isBomber ? 120 : 60;
            eh.zeniReward = isBomber ? 20 : 35;
            eh.scoreReward = isBomber ? 50 : 100;

            if (isBomber)
            {
                BomberEnemy bomber = go.AddComponent<BomberEnemy>();
                bomber.explosionDamage = 180;
                bomber.blastRadius = 3.0f;
            }

            return go;
        }

        private GameObject GetBossForWave(int wave)
        {
            switch (wave)
            {
                case 3: return bossVegetaPrefab != null ? bossVegetaPrefab : CreateProceduralBoss("Prince Vegeta", 1400);
                case 5: return bossFriezaPrefab != null ? bossFriezaPrefab : CreateProceduralBoss("Lord Frieza", 2800);
                case 7: return bossCellPrefab != null ? bossCellPrefab : CreateProceduralBoss("Perfect Cell", 4500);
                case 10: return bossBuuPrefab != null ? bossBuuPrefab : CreateProceduralBoss("Majin Buu", 7500);
                default: return null;
            }
        }

        private GameObject CreateProceduralBoss(string bName, int hp)
        {
            GameObject go = new GameObject("Boss_" + bName);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            string sName = bName.Contains("Vegeta") ? "BossVegeta" : (bName.Contains("Frieza") ? "BossFrieza" : (bName.Contains("Cell") ? "BossCell" : "BossBuu"));
            Sprite s = Resources.Load<Sprite>("Sprites/" + sName) ?? Resources.Load<Sprite>(sName);
#if UNITY_EDITOR
            if (s == null) s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{sName}.png");
#endif
            if (s != null)
            {
                sr.sprite = s;
                sr.color = Color.white;
            }
            else
            {
                sr.color = new Color(0.7f, 0.15f, 0.85f, 1f);
            }
            sr.sortingOrder = 6;
            go.transform.localScale = Vector3.one * 1.5f;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.6f;

            EnemyMovement mv = go.AddComponent<EnemyMovement>();
            mv.moveSpeed = 1.1f;

            EnemyHealth eh = go.AddComponent<EnemyHealth>();
            eh.maxHealth = hp;
            eh.currentHealth = hp;
            eh.baseDamage = 250;
            eh.zeniReward = 300;
            eh.scoreReward = 2000;

            BossEnemy boss = go.AddComponent<BossEnemy>();
            boss.bossName = bName;
            boss.attackDamage = 60;

            return go;
        }

        private void SpawnBoss(GameObject bossPrefab, float hpMult, float spdMult)
        {
            if (paths == null || paths.Length == 0) return;

            WaypointPath centerPath = paths[paths.Length / 2];
            Transform[] waypoints = centerPath.GetPoints();

            GameObject boss;
            if (bossPrefab.scene.IsValid())
            {
                boss = bossPrefab;
                boss.transform.position = waypoints[0].position;
            }
            else
            {
                boss = Instantiate(bossPrefab, waypoints[0].position, Quaternion.identity);
            }
            boss.transform.localScale = Vector3.one * 1.5f;

            EnemyMovement movement = boss.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetWaypoints(waypoints);
                movement.SetSpeedMultiplier(0.9f);
            }

            EnemyHealth health = boss.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.InitializeHealth(health.maxHealth, hpMult * 2.5f);
            }
        }

        public void OnEnemyKilled(GameObject enemy)
        {
            remainingEnemiesToKill = Mathf.Max(0, remainingEnemiesToKill - 1);
        }
    }
}
