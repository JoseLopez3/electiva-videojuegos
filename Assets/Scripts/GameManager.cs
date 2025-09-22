using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Logic")]
    [SerializeField] private float startTime = 30f; // 30 segundos
    private float currentTime;
    private bool isGameOver = false;

    [Header("Enemy Tracking")]
    private List<EnemyAI> activeEnemies = new List<EnemyAI>();

    [Header("UI References")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private TextMeshProUGUI endGameText;
    
    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startTime;
        Time.timeScale = 1; // Se asegura de que el juego no esté pausado al empezar
    }

    void Update()
    {
        if (isGameOver) return;

        HandleTimer();
        HandlePauseInput();
    }

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver(false); // Se acabo el tiempo, el jugador pierde
        }
        
        // Formateo del tiempo
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
            Time.timeScale = 0f; // Pausa el juego
            pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el juego
            pauseMenuPanel.SetActive(false);
        }
    }

    public void GameOver(bool playerWon)
    {
        if (isGameOver) return; 
        
        
        isGameOver = true;
        Time.timeScale = 0f;
        
        gameOverMenuPanel.SetActive(true);

        if (playerWon)
        {
            endGameText.text = "¡VICTORIA!";
        }
        else
        {
            endGameText.text = "DERROTA";
        }
    }

    //Metodos para botones
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
    
    //Metodos de UI y enemigos
    public void UpdatePlayerHealthUI(int currentHealth, int maxHealth)
    {
        playerHealthBar.maxValue = maxHealth;
        playerHealthBar.value = currentHealth;
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        activeEnemies.Remove(enemy);
        if (activeEnemies.Count == 0 && !isGameOver)
        {
            GameOver(true); // Todos los enemigos derrotados, el jugador gana
        }
    }
}