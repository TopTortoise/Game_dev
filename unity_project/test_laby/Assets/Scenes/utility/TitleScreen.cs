using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty { Normal, TeacherMode }

public static class GameData 
{
    public static Difficulty selectedDifficulty = Difficulty.Normal;
}
public class TitleScreen : MonoBehaviour
{

    public GameObject startPanel;
    public GameObject difficultyPanel;

    public string sceneToLoad = "MainScene";
    void Start()
    {
       
        startPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }
    public void OnClickedStart()
    {
        startPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }
    public void OnNormalClicked()
    {
        GameData.selectedDifficulty = Difficulty.Normal;
        StartGame();
    }
    public void OnClick_TeacherMode()
    {
        GameData.selectedDifficulty = Difficulty.TeacherMode;
        StartGame();
    }
    public void OnClickedBack()
    {
        startPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }
    public void TutorialSceen()
    {
        SceneManager.LoadScene("TutorialSceen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
