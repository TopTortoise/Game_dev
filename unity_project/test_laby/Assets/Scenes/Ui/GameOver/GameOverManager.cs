
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvasGroup;
    public float fadeSpeed = 0.5f;
    public GameOverUI gameoverUiComponent;
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
        gameoverUiComponent.GetComponent<GameOverUI>().RefreshData();
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
        GameState.Instance.RequestPause(true);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        GameState.Instance.ResetGameState();
        //Destroy(gameObject); 
        SceneManager.LoadScene("MainScene");
        GameState.Instance.StartNewCycle(10);
    }

    public void BackToMenu()
    {
         Application.Quit();
    }
    public void Controll()
    {
        
    }

}
