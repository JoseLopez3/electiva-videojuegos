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
    private List<EnemyAI> activeEnemies = new List<EnemyAI>();

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
        // Suscribirse a todos los eventos que le importan
        GameEvents.OnPlayerHealthChanged += UpdatePlayerHealthUI;
        GameEvents.OnEnemyDied += OnEnemyDied;
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        // MUY IMPORTANTE: Desuscribirse para evitar errores
        GameEvents.OnPlayerHealthChanged -= UpdatePlayerHealthUI;
        GameEvents.OnEnemyDied -= OnEnemyDied;
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void OnEnemyDied()
    {
        // Eliminamos el primer enemigo nulo/inactivo que encontremos
        activeEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
        Debug.Log("activeEnemies '" + activeEnemies.Count );

        if (activeEnemies.Count == 1 && !isGameOver)
        {
            GameEvents.GameOver(true); // Anuncia victoria
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startTime;
        Time.timeScale = 1; // Se asegura de que el juego no esté pausado al empezar
        activeEnemies = new List<EnemyAI>(FindObjectsOfType<EnemyAI>());
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
            GameEvents.GameOver(false); // Se acabo el tiempo, el jugador pierde
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

    // --- CÓDIGO NUEVO CON DOTWEEN (DESPUÉS) ---
public void TogglePause()
{
    isPaused = !isPaused;

    if (isPaused)
    {
        Time.timeScale = 0f; // Pausamos el juego
        // --- PAUSA LA MÚSICA ---
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseMusic();
        }
        // Preparamos el panel para la animación de entrada
        pauseMenuPanel.transform.localScale = Vector3.zero; // Lo hacemos invisible al instante
        pauseMenuPanel.SetActive(true); // Lo activamos para poder animarlo

        // Creamos la animación de escalado para que aparezca
        pauseMenuPanel.transform.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack) // Un efecto de "rebote" muy agradable
            .SetUpdate(true); // ¡MUY IMPORTANTE! Para que la animación funcione aunque el juego esté pausado (Time.timeScale = 0)
    }
    else
    {
        // Creamos la animación para que se encoja y desaparezca
        pauseMenuPanel.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack) // El efecto de rebote inverso
            .SetUpdate(true) // También necesita esto para funcionar
            .OnComplete(() => {
                // Esto se ejecuta CUANDO la animación TERMINA
                pauseMenuPanel.SetActive(false); // Ahora sí lo desactivamos
                Time.timeScale = 1f; // Reanudamos el juego
                // --- REANUDA LA MÚSICA ---
                 if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.UnpauseMusic();
                }
            });
    }
}

// Añade este nuevo método a tu GameManager.cs

public void RestartGameWithAnimation()
{
    // Animamos el menú para que se encoja y desaparezca
    pauseMenuPanel.transform.DOScale(0f, 0.2f)
        .SetEase(Ease.InBack)
        .SetUpdate(true)
        .OnComplete(() => {
            // Cuando la animación termina, reiniciamos el juego
            Time.timeScale = 1f; // ¡Muy importante restaurar el tiempo antes de cambiar de escena!
            RestartGame(); // Llamamos a tu método original de reinicio
        });
}

// Puedes crear otro método similar para volver al menú principal
public void QuitToMenuWithAnimation()
{
    // Animamos el menú para que se encoja y desaparezca
    pauseMenuPanel.transform.DOScale(0f, 0.2f)
        .SetEase(Ease.InBack)
        .SetUpdate(true)
        .OnComplete(() => {
            // Cuando la animación termina, volvemos al menú
            Time.timeScale = 1f;
            // Aquí iría tu lógica para cargar la escena del menú principal, por ejemplo:
            // SceneManager.LoadScene("MainMenu"); 
        });
}

    private void HandleGameOver(bool playerWon)
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

        // Preparamos el panel para la animación de entrada
    gameOverMenuPanel.transform.localScale = Vector3.zero; // Lo hacemos invisible al instante
    gameOverMenuPanel.SetActive(true); // Lo activamos para poder animarlo

    // Creamos la animación de escalado para que aparezca
    gameOverMenuPanel.transform.DOScale(1f, 0.5f) // Le damos un poco más de tiempo para que sea más dramático
        .SetEase(Ease.OutElastic) // Un efecto elástico es genial para pantallas de fin de nivel
        .SetUpdate(true); // Para que la animación funcione con Time.timeScale = 0
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
    private void UpdatePlayerHealthUI(int currentHealth, int maxHealth)
    {
        playerHealthBar.maxValue = maxHealth;
        playerHealthBar.value = currentHealth;
    }

    // public void RegisterEnemy(EnemyAI enemy)
    // {
    //     activeEnemies.Add(enemy);
    // }

    // public void UnregisterEnemy(EnemyAI enemy)
    // {
    //     activeEnemies.Remove(enemy);
    //     Debug.Log("activeEnemies '" + activeEnemies.Count );
    // Debug.Log("isGameOver '" + !isGameOver );

    //     if (activeEnemies.Count == 0 && !isGameOver)
    //     {
    //         GameEvents.GameOver(true); // Todos los enemigos derrotados, el jugador gana
    //     }
    // }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}