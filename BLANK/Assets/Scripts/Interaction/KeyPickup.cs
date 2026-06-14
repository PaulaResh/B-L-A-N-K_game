using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [Header("Key")]
    public ItemData keyData;

    [Header("Audio")]
    public string pickupSound = "Pickup";

    private bool isPickedUp = false;

    public void Interact()
    {
        if (isPickedUp) return;

        if (SimpleInventory.Instance == null) return;

        if (SimpleInventory.Instance.PickUpKey(keyData))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound(pickupSound);

            isPickedUp = true;
            gameObject.SetActive(false);
        }
    }

    public string GetInteractionPrompt()
    {
        return $"Подобрать ключ ({keyData.displayName})";
    }

    public bool CanInteract()
    {
        return !isPickedUp;
    }
}\