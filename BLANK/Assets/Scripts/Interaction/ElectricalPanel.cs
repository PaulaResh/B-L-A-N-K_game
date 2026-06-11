using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Header("Electrical Panel")]
    public string interactionPrompt = "Взаимодействовать со щитком";
    
    [Header("Required Items (in order)")]
    public string requiredItemForAct2 = "Switch";      // Рубильник
    public string requiredItemForAct3 = "Fuse";        // Предохранитель

    private int currentStage = 0; // 0 = nothing, 1 = switch installed, 2 = fuse installed

    public void Interact()
    {
        if (Inventory.Instance == null) return;

        bool advanced = false;

        if (currentStage == 0 && Inventory.Instance.HasItem(requiredItemForAct2))
        {
            Inventory.Instance.RemoveItem(requiredItemForAct2);
            currentStage = 1;
            Debug.Log("[ElectricalPanel] Рубильник установлен. Акт 2 → Акт 3");
            advanced = true;
        }
        else if (currentStage == 1 && Inventory.Instance.HasItem(requiredItemForAct3))
        {
            Inventory.Instance.RemoveItem(requiredItemForAct3);
            currentStage = 2;
            Debug.Log("[ElectricalPanel] Предохранитель установлен. Акт 3 → Акт 4");
            advanced = true;
        }
        else if (currentStage == 0)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought("Нужен рубильник.", 2f);
        }
        else if (currentStage == 1)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought("Нужен предохранитель.", 2f);
        }

        if (advanced && ActManager.Instance != null)
        {
            ActManager.Instance.AdvanceToNextAct();
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("PanelClick");
    }

    public string GetInteractionPrompt()
    {
        if (currentStage == 0) return "Установить рубильник";
        if (currentStage == 1) return "Установить предохранитель";
        return "Щиток активирован";
    }

    public bool CanInteract()
    {
        return currentStage < 2;
    }
}