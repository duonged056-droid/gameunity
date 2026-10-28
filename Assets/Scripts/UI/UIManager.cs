using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DragonBallDefenders
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        public Slider baseHealthSlider;
        public TextMeshProUGUI baseHealthText;
        public Slider kiSlider;
        public TextMeshProUGUI kiText;
        public TextMeshProUGUI zeniText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI waveText;
        public TextMeshProUGUI countdownText;

        [Header("Skill Icons & Cooldowns")]
        public Image kamehamehaCooldownFill;
        public Image spiritBombCooldownFill;
        public Button kamehamehaButton;
        public Button spiritBombButton;
        public Button superSaiyanButton;

        [Header("Shop & Upgrade Panel")]
        public GameObject shopPanel;
        public Button shopToggleButton;
        public Button upgradeDamageButton;
        public Button upgradeFireRateButton;
        public Button upgradeSsjButton;
        public Button buySenzuHealButton;
        public Button deployBulmaTurretButton;
        public Button deployVegetaButton;
        public Button deployPiccoloButton;

        public TextMeshProUGUI damageUpgradeCostText;
        public TextMeshProUGUI fireRateUpgradeCostText;
        public TextMeshProUGUI ssjUpgradeCostText;

        [Header("Warning & Announcements")]
        public GameObject bossWarningBanner;
        public TextMeshProUGUI bossWarningText;

        [Header("Game End Panels")]
        public GameObject gameOverPanel;
        public TextMeshProUGUI gameOverScoreText;
        public Button gameOverRestartButton;

        public GameObject victoryPanel;
        public TextMeshProUGUI victoryScoreText;
        public Button victoryRestartButton;

        [Header("References")]
        public DefenderAttack playerAttack;

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
            SetupEventListeners();
            SetupButtonCallbacks();

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (bossWarningBanner != null) bossWarningBanner.SetActive(false);
            if (shopPanel != null) shopPanel.SetActive(false);

            UpdateShopUI();
        }

        private void SetupEventListeners()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnZeniChanged += UpdateZeniDisplay;
                GameManager.Instance.OnKiChanged += UpdateKiDisplay;
                GameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            }

            if (DragonBallAltar.Instance != null)
            {
                DragonBallAltar.Instance.OnHealthChanged += UpdateBaseHealthDisplay;
                UpdateBaseHealthDisplay(DragonBallAltar.Instance.currentHealth, DragonBallAltar.Instance.maxHealth);
            }

            if (WaveSpawner.Instance != null)
            {
                WaveSpawner.Instance.OnWaveChanged += UpdateWaveDisplay;
                WaveSpawner.Instance.OnCountdownChanged += UpdateCountdownDisplay;
            }

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnUpgradePurchased += UpdateShopUI;
            }
        }

        private void SetupButtonCallbacks()
        {
            if (kamehamehaButton != null)
                kamehamehaButton.onClick.AddListener(() => playerAttack?.TryCastKamehameha());

            if (spiritBombButton != null)
                spiritBombButton.onClick.AddListener(() => playerAttack?.TryCastSpiritBomb());

            if (superSaiyanButton != null)
                superSaiyanButton.onClick.AddListener(() => playerAttack?.TryActivateSuperSaiyan());

            if (shopToggleButton != null)
                shopToggleButton.onClick.AddListener(ToggleShopPanel);

            if (upgradeDamageButton != null)
                upgradeDamageButton.onClick.AddListener(() => { UpgradeManager.Instance?.UpgradeDamage(); UpdateShopUI(); });

            if (upgradeFireRateButton != null)
                upgradeFireRateButton.onClick.AddListener(() => { UpgradeManager.Instance?.UpgradeFireRate(); UpdateShopUI(); });

            if (upgradeSsjButton != null)
                upgradeSsjButton.onClick.AddListener(() => { UpgradeManager.Instance?.UpgradeSuperSaiyan(); UpdateShopUI(); });

            if (buySenzuHealButton != null)
                buySenzuHealButton.onClick.AddListener(() => { UpgradeManager.Instance?.BuySenzuBaseHeal(); UpdateShopUI(); });

            if (deployBulmaTurretButton != null)
                deployBulmaTurretButton.onClick.AddListener(() => { UpgradeManager.Instance?.DeployAllyToEmptySlot(AllyType.BulmaTurret); UpdateShopUI(); });

            if (deployVegetaButton != null)
                deployVegetaButton.onClick.AddListener(() => { UpgradeManager.Instance?.DeployAllyToEmptySlot(AllyType.Vegeta); UpdateShopUI(); });

            if (deployPiccoloButton != null)
                deployPiccoloButton.onClick.AddListener(() => { UpgradeManager.Instance?.DeployAllyToEmptySlot(AllyType.Piccolo); UpdateShopUI(); });

            if (gameOverRestartButton != null)
                gameOverRestartButton.onClick.AddListener(() => GameManager.Instance?.RestartGame());

            if (victoryRestartButton != null)
                victoryRestartButton.onClick.AddListener(() => GameManager.Instance?.RestartGame());
        }

        private void Update()
        {
            // Update skill cooldown fills
            if (playerAttack != null)
            {
                if (kamehamehaCooldownFill != null)
                    kamehamehaCooldownFill.fillAmount = playerAttack.KamehamehaCooldownRatio;

                if (spiritBombCooldownFill != null)
                    spiritBombCooldownFill.fillAmount = playerAttack.SpiritBombCooldownRatio;
            }

            // Keyboard shortcut for Shop [B]
            if (CheckKeyInput(KeyCode.B))
            {
                ToggleShopPanel();
            }
        }

        private bool CheckKeyInput(KeyCode code)
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (code == KeyCode.B && UnityEngine.InputSystem.Keyboard.current.bKey.wasPressedThisFrame) return true;
            }
