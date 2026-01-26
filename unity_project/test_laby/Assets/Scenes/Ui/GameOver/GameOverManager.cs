
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvasGroup;
    public float fadeSpeed = 0.5f;

    private bool isDead = false;

    void Start()
    {
    
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;            
            gameOverCanvasGroup.interactable = false; 
            gameOverCanvasGroup.blocksRaycasts = false; 
        }
        isDead = false;
    }
    public void StartGameOver()
    {
        if (isDead) return;

        isDead = true;
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            gameOverCanvasGroup.alpha = alpha;
            yield return null;
        }

        gameOverCanvasGroup.interactable = true;
        gameOverCanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        GameState.Instance.ResetGameState();
        SceneManager.LoadScene("MainScene");
        GameState.Instance.StartNewCycle(100);
    }

    public void BackToMenu()
    {
         Application.Quit();
    }
    public void Controll()
    {
        
    }

}
