using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private bool isPaused = false;

    public GameObject pauseMenuPanel;
    public GameObject controlsPanel;
    private bool controlActive = false;
    
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            
            if (controlActive) 
            {
                ActivateControls(); 
            }
            else 
            {
                ResumeGame();
            }
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!isPaused && pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            isPaused = true;
            GameState.Instance.RequestPause(true); 
        }
    }

    public void ResumeGame()
    {
        if (isPaused && pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            isPaused = false;
            GameState.Instance.RequestPause(false);
        }
    }

    public void ActivateControls()
    {
        controlActive = !controlActive;
        controlsPanel.SetActive(controlActive);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Application Quit");
        Application.Quit();
    }
}
