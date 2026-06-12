using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance { get; private set; }

    [Header("Settings")]
    public Transform heldItemHolder;

    [Header("Input")]
    public KeyCode nextItemKey = KeyCode.E;
    public KeyCode prevItemKey = KeyCode.Q;

    [Header("Key")]
    public ItemData heldKey;

    private ItemData currentItem = null;
    private ItemData nextItem = null;
    private GameObject currentVisual = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(nextItemKey)) SwitchToNextItem();
        if (Input.GetKeyDown(prevItemKey)) SwitchToEmptyHand();
    }

    public void PickUpItem(ItemData item)
    {
        if (item == null) return;
        nextItem = item;
        if (currentItem == null) SwitchToNextItem();
        else Debug.Log("[Inventory] Отложен предмет: " + item.displayName + " (нажми E)");
    }

    public bool PickUpKey(ItemData key)
    {
        if (key == null || !key.isKey) return false;
        if (heldKey != null)
        {
            if (DialogueSystem.Instance != null) 
                DialogueSystem.Instance.ShowThought("Больше одного ключа не унести...", 2f);
            return false;
        }
        heldKey = key;
        if (DialogueSystem.Instance != null) 
            DialogueSystem.Instance.ShowThought("Подобрал: " + key.displayName, 2f);
        return true;
    }

    public bool HasKeyFor(string doorId)
    {
        return heldKey != null && heldKey.targetDoorId == doorId;
    }

    public void ConsumeKey()
    {
        heldKey = null;
    }

    // === НОВЫЙ МЕТОД ДЛЯ ЩИТКА ===
    public bool UseItem(string requiredItemName)
    {
        if (currentItem == null)
        {
            Debug.Log("[Inventory] В руках пусто, нельзя использовать");
            return false;
        }

        if (currentItem.itemName != requiredItemName)
        {
            Debug.Log("[Inventory] Требуется '" + requiredItemName + "', но в руках '" + currentItem.itemName + "'");
            return false;
        }

        Debug.Log("[Inventory] Использован: " + currentItem.displayName);

        if (currentItem.isConsumable)
        {
            currentItem = null;
            nextItem = null;
            UpdateVisual();
        }

        return true;
    }
    // ============================

    public void SwitchToNextItem()
    {
        if (nextItem == null) return;
        currentItem = nextItem;
        nextItem = null;
        UpdateVisual();
    }

    public void SwitchToEmptyHand()
    {
        currentItem = null;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (currentVisual != null)
        {
            DestroyImmediate(currentVisual);
            currentVisual = null;
        }

        if (currentItem == null || currentItem.visualPrefab == null || heldItemHolder == null) return;

        currentVisual = Instantiate(currentItem.visualPrefab, heldItemHolder);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;
        currentVisual.transform.localScale = Vector3.one;

        foreach (var collider in currentVisual.GetComponentsInChildren<Collider>()) collider.enabled = false;
        foreach (var script in currentVisual.GetComponentsInChildren<MonoBehaviour>()) script.enabled = false;
    }
}