using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicial : MonoBehaviour
{
    public GameObject principalMenu;
    public GameObject optionsMenu;
    public void play(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void play_level2(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void play_level3(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

     public void MostrarMenuOpciones()
    {
        principalMenu.SetActive(false); 
        optionsMenu.SetActive(true);    
    }

    // 4. Función pública para volver al menú principal
    public void MostrarMenuPrincipal()
    {
        principalMenu.SetActive(true);    
        optionsMenu.SetActive(false);    
    }

    // Update is called once per frame
    public void exit(){
        Debug.Log("Salir...");
        Application.Quit();
    }
}
