using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class LoreManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogLine
    {
        public string speakerName;
        [TextArea(3, 10)]
        public string text;
        public bool isGoddess;
    }

    [Header("UI Referenzen")]
    public Image goddessImage;
    public Image playerImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Story Daten")]
    public List<DialogLine> dialogueLines;
    public string nextSceneName = "MainScene";

    private int currentLineIndex = 0;
    private bool isTyping = false;       
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowDialog();
    }

    void Update()
    {
        bool pressedNext = false;


        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pressedNext = true;
        }

        else if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            pressedNext = true;
        }

        if (pressedNext)
        {
            NextLine();
        }
    }
    void CompleteTextImmediately()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        dialogueText.text = dialogueLines[currentLineIndex].text;
        isTyping = false;
    }
     public static void DestroyAllDontDestroyOnLoadObjects()
    {
       
        GameObject temp = new GameObject("TempDDOLFinder");
        Object.DontDestroyOnLoad(temp);

        Scene ddolScene = temp.scene;

        GameObject[] roots = ddolScene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            Object.Destroy(roots[i]);
        }
    }

    void ShowDialog()
    {
        if (dialogueLines.Count == 0) return;

        DialogLine currentLine = dialogueLines[currentLineIndex];

        nameText.text = currentLine.speakerName;
        dialogueText.text = currentLine.text;

        if (currentLine.isGoddess)
        {
            SetImageActive(goddessImage, true);
            SetImageActive(playerImage, false);
        }
        else
        {
            SetImageActive(goddessImage, false);
            SetImageActive(playerImage, true);
        }
    }


    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Count)
        {
            ShowDialog();
        }
        else
        {
            DestroyAllDontDestroyOnLoadObjects();
           
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void SetImageActive(Image img, bool isActive)
    {
        if (isActive)
        {
            img.color = new Color(1f, 1f, 1f, 1f);
            img.transform.localScale = Vector3.one * 1.1f;
        }
        else
        {
            img.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            img.transform.localScale = Vector3.one;
        }
    }
}