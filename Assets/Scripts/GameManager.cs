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
    [SerializeField] private float startTime = 30f;
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
    [SerializeField] private GameObject introStoryPanel;

    
    // puntuación 
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI finalScoreText; 

    [Header("Score System - NEW")]
    private int currentScore = 0;
    [SerializeField] private float scoreInterval = 0.5f; // Intervalo para añadir puntos
    [SerializeField] private int scorePerInterval = 10;   // Puntos añadidos por intervalo
    [SerializeField] private float invulnerabilityGracePeriod = 2.0f; // Tiempo para reanudar puntuación después de daño
    private bool canGainScore = true;
    private Coroutine scoreGracePeriodCoroutine;
    //Referencias para fin de nivel
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject restartButton;

    [Header("Audio")]
    [SerializeField] private AudioClip levelMusic;

    [Header("Level Logic")]
    [SerializeField] private int currentLevelBuildIndex; 

    [Header("Intro Sequence")]
    [SerializeField] private float introDuration = 20f;
    
    private bool isPaused = false;

    private void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdatePlayerHealthUI;
        GameEvents.OnEnemyVisuallyDied += OnEnemyVisuallyDiedHandler;
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnPlayerTookDamage += OnPlayerTookDamageHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdatePlayerHealthUI;
        GameEvents.OnEnemyVisuallyDied -= OnEnemyVisuallyDiedHandler;
        GameEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnPlayerTookDamage -= OnPlayerTookDamageHandler;
    }

    //  Manejador para cuando el jugador recibe daño 
    private void OnPlayerTookDamageHandler()
    {
        if (canGainScore) // Solo pausar si estaba ganando puntos
        {
            canGainScore = false;
            if (scoreGracePeriodCoroutine != null)
            {
                StopCoroutine(scoreGracePeriodCoroutine);
            }
            scoreGracePeriodCoroutine = StartCoroutine(ScoreGracePeriod());
            Debug.Log("Jugador recibió daño, puntuación pausada.");
        }
    }

    private IEnumerator ScoreGracePeriod()
    {
        yield return new WaitForSeconds(invulnerabilityGracePeriod);
        canGainScore = true;
        Debug.Log("Puntuación reanudada.");
    }
    // --------------------------------------------------------------------

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
    if (allEnemiesInScene.Count > 0 && visuallyDeadEnemies.Count >= allEnemiesInScene.Count && !isGameOver)
    {
        Debug.Log("¡Condición de victoria cumplida! Iniciando corrutina de fin de nivel.");
        


        canGainScore = false; 
        if (scoreGracePeriodCoroutine != null) StopCoroutine(scoreGracePeriodCoroutine);
        
        StartCoroutine(DeactivateAllVisuallyDeadEnemiesAndWin());
    }
}

    private IEnumerator DeactivateAllVisuallyDeadEnemiesAndWin()
    {
        yield return new WaitForSeconds(1.5f); 

        List<EnemyAI> enemiesToDeactivate = new List<EnemyAI>(visuallyDeadEnemies);

        foreach (EnemyAI enemy in enemiesToDeactivate) 
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
        currentLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
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
        allEnemiesInScene.AddRange(FindObjectsOfType<EnemyAI>(true)); 

        Debug.Log($"GameManager inició. Total de enemigos detectados en escena: {allEnemiesInScene.Count}");

        if (AudioManager.Instance != null && levelMusic != null) AudioManager.Instance.PlayMusic(levelMusic);

        //iniciamos la secuencia de introducción.
        StartCoroutine(StartIntroSequence());
    }

    private IEnumerator StartIntroSequence()
    {
        Debug.Log("Iniciando secuencia de introducción...");

        //Se Bloquea al jugador para dar lugar a la narracion
        MovementPhisic player = FindObjectOfType<MovementPhisic>();
        if (player != null)
        {
            player.canMove = false;
        }

        // Desactivar el aumento de puntuación
        canGainScore = false;
        currentScore = 0; 
        UpdateScoreUI(); 

        if (introStoryPanel != null)
        {
            introStoryPanel.SetActive(true);
        }

        // Esperar los segundos definidos en introDuration
        yield return new WaitForSeconds(introDuration);

        Debug.Log("Fin de la introducción. ¡El juego comienza!");

        if (introStoryPanel != null)
        {
            introStoryPanel.SetActive(false);
        }

        // Desbloquear al jugador
        if (player != null)
        {
            player.canMove = true;
        }

        canGainScore = true;
        StartCoroutine(AddScoreOverTime());
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
    if (isGameOver) return;
    isGameOver = true;
    Time.timeScale = 0f;

    if (nextLevelButton != null) nextLevelButton.SetActive(false);
    if (restartButton != null) restartButton.SetActive(true);

    if (playerWon)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.StopMusic();

        int nextLevelIndex = currentLevelBuildIndex + 1;
        
        // Comprobar si es el último nivel 
        
        // Si son iguales, significa que no hay más niveles después de este.
        bool isLastLevel = (nextLevelIndex == SceneManager.sceneCountInBuildSettings);

        if (isLastLevel)
        {
            // Es el último nivel
            endGameText.text = "¡VICTORIA!\n\nTras acabar con el último enemigo, una explosión de energía pura desintegra a los cultistas restantes y silencia las catacumbas. El regreso de Mordekaiser ha sido frustrado, y la oscuridad se retira una vez más ante el poder de LeBlanc.";
            
            if (nextLevelButton != null) nextLevelButton.SetActive(false);
            
             if (restartButton != null) restartButton.SetActive(false);
        }
        else
        {
            endGameText.text = "¡VICTORIA!";
            
            int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);

            if (nextLevelIndex > highestLevelReached)
            {
                PlayerPrefs.SetInt("LevelReached", nextLevelIndex);
                PlayerPrefs.Save();
                Debug.Log("Nuevo nivel desbloqueado: " + nextLevelIndex);
            }
            
            if (nextLevelButton != null) nextLevelButton.SetActive(true);
            if (restartButton != null) restartButton.SetActive(false);
        }

    }
    else
    {
        endGameText.text = "DERROTA";
        if (AudioManager.Instance != null) AudioManager.Instance.StopMusic();
    }

    if (finalScoreText != null)
    {
        finalScoreText.text = "Puntuación Final: " + currentScore;
    }

    gameOverMenuPanel.transform.localScale = Vector3.zero;
    gameOverMenuPanel.SetActive(true);
    gameOverMenuPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic).SetUpdate(true);
}

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; 
        int nextLevelIndex = currentLevelBuildIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning("No hay más niveles. Volviendo al menú principal.");
            SceneManager.LoadScene(0); 
        }
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
        visuallyDeadEnemies.Remove(enemy); 
        Debug.Log($"Enemigo desregistrado: {enemy.name}. Total activos restantes: {allEnemiesInScene.Count}. Visualmente muertos: {visuallyDeadEnemies.Count}");
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }


    private IEnumerator AddScoreOverTime()
    {
        while (!isGameOver) 
        {
            yield return new WaitForSeconds(scoreInterval); 

            if (canGainScore && !isPaused) // Solo sumar puntos si el jugador no ha recibido daño y el juego no está pausado
            {
                currentScore += scorePerInterval;
                UpdateScoreUI();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }
}