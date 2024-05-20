using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public enum MenuState { Active, Inactive }

    private MenuState currentState;

    public GameObject pauseMenuUI;

    private void Start()
    {
        currentState = MenuState.Inactive;
        pauseMenuUI.SetActive(false);
    }

    private void Update()
    {
        switch (currentState)
        {
            case MenuState.Active:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResumeGame();
                }
                break;
            case MenuState.Inactive:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }
                break;
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        currentState = MenuState.Active;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reanudar el juego
        currentState = MenuState.Inactive;
    }

    public void GoToMainMenu()
    {
        // Cargar la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }
}
