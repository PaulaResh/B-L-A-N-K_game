using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Header("Electrical Panel Settings")]
    public string interactionPrompt = "Взаимодействовать со щитком";

    [Header("Required Items")]
    public string requiredItemForAct2 = "Switch";   // Рубильник
    public string requiredItemForAct3 = "Fuse";     // Предохранитель

    [Header("Current State")]
    [SerializeField] private int currentStage = 0; // 0 = ничего, 1 = рубильник, 2 = предохранитель

    public void Interact()
    {
        if (HeldItemManager.Instance == null)
        {
            Debug.LogWarning("[ElectricalPanel] HeldItemManager не найден!");
            return;
        }

        bool advanced = false;

        // === Этап 1: Установка рубильника ===
        if (currentStage == 0)
        {
            if (HeldItemManager.Instance.HasItem(requiredItemForAct2))
            {
                HeldItemManager.Instance.DropCurrentItem(); // Убираем из руки
                currentStage = 1;
                Debug.Log("[ElectricalPanel] Рубильник установлен.");

                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought("Рубильник на месте...", 2.5f);

                advanced = true;
            }
            else
            {
                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought("Нужен рубильник.", 2f);
            }
        }
        // === Этап 2: Установка предохранителя ===
        else if (currentStage == 1)
        {
            if (HeldItemManager.Instance.HasItem(requiredItemForAct3))
            {
                HeldItemManager.Instance.DropCurrentItem(); // Убираем из руки
                currentStage = 2;
                Debug.Log("[ElectricalPanel] Предохранитель установлен.");

                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought("Теперь всё работает...", 2.5f);

                advanced = true;
            }
            else
            {
                if (DialogueSystem.Instance != null)
                    DialogueSystem.Instance.ShowThought("Нужен предохранитель.", 2f);
            }
        }
        else
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought("Щиток уже активирован.", 2f);
        }

        // Переход между актами
        if (advanced && ActManager.Instance != null)
        {
            ActManager.Instance.AdvanceToNextAct();
        }

        // Звук
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