using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // change name if needed
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
