using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Header("Panel")]
    public string interactionPrompt = "Взаимодействовать";

    [Header("Items")]
    public ItemData requiredItem1;  // рубильник
    public ItemData requiredItem2;  // предохранитель

    [Header("Teleport")]
    public GameObject teleportZone;

    [SerializeField] private int currentStage = 0; // 0 = ничего, 1 = первый, 2 = оба

    public void Interact()
    {
        bool advanced = false;

        // Этап 1: первый предмет
        if (currentStage == 0 && requiredItem1 != null)
        {
            if (SimpleInventory.Instance?.UseItem(requiredItem1.itemName) == true)
            {
                currentStage = 1;
                Debug.Log($"[ElectricalPanel] Установлен: {requiredItem1.displayName}");

                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought($"{requiredItem1.displayName} на месте...", 2.5f);

                advanced = true;
            }
        }
        // Этап 2: второй предмет
        else if (currentStage == 1 && requiredItem2 != null)
        {
            if (SimpleInventory.Instance?.UseItem(requiredItem2.itemName) == true)
            {
                currentStage = 2;
                Debug.Log($"[ElectricalPanel] Установлен: {requiredItem2.displayName}");

                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought($"{requiredItem2.displayName} установлен...", 2.5f);

                // Активируем телепорт
                if (teleportZone != null)
                    teleportZone.SetActive(true);

                advanced = true;
            }
        }

        if (advanced && ActManager.Instance != null)
            ActManager.Instance.AdvanceToNextAct();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("PanelClick");
    }

    public string GetInteractionPrompt()
    {
        if (currentStage == 0 && requiredItem1 != null)
            return $"Установить {requiredItem1.displayName}";
        if (currentStage == 1 && requiredItem2 != null)
            return $"Установить {requiredItem2.displayName}";
        return "Щиток активирован";
    }

    public bool CanInteract()
    {
        return currentStage < 2;
    }
}