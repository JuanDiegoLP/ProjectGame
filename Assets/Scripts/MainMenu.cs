using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public enum MenuState { Main }

    private MenuState currentState = MenuState.Main;

    public void StartGame()
    {
        // Cargar la escena del juego
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Salir de la aplicaci�n
        Application.Quit();
    }
}
