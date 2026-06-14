using UnityEngine;

public class ActManager : MonoBehaviour
{
    public static ActManager Instance { get; private set; }

    public enum GameAct { Act1, Act2, Act3, Act4 }
    public GameAct currentAct = GameAct.Act1;

    [Header("References")]
    public MonsterController monsterController;
    public DialogueSystem dialogueSystem;

    [Header("Act Messages")]
    public string act2Message = "Кажется, кто-то появился...";
    public string act3Message = "Он начал охоту...";
    public string act4Message = "Беги. Не останавливайся.";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (monsterController == null)
            monsterController = FindObjectOfType<MonsterController>();

        if (dialogueSystem == null)
            dialogueSystem = FindObjectOfType<DialogueSystem>();

        SetAct(GameAct.Act1);
    }

    public void AdvanceToNextAct()
    {
        switch (currentAct)
        {
            case GameAct.Act1:
                SetAct(GameAct.Act2);
                break;
            case GameAct.Act2:
                SetAct(GameAct.Act3);
                break;
            case GameAct.Act3:
                SetAct(GameAct.Act4);
                break;
            case GameAct.Act4:
                Debug.Log("[ActManager] Already in final act.");
                break;
        }
    }

    public void SetAct(GameAct newAct)
    {
        currentAct = newAct;
        Debug.Log($"[ActManager] Переход в {currentAct}");

        if (monsterController != null)
            monsterController.SetAct((MonsterController.Act)newAct);

        switch (currentAct)
        {
            case GameAct.Act1:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought("Нужно найти ключ...", 3f);
                break;

            case GameAct.Act2:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act2Message, 4f);
                break;

            case GameAct.Act3:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act3Message, 4f);
                break;

            case GameAct.Act4:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act4Message, 5f);
                if (monsterController != null)
                    monsterController.StartFinalChase();
                break;
        }
    }
}