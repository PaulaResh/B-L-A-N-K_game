using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    [Header("Ending Scenes (optional)")]
    public string badEndingScene = "BadEnding";
    public string goodEndingScene = "GoodEnding";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TriggerBadEnding()
    {
        Debug.Log("[EndingManager] Плохая концовка (лифт)");
        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ShowThought("Это был неправильный выбор...", 4f);

        // You can load scene or just show UI
        // SceneManager.LoadScene(badEndingScene);
    }

    public void TriggerGoodEnding()
    {
        Debug.Log("[EndingManager] Хорошая концовка (лестница)");
        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ShowThought("Ты выбрался...", 5f);

        // SceneManager.LoadScene(goodEndingScene);
    }
}