#endif
            return Input.GetKeyDown(code);
        }

        public void ToggleShopPanel()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(!shopPanel.activeSelf);
                UpdateShopUI();
            }
        }

        private void UpdateBaseHealthDisplay(int current, int max)
        {
            if (baseHealthSlider != null)
            {
                baseHealthSlider.maxValue = max;
                baseHealthSlider.value = current;
            }
            if (baseHealthText != null)
            {
                baseHealthText.text = $"Ngọc Rồng: {current} / {max}";
            }
        }

        private void UpdateKiDisplay(float current, float max)
        {
            if (kiSlider != null)
            {
                kiSlider.maxValue = max;
                kiSlider.value = current;
            }
            if (kiText != null)
            {
                kiText.text = $"Ki: {Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
            }
        }

        private void UpdateZeniDisplay(int zeni)
        {
            if (zeniText != null)
            {
                zeniText.text = $"Zeni: {zeni} G";
            }
            UpdateShopUI();
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        private void UpdateWaveDisplay(int current, int total)
        {
            if (waveText != null)
            {
                waveText.text = $"ĐỢT: {current} / {total}";
            }
            if (countdownText != null)
            {
                countdownText.text = "";
            }
        }

        private void UpdateCountdownDisplay(float timeRemaining)
        {
            if (countdownText != null)
            {
                countdownText.text = timeRemaining > 0.1f ? $"ĐỢT TIẾP THEO TRONG: {timeRemaining:F1}s" : "";
            }
        }

        public void UpdateShopUI()
        {
            if (UpgradeManager.Instance == null) return;

            UpgradeManager um = UpgradeManager.Instance;
            if (damageUpgradeCostText != null)
                damageUpgradeCostText.text = um.damageLevel >= um.maxDamageLevel ? "MAX" : $"{um.GetDamageUpgradeCost()} G (Lv {um.damageLevel})";

            if (fireRateUpgradeCostText != null)
                fireRateUpgradeCostText.text = um.fireRateLevel >= um.maxFireRateLevel ? "MAX" : $"{um.GetFireRateUpgradeCost()} G (Lv {um.fireRateLevel})";

            if (ssjUpgradeCostText != null)
                ssjUpgradeCostText.text = um.ssjDurationLevel >= um.maxSsjLevel ? "MAX" : $"{um.GetSsjUpgradeCost()} G (Lv {um.ssjDurationLevel})";
        }

        public void ShowBossWarning(string bossName)
        {
            if (bossWarningBanner != null)
            {
                bossWarningBanner.SetActive(true);
                if (bossWarningText != null)
                {
                    bossWarningText.text = $"CẢNH BÁO: TRÙM CUỐI {bossName.ToUpper()} XUẤT HIỆN!";
                }
                StartCoroutine(HideBossWarningAfterDelay(3.5f));
            }
        }

        private IEnumerator HideBossWarningAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (bossWarningBanner != null)
            {
                bossWarningBanner.SetActive(false);
            }
        }

        public void ShowGameOverPanel(int finalScore)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (gameOverScoreText != null)
                {
                    gameOverScoreText.text = $"Điểm Đạt Được: {finalScore}";
                }
            }
        }

        public void ShowVictoryPanel(int finalScore)
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                if (victoryScoreText != null)
                {
                    victoryScoreText.text = $"Chiến Thắng! Điểm: {finalScore}\n7 Viên Ngọc Rồng An Toàn!";
                }
            }
        }
    }
}
