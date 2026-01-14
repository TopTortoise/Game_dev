using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Falls du TextMeshPro benutzt

public class GameOverManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvasGroup;
    public float fadeSpeed = 0.5f;
    private bool isDead = false;

    public void StartGameOver()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine(FadeToBlack());
        }
    }

    IEnumerator FadeToBlack()
    {
        // Wir warten nicht auf Time.timeScale = 0, damit Animationen weiterlaufen!
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            gameOverCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Erst wenn alles schwarz ist, machen wir die Buttons klickbar
        gameOverCanvasGroup.interactable = true;
        gameOverCanvasGroup.blocksRaycasts = true;
        
        // Optional: Hier könntest du das Spiel pausieren, wenn alles schwarz ist
        // Time.timeScale = 0; 
    }

    public void PlayAgain()
    {
        Time.timeScale = 1; // Zeit wieder normalisieren
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FindFirstObjectByType<ghost>().Awake();

    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu"); // Name deiner Menü-Szene
    }
}
