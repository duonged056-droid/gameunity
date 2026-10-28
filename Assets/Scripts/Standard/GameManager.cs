using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Chỉ số Căn cứ & Kho Tài nguyên")]
    public int baseHP = 100;
    public int zeniGold = 0;

    [Header("Trang bị & Sức mạnh Chiến binh")]
    public int attackDamage = 25;
    public float fireRate = 0.4f;
    public int upgradeDamageCost = 50;
    public int upgradeSpeedCost = 50;

    [Header("Liên kết Giao diện (UI)")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI upgradeDamageBtnText;
    public TextMeshProUGUI upgradeSpeedBtnText;
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    public void AddResource(int amount)
    {
        zeniGold += amount;
        UpdateUI();
    }

    public void TakeBaseDamage(int dmg)
    {
        baseHP -= dmg;
        if (baseHP <= 0)
        {
            baseHP = 0;
            GameOver();
        }
        UpdateUI();
    }

    public void UpgradeWeaponDamage()
    {
        if (zeniGold >= upgradeDamageCost)
        {
            zeniGold -= upgradeDamageCost;
            attackDamage += 15;
            upgradeDamageCost += 40;
            UpdateUI();
        }
    }

    public void UpgradeWeaponSpeed()
    {
        if (zeniGold >= upgradeSpeedCost && fireRate > 0.1f)
        {
            zeniGold -= upgradeSpeedCost;
            fireRate -= 0.05f;
            upgradeSpeedCost += 50;
            UpdateUI();
        }
    }

    public void UpdateWaveUI(int currentWave)
    {
        if (waveText != null) waveText.text = "Đợt quái: " + currentWave;
    }

    public void UpdateUI()
    {
        if (hpText != null) hpText.text = "Máu Ngọc Rồng: " + baseHP;
        if (goldText != null) goldText.text = "Kho Zeni: " + zeniGold;
        if (upgradeDamageBtnText != null)
            upgradeDamageBtnText.text = $"Nâng Cấp Chưởng (+15 Dame)\nGiá: {upgradeDamageCost} Zeni";
        if (upgradeSpeedBtnText != null)
            upgradeSpeedBtnText.text = $"Nâng Tốc Bắn\nGiá: {upgradeSpeedCost} Zeni";
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
