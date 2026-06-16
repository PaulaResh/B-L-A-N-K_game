using UnityEngine;

public class ActManager : MonoBehaviour
{
    public static ActManager Instance { get; private set; }

    public enum GameAct { Act1, Act2, Act3, Act4 }

    [Header("=== Текущий Акт ===")]
    [Tooltip("Поменяй здесь акт — он сразу применится")]
    public GameAct currentAct = GameAct.Act1;

    [Header("References")]
    public MonsterController monsterController;
    public DialogueSystem dialogueSystem;

    [Header("Act Messages")]
    public string act2Message = "Кажется, кто-то появился...";
    public string act3Message = "Он начал охоту...";
    public string act4Message = "Беги. Не останавливайся.";

    private GameAct previousAct;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (monsterController == null)
            monsterController = FindFirstObjectByType<MonsterController>(FindObjectsInactive.Include);

        if (dialogueSystem == null)
            dialogueSystem = FindFirstObjectByType<DialogueSystem>();

        previousAct = currentAct;
        ApplyAct();
    }

    private void OnValidate()
    {
        if (Instance == null || !Application.isPlaying)
        {
            previousAct = currentAct;
            return;
        }

        if (previousAct != currentAct)
        {
            previousAct = currentAct;
            ApplyAct();
        }
    }

    private void ApplyAct()
    {
        Debug.Log($"[ActManager] === ПЕРЕХОД В {currentAct} ===");

        if (monsterController == null)
        {
            Debug.LogWarning("[ActManager] MonsterController не найден!");
            return;
        }

        // Сначала выключаем монстра
        monsterController.gameObject.SetActive(false);
        monsterController.SetState(MonsterController.MonsterState.Disabled);

        switch (currentAct)
        {
            case GameAct.Act1:
                Debug.Log("[ActManager] Act 1 — монстр выключен");
                break;

            case GameAct.Act2:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act2Message, 4f);
                Debug.Log("[ActManager] Act 2 — монстр выключен (ждёт триггер)");
                break;

            case GameAct.Act3:
                Debug.Log("[ActManager] Act 3 — ЗАПУСК ПАТРУЛЯ");
                monsterController.StartChase();
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act3Message, 4f);
                break;

            case GameAct.Act4:
                if (dialogueSystem != null)
                    dialogueSystem.ShowThought(act4Message, 5f);
                Debug.Log("[ActManager] Act 4 — монстр выключен (ждёт FinalChaseTrigger)");
                break;
        }
    }

    // ← Этот метод был нужен для ElectricalPanel
    public string GetRequiredItemForCurrentAct()
    {
        switch (currentAct)
        {
            case GameAct.Act1: return "Key";
            case GameAct.Act2: return "Fuse";
            case GameAct.Act3: return "Switch";
            default: return "";
        }
    }

    public void AdvanceToNextAct()
    {
        switch (currentAct)
        {
            case GameAct.Act1: currentAct = GameAct.Act2; break;
            case GameAct.Act2: currentAct = GameAct.Act3; break;
            case GameAct.Act3: currentAct = GameAct.Act4; break;
            case GameAct.Act4:
                Debug.Log("Уже в финальном акте");
                return;
        }
        ApplyAct();
    }

    public void TriggerMonsterAppearance(Transform spawnPoint, Transform targetPoint)
    {
        if (currentAct == GameAct.Act2 && monsterController != null)
            monsterController.AppearAndMoveTo(spawnPoint, targetPoint);
    }

    public void TriggerFinalChase(Transform spawnPoint)
    {
        if (currentAct == GameAct.Act4 && monsterController != null)
            monsterController.StartAct4Chase(spawnPoint);
    }
}