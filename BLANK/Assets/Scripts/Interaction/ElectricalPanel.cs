using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Header("Настройки")]
    public string interactionPrompt = "Взаимодействовать со щитком";

    [SerializeField] private bool isCompleted = false;

    public void Interact()
    {
        if (isCompleted) return;

        if (HeldItemManager.Instance == null || ActManager.Instance == null)
        {
            Debug.LogWarning("[ElectricalPanel] Менеджеры не найдены!");
            return;
        }

        string requiredItem = ActManager.Instance.GetRequiredItemForCurrentAct();

        if (HeldItemManager.Instance.HasItem(requiredItem))
        {
            HeldItemManager.Instance.UseCurrentItem();

            isCompleted = true;

            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought("Теперь можно идти дальше...", 2.5f);

            // Активируем телепорт
            SceneTeleporter teleporter = FindFirstObjectByType<SceneTeleporter>();
            if (teleporter != null)
                teleporter.Activate();

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
        return isCompleted ? "Щиток активирован" : interactionPrompt;
    }

    public bool CanInteract()
    {
        return !isCompleted;
    }
}