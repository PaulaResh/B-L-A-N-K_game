using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public static HeldItemManager Instance { get; private set; }

    [Header("Holder (пустой объект в руке игрока)")]
    public Transform heldItemHolder;

    private string currentItemName = "";
    private GameObject currentHeldVisual;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PickUpItem(string itemName, GameObject worldObject)
    {
        currentItemName = itemName;

        // Удаляем предыдущий визуал, если был
        if (currentHeldVisual != null)
            Destroy(currentHeldVisual);

        // Создаём визуальную копию предмета в руке
        if (heldItemHolder != null && worldObject != null)
        {
            currentHeldVisual = Instantiate(worldObject, heldItemHolder);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
            currentHeldVisual.transform.localScale = Vector3.one * 0.6f;

            // Отключаем коллайдер и скрипты на копии в руке
            Collider col = currentHeldVisual.GetComponent<Collider>();
            if (col) col.enabled = false;

            MonoBehaviour[] scripts = currentHeldVisual.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script is PickupItem || script is IInteractable)
                    script.enabled = false;
            }
        }

        Debug.Log($"[HeldItemManager] Подобран предмет: {itemName}");
    }

    public bool HasItem(string itemName)
    {
        return currentItemName == itemName;
    }

    public void UseCurrentItem()
    {
        if (currentHeldVisual != null)
        {
            Destroy(currentHeldVisual);
            currentHeldVisual = null;
        }
        currentItemName = "";
    }

    public void DropCurrentItem()
    {
        UseCurrentItem();
    }
}