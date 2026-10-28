using System;
using UnityEngine;

namespace DragonBallDefenders
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Upgrade Levels")]
        public int damageLevel = 1;
        public int fireRateLevel = 1;
        public int ssjDurationLevel = 1;

        [Header("Max Upgrade Levels")]
        public int maxDamageLevel = 5;
        public int maxFireRateLevel = 5;
        public int maxSsjLevel = 5;

        [Header("Base Upgrade Costs (Zeni)")]
        public int baseDamageUpgradeCost = 100;
        public int baseFireRateUpgradeCost = 120;
        public int baseSsjUpgradeCost = 150;
        public int senzuBaseHealCost = 100;

        [Header("Ally Deployment Costs (Zeni)")]
        public int bulmaTurretCost = 150;
        public int vegetaCost = 250;
        public int piccoloCost = 300;

        [Header("Ally Prefabs")]
        public GameObject bulmaTurretPrefab;
        public GameObject vegetaPrefab;
        public GameObject piccoloPrefab;

        [Header("Player Reference")]
        public DefenderAttack playerAttack;

        public event Action OnUpgradePurchased;

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

        public int GetDamageUpgradeCost() => baseDamageUpgradeCost * damageLevel;
        public int GetFireRateUpgradeCost() => baseFireRateUpgradeCost * fireRateLevel;
        public int GetSsjUpgradeCost() => baseSsjUpgradeCost * ssjDurationLevel;

        public bool UpgradeDamage()
        {
            if (damageLevel >= maxDamageLevel) return false;
            int cost = GetDamageUpgradeCost();

            if (GameManager.Instance != null && GameManager.Instance.SpendZeni(cost))
            {
                damageLevel++;
                if (playerAttack != null)
                {
                    playerAttack.baseDamage = Mathf.RoundToInt(playerAttack.baseDamage * 1.25f);
                }
                SoundManager.Instance?.PlaySuperSaiyanSound();
                OnUpgradePurchased?.Invoke();
                return true;
            }
            return false;
        }

        public bool UpgradeFireRate()
        {
            if (fireRateLevel >= maxFireRateLevel) return false;
            int cost = GetFireRateUpgradeCost();

            if (GameManager.Instance != null && GameManager.Instance.SpendZeni(cost))
            {
                fireRateLevel++;
                if (playerAttack != null)
                {
                    playerAttack.fireRate = playerAttack.fireRate * 1.2f;
                }
                SoundManager.Instance?.PlaySuperSaiyanSound();
                OnUpgradePurchased?.Invoke();
                return true;
            }
            return false;
        }

        public bool UpgradeSuperSaiyan()
        {
            if (ssjDurationLevel >= maxSsjLevel) return false;
            int cost = GetSsjUpgradeCost();

            if (GameManager.Instance != null && GameManager.Instance.SpendZeni(cost))
            {
                ssjDurationLevel++;
                if (playerAttack != null)
                {
                    playerAttack.superSaiyanDuration += 2.5f;
                }
                SoundManager.Instance?.PlaySuperSaiyanSound();
                OnUpgradePurchased?.Invoke();
                return true;
            }
            return false;
        }

        public bool BuySenzuBaseHeal()
        {
            if (DragonBallAltar.Instance == null) return false;
            if (DragonBallAltar.Instance.currentHealth >= DragonBallAltar.Instance.maxHealth) return false;

            if (GameManager.Instance != null && GameManager.Instance.SpendZeni(senzuBaseHealCost))
            {
                DragonBallAltar.Instance.Heal(250);
                OnUpgradePurchased?.Invoke();
                return true;
            }
            return false;
        }

        public bool DeployAllyToEmptySlot(AllyType type)
        {
            int cost = (type == AllyType.BulmaTurret) ? bulmaTurretCost :
                       (type == AllyType.Vegeta ? vegetaCost : piccoloCost);

            GameObject prefab = (type == AllyType.BulmaTurret) ? bulmaTurretPrefab :
                                (type == AllyType.Vegeta ? vegetaPrefab : piccoloPrefab);

            if (GameManager.Instance == null || !GameManager.Instance.SpendZeni(cost))
            {
                return false;
            }

            // Find first available slot
            DefenderSlot[] slots = FindObjectsByType<DefenderSlot>();
            foreach (var slot in slots)
            {
                if (!slot.isOccupied)
                {
                    slot.DeployAlly(prefab);
                    SoundManager.Instance?.PlaySuperSaiyanSound();
                    OnUpgradePurchased?.Invoke();
                    return true;
                }
            }

            // If no slot was available, refund Zeni
            GameManager.Instance.AddZeni(cost);
            return false;
        }
    }
}
