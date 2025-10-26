using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.SceneManagement; 
using DG.Tweening; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Logic")]
    [SerializeField] private float startTime = 30f; // 30 segundos
    private float currentTime;
    private bool isGameOver = false;

    [Header("Enemy Tracking")]
    private List<EnemyAI> allEnemiesInScene = new List<EnemyAI>(); 
    private List<EnemyAI> visuallyDeadEnemies = new List<EnemyAI>(); 

    [Header("UI References")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private TextMeshProUGUI endGameText;

    [Header("Audio")]
    [SerializeField] private AudioClip levelMusic;
    
    private bool isPaused = false;

    private void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdatePlayerHealthUI;
        GameEvents.OnEnemyVisuallyDied += OnEnemyVisuallyDiedHandler;
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdatePlayerHealthUI;
        GameEvents.OnEnemyVisuallyDied -= OnEnemyVisuallyDiedHandler;
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void OnEnemyVisuallyDiedHandler(EnemyAI enemy)
    {
        if (enemy != null && enemy.isVisuallyDead && !visuallyDeadEnemies.Contains(enemy))
        {
            visuallyDeadEnemies.Add(enemy);
            Debug.Log($"Enemigo '{enemy.name}' ahora está visualmente muerto. Total visualmente muertos: {visuallyDeadEnemies.Count}");
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (visuallyDeadEnemies.Count == allEnemiesInScene.Count && !isGameOver)
        {
            Debug.Log("¡Todos los enemigos han muerto visualmente! Procediendo a la victoria.");
            isGameOver = true;
            StartCoroutine(DeactivateAllVisuallyDeadEnemiesAndWin());
        }
    }

    private IEnumerator DeactivateAllVisuallyDeadEnemiesAndWin()
    {
        yield return new WaitForSeconds(1.5f); 

        // --- CORRECCIÓN AQUÍ: Crear una copia de la lista antes de iterar ---
        List<EnemyAI> enemiesToDeactivate = new List<EnemyAI>(visuallyDeadEnemies);

        foreach (EnemyAI enemy in enemiesToDeactivate) // Iterar sobre la COPIA
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.hasDiedCompletely)
            {
                enemy.DeactivateCompletely(); 
            }
        }
        
        GameEvents.GameOver(true);
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = startTime;
        Time.timeScale = 1;

        allEnemiesInScene.Clear();
        visuallyDeadEnemies.Clear();
        // Asegurarse de que esta línea se ejecuta DESPUÉS de que todos los enemigos se hayan inicializado
        // Si los enemigos se generan dinámicamente o salen de un pool al inicio, esta lista debe actualizarse.
        allEnemiesInScene.AddRange(FindObjectsOfType<EnemyAI>(true)); 

        Debug.Log($"GameManager inició. Total de enemigos detectados en escena: {allEnemiesInScene.Count}");

        if (AudioManager.Instance != null && levelMusic != null)
        {
            AudioManager.Instance.PlayMusic(levelMusic);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        HandleTimer();
        HandlePauseInput();
    }

    public void GoToMenu(){
        SceneManager.LoadScene(0);
    }

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameEvents.GameOver(false); 
        }
        
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseMusic();
            }
            pauseMenuPanel.transform.localScale = Vector3.zero;
            pauseMenuPanel.SetActive(true);
            pauseMenuPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            pauseMenuPanel.transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .OnComplete(() => {
                    pauseMenuPanel.SetActive(false);
                    Time.timeScale = 1f;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.UnpauseMusic();
                    }
                });
        }
    }

    public void RestartGameWithAnimation()
    {
        pauseMenuPanel.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => {
                Time.timeScale = 1f;
                RestartGame();
            });
    }

    public void QuitToMenuWithAnimation()
    {
        pauseMenuPanel.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0); 
            });
    }

    private void HandleGameOver(bool playerWon)
    {
        if (isGameOver && playerWon) 
        {
            if (endGameText.text == "¡VICTORIA!" && playerWon) return; 
        }
        
        isGameOver = true;
        Time.timeScale = 0f;
        
        gameOverMenuPanel.SetActive(true);

        if (playerWon)
        {
            endGameText.text = "¡VICTORIA!";
            if (AudioManager.Instance != null) AudioManager.Instance.StopMusic(); 
        }
        else
        {
            endGameText.text = "DERROTA";
            if (AudioManager.Instance != null) AudioManager.Instance.StopMusic(); 
        }

        gameOverMenuPanel.transform.localScale = Vector3.zero;
        gameOverMenuPanel.SetActive(true);
        gameOverMenuPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic).SetUpdate(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
    
    private void UpdatePlayerHealthUI(int currentHealth, int maxHealth)
    {
        playerHealthBar.maxValue = maxHealth;
        playerHealthBar.value = currentHealth;
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (!allEnemiesInScene.Contains(enemy))
        {
            allEnemiesInScene.Add(enemy);
            Debug.Log($"Enemigo registrado: {enemy.name}. Total: {allEnemiesInScene.Count}");
        }
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        allEnemiesInScene.Remove(enemy); 
        visuallyDeadEnemies.Remove(enemy); // Esto es seguro ya que Remove no falla si el elemento no está.
        Debug.Log($"Enemigo desregistrado: {enemy.name}. Total activos restantes: {allEnemiesInScene.Count}. Visualmente muertos: {visuallyDeadEnemies.Count}");
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}