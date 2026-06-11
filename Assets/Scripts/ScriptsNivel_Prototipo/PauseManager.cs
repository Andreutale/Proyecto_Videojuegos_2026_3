using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public Animator pauseAnimator;
    public GameObject CanvasAjustes;

    private bool isPaused = false;

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

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

        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        Debug.Log("Abriendo ajustes desde: " + gameObject.name);

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (CanvasAjustes != null)
            CanvasAjustes.SetActive(true);
        else
            Debug.LogError("CanvasAjustes NO asignado en el Inspector");
    }

    public void CloseSettings()
    {
        if (CanvasAjustes != null)
            CanvasAjustes.SetActive(false);

        if (pauseMenu != null)
            pauseMenu.SetActive(true);
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