using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HardReset : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(ResetGame());
    }

    IEnumerator ResetGame()
    {
        // Wait one frame so Unity finishes scene switch
        yield return null;

        // Destroy ALL DontDestroyOnLoad objects
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        yield return null;

        
        SceneManager.LoadScene("MainScene");
    }
}
