using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using DG.Tweening;

public class MenuInicial : MonoBehaviour
{
    [Header("Menus")]
    public GameObject principalMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject tutorialMenu;
    public AudioSource EnterToPlay;
    public AudioSource CanNotEnter;


    [Header("Level Buttons")]
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        if (AudioManager.Instance != null)
    {
        AudioManager.Instance.StopMusic();
    }
        
        UpdateButtonStates();
    }

    // Esta función actualiza si los botones son interactuables o no
    public void UpdateButtonStates()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);

        if (level2Button != null)
        {
            level2Button.interactable = (highestLevelReached >= 2);
        }

        if (level3Button != null)
        {
            level3Button.interactable = (highestLevelReached >= 3);
        }
    }

    // Carga el Nivel 1 (siempre disponible)
    public void play()
    {
        EnterToPlay.Play();
        DOTween.KillAll();
        
        SceneManager.LoadScene(1); 
    }

    // Carga el Nivel 2, pero solo si está desbloqueado
    public void play_level2()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (highestLevelReached >= 2)
        {
            EnterToPlay.Play();
            DOTween.KillAll();
            SceneManager.LoadScene(2);
        }
        else
        {
            CanNotEnter.Play();
            Debug.Log("Nivel 2 está bloqueado. ¡Completa el nivel anterior primero!");
        }
    }

    // Carga el Nivel 3, pero solo si está desbloqueado
    public void play_level3()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (highestLevelReached >= 3)
        {
            EnterToPlay.Play();
            DOTween.KillAll();
            SceneManager.LoadScene(3);
        }
        else
        {
            CanNotEnter.Play();
            Debug.Log("Nivel 3 está bloqueado. ¡Completa los niveles anteriores primero!");
        }
    }

    public void MostrarMenuOpciones()
    {
        principalMenu.SetActive(false); 
        optionsMenu.SetActive(true);    
    }

    public void MostrarMenuPrincipal()
    {
        principalMenu.SetActive(true);    
        optionsMenu.SetActive(false);    
        creditsMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void MostrarMenuCreditos()
    {
        principalMenu.SetActive(false); 
        creditsMenu.SetActive(true);    
    }

    public void mostrarTutorialMenu()
    {
        principalMenu.SetActive(false); 
        tutorialMenu.SetActive(true);    
    }

  

    public void exit()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    //  Función para resetear el progreso para pruebas
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("LevelReached");
        Debug.Log("Progreso reseteado. Solo el Nivel 1 está disponible.");
        UpdateButtonStates();
    }
}