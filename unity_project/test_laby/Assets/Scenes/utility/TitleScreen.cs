using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LoreScene"); // change name if needed
    }
    public void TutorialSceen()
    {
        SceneManager.LoadScene("TutorialSceen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
