using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MenuInicial : MonoBehaviour
{
    [Header("Menus")]
    public GameObject principalMenu;
    public GameObject optionsMenu;

    [Header("Level Buttons")]
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        
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
    
        SceneManager.LoadScene(1); 
    }

    // Carga el Nivel 2, pero solo si está desbloqueado
    public void play_level2()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (highestLevelReached >= 2)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("Nivel 2 está bloqueado. ¡Completa el nivel anterior primero!");
        }
    }

    // Carga el Nivel 3, pero solo si está desbloqueado
    public void play_level3()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (highestLevelReached >= 3)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
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