using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemPrefab; // предмет в руке

    public void Interact()
    {
        Inventory.Instance.Add(itemPrefab);

        Destroy(gameObject); // убираем с карты
    }

    public bool CanInteract() => true;

    public string GetInteractionText() => "";
}