using UnityEngine;
using UnityEngine.SceneManagement;

public class ActManager : MonoBehaviour
{
    public static ActManager Instance { get; private set; }

    public enum Act { Act1, Act2, Act3, Act4 }

    [Header("Current State")]
    public Act currentAct = Act.Act1;

    public string GetRequiredItemForCurrentAct()
    {
        switch (currentAct)
        {
            case Act.Act1: return "Key";
            case Act.Act2: return "Fuse";
            case Act.Act3: return "Switch";
            default: return "";
        }
    }

    public void AdvanceToNextAct()
    {
        if (currentAct == Act.Act4) return;

        currentAct++;
        Debug.Log($"[ActManager] Переход в {currentAct}");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}