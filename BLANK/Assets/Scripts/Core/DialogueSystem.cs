using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI")]
    public Text thoughtText;           // Assign Text component in Inspector
    public GameObject thoughtPanel;    // Optional background panel

    [Header("Settings")]
    public float defaultDuration = 3f;

    private Coroutine currentThoughtCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (thoughtPanel != null)
            thoughtPanel.SetActive(false);
        
        if (thoughtText != null)
            thoughtText.text = "";
    }

    public void ShowThought(string text, float duration = -1f)
    {
        if (duration <= 0) duration = defaultDuration;

        if (currentThoughtCoroutine != null)
            StopCoroutine(currentThoughtCoroutine);

        currentThoughtCoroutine = StartCoroutine(ShowThoughtCoroutine(text, duration));
    }

    private IEnumerator ShowThoughtCoroutine(string text, float duration)
    {
        if (thoughtPanel != null) thoughtPanel.SetActive(true);
        if (thoughtText != null) thoughtText.text = text;

        yield return new WaitForSeconds(duration);

        if (thoughtText != null) thoughtText.text = "";
        if (thoughtPanel != null) thoughtPanel.SetActive(false);
    }
}