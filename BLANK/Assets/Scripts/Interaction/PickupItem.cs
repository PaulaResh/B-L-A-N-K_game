using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    public string itemName = "Key"; // "Key", "Switch", "Fuse"
    public string interactionPrompt = "Подобрать";

    private bool isPickedUp = false;

    public void Interact()
{
    if (isPickedUp) return;

    if (HeldItemManager.Instance != null)
    {
        HeldItemManager.Instance.PickUpItem(itemName, gameObject);
    }

    if (Inventory.Instance != null)
    {
        Inventory.Instance.AddItem(itemName);
    }

    isPickedUp = true;
    gameObject.SetActive(false); // Прячем объект в мире

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySound("Pickup");
}

    public string GetInteractionPrompt()
    {
        return interactionPrompt + " (" + itemName + ")";
    }

    public bool CanInteract()
    {
        return !isPickedUp;
    }
}