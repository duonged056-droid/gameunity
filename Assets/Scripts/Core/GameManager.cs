using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DragonBallDefenders
{
    public enum GameState
    {
        Starting,
        Playing,
        WaveBreak,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState currentState = GameState.Starting;

        [Header("Economy & Resources")]
        public int currentZeni = 300; // Starting money
        public float currentKi = 100f;
        public float maxKi = 100f;
        public int currentScore = 0;

        [Header("Base Reference")]
        public DragonBallAltar dragonBallAltar;

        [Header("UI Reference")]
        public UIManager uiManager;

        public event Action<int> OnZeniChanged;
        public event Action<float, float> OnKiChanged;
        public event Action<int> OnScoreChanged;
        public event Action<GameState> OnGameStateChanged;

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
            SetGameState(GameState.Playing);
            OnZeniChanged?.Invoke(currentZeni);
            OnKiChanged?.Invoke(currentKi, maxKi);
            OnScoreChanged?.Invoke(currentScore);
        }

        private void Update()
        {
            // Passive Ki regeneration
            if (currentState == GameState.Playing || currentState == GameState.WaveBreak)
            {
                AddKi(8f * Time.deltaTime);
            }
        }

        public void SetGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);

            if (newState == GameState.GameOver)
            {
                SoundManager.Instance?.PlayGameOverSound();
                uiManager?.ShowGameOverPanel(currentScore);
            }
            else if (newState == GameState.Victory)
            {
                SoundManager.Instance?.PlayVictorySound();
                uiManager?.ShowVictoryPanel(currentScore);
            }
        }

        public void AddZeni(int amount)
        {
            currentZeni += amount;
            OnZeniChanged?.Invoke(currentZeni);
            SoundManager.Instance?.PlayCoinSound();
        }

        public bool SpendZeni(int amount)
        {
            if (currentZeni >= amount)
            {
                currentZeni -= amount;
                OnZeniChanged?.Invoke(currentZeni);
                return true;
            }
            return false;
        }

        public void AddKi(float amount)
        {
            currentKi = Mathf.Clamp(currentKi + amount, 0f, maxKi);
            OnKiChanged?.Invoke(currentKi, maxKi);
        }

        public bool ConsumeKi(float amount)
        {
            if (currentKi >= amount)
            {
                currentKi -= amount;
                OnKiChanged?.Invoke(currentKi, maxKi);
                return true;
            }
            return false;
        }

        public void AddScore(int amount)
        {
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
