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

        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(itemName);
        }

        isPickedUp = true;
        gameObject.SetActive(false); // or destroy / put in inventory visually

        // Optional: play pickup sound via AudioManager
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Pickup");

        Debug.Log($"[Pickup] Picked up: {itemName}");
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