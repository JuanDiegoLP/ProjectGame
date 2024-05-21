using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Cargar la escena del juego
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Salir de la aplicación
        Application.Quit();
    }
}
