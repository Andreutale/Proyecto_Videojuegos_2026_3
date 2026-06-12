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
        // Ocultamos la configuración y volvemos a mostrar la pausa normal
        if (configMenu != null) configMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
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