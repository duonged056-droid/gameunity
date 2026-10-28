using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab Quái vật")]
    public GameObject normalEnemyPrefab;
    public GameObject bomberEnemyPrefab;

    [Header("Danh sách Điểm đầu & Điểm cuối")]
    public Transform[] spawnPoints;
    public Transform[] endPoints;

    [Header("Cấu hình Đợt tấn công (Waves)")]
    public int currentWave = 1;
    public int baseEnemiesPerWave = 5;
    public float timeBetweenSpawns = 1.2f;
    public float timeBetweenWaves = 4f;

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateWaveUI(currentWave);
            }

            // Tăng dần số lượng, máu và tốc độ quái sau mỗi đợt
            int enemyCount = baseEnemiesPerWave + (currentWave - 1) * 3;
            float hpMultiplier = 1f + (currentWave - 1) * 0.35f;
            float speedBonus = (currentWave - 1) * 0.2f;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnSingleEnemy(hpMultiplier, speedBonus);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Đợi người chơi tiêu diệt hết quái trong đợt hiện tại
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            // Nghỉ giữa hiệp để nâng cấp vũ khí trước khi sang đợt tiếp theo
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
        }
    }

    void SpawnSingleEnemy(float hpMultiplier, float speedBonus)
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || endPoints == null || endPoints.Length == 0) return;

        int laneIndex = Random.Range(0, spawnPoints.Length);
        laneIndex = Mathf.Clamp(laneIndex, 0, endPoints.Length - 1);
        GameObject prefabToSpawn = (Random.value < 0.35f) ? bomberEnemyPrefab : normalEnemyPrefab;

        if (prefabToSpawn != null)
        {
            GameObject enemyObj = Instantiate(prefabToSpawn);
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.SetupEnemy(spawnPoints[laneIndex], endPoints[laneIndex], hpMultiplier, speedBonus);
            }
        }
    }
}
