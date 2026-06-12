using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject configMenu;
    public GameObject pauseButton;
    public Animator pauseAnimator;

    private bool isPaused = false;

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        if (configMenu != null)
            configMenu.SetActive(false);

        if (pauseAnimator != null)
        {
            pauseAnimator.Play("PauseMenu", 0, 0f);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        if (configMenu != null)
            configMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    public void AbrirConfiguracion()
    {
        // Ocultamos los botones de pausa y mostramos los de configuración
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (configMenu != null) configMenu.SetActive(true);
    }

    public void CerrarConfiguracion()
    {
        Debug.Log("Cerrando configuración y regresando a la pausa...");

        // 1. Apagamos los ajustes y encendemos el menú de pausa
        if (configMenu != null) configMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);

        // 2. ¡LA CLAVE! Aseguramos que el botón de pausa de la pantalla permanezca apagado 
        // mientras sigamos en el menú de pausa principal.
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    public void HidePauseButton()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);
    }

    public void ShowPauseButton()
    {
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MenuInicial");
    }
}