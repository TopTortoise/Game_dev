using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private bool isPaused = false;

    public GameObject pauseMenuPanel;
    public GameObject controlsPanel; 
    private bool controlActive =  false; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(false); 
    }

       public void TogglePause()
    {
        if (isPaused)
        {
            if(controlActive) {
                controlsPanel.SetActive(false);
                controlActive = false; 
            }
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Menü an
            Time.timeScale = 0f;            // Zeit stopp
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Menü aus
            Time.timeScale = 1f;             // Zeit läuft
            isPaused = false;
        }
    }

    public void ActivateControls()
    {
        if (controlActive)
        {
            controlsPanel.SetActive(false);
            controlActive = false;
        }
        else
        {
            controlsPanel.SetActive(true);
            controlActive = true;
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("Application Quit");
          Application.Quit();
    }
}
