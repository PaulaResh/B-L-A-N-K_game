using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    public ItemData itemData;

    [Header("Audio")]
    public string pickupSound = "Pickup";

    private bool isPickedUp = false;

    public void Interact()
    {
        if (isPickedUp) return;

        SimpleInventory.Instance?.PickUpItem(itemData);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(pickupSound);

        isPickedUp = true;
        gameObject.SetActive(false);

        Debug.Log($"[PickupItem] Подобран: {itemData.displayName}");
    }

    public string GetInteractionPrompt()
    {
        return $"Подобрать ({itemData.displayName})";
    }

    public bool CanInteract()
    {
        return !isPickedUp;
    }
}