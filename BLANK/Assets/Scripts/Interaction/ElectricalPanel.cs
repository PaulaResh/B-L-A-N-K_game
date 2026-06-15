using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Header("Electrical Panel Settings")]
    public string interactionPrompt = "Взаимодействовать со щитком";

    [Header("Required Items")]
    public string requiredItemForAct2 = "Switch";
    public string requiredItemForAct3 = "Fuse";

    [Header("Current State")]
    [SerializeField] private int currentStage = 0;

    private bool isCompleted = false;

    public void Interact()
    {
        if (isCompleted) return;

        if (HeldItemManager.Instance == null)
        {
            Debug.LogWarning("[ElectricalPanel] HeldItemManager не найден!");
            return;
        }

        string requiredItem = ActManager.Instance.GetRequiredItemForCurrentAct();

        if (HeldItemManager.Instance.HasItem(requiredItem))
        {
            HeldItemManager.Instance.DropCurrentItem();
            currentStage++;
            isCompleted = true;

            Debug.Log($"[ElectricalPanel] {requiredItem} установлен.");

            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought("Теперь можно идти дальше...", 2.5f);

            // Активируем телепорт
            SceneTeleporter teleporter = FindFirstObjectByType<SceneTeleporter>();
            if (teleporter != null)
                teleporter.Activate();

            if (ActManager.Instance != null)
                ActManager.Instance.AdvanceToNextAct();
        }
        else
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought($"Нужен {requiredItem}.", 2f);
        }
    }

    public string GetInteractionPrompt()
    {
        if (isCompleted) return "Щиток активирован";
        return interactionPrompt;
    }

    public bool CanInteract()
    {
        return !isCompleted;
    }
}