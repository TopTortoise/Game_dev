using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("JonaScene"); // change name if needed
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
