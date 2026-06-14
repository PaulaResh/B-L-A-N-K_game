using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance { get; private set; }

    [Header("Settings")]
    public Transform heldItemHolder;  // пустой объект перед камерой

    [Header("Input")]
    public KeyCode nextItemKey = KeyCode.E;
    public KeyCode prevItemKey = KeyCode.Q;

    private ItemData currentItem = null;
    private ItemData nextItem = null;  // буфер для переключения

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
        if (Input.GetKeyDown(nextItemKey))
            SwitchToNextItem();

        if (Input.GetKeyDown(prevItemKey))
            SwitchToEmptyHand();
    }

    /// <summary>
    /// Подобрал новый предмет
    /// </summary>
    public void PickUpItem(ItemData item)
    {
        if (item == null) return;

        nextItem = item;

        if (currentItem == null)
        {
            // Рука была пуста — сразу переключаемся
            SwitchToNextItem();
        }
        else
        {
            Debug.Log($"[Inventory] Сохранён буфер: {item.displayName} (нажми E чтобы переключиться)");
        }
    }

    public void SwitchToNextItem()
    {
        if (nextItem == null)
        {
            Debug.Log("[Inventory] Нет следующего предмета");
            return;
        }

        currentItem = nextItem;
        nextItem = null;
        UpdateVisual();
        Debug.Log($"[Inventory] Переключились на: {currentItem.displayName}");
    }

    public void SwitchToEmptyHand()
    {
        currentItem = null;
        UpdateVisual();
        Debug.Log("[Inventory] Пустая рука");
    }

    public ItemData GetCurrentItem()
    {
        return currentItem;
    }

    public bool HasItem(string itemName)
    {
        return currentItem != null && currentItem.itemName == itemName;
    }

    /// <summary>
    /// Использовать текущий предмет
    /// </summary>
    public bool UseItem(string requiredItemName)
    {
        if (currentItem == null)
        {
            Debug.Log("[Inventory] Рука пуста, нельзя использовать");
            return false;
        }

        if (currentItem.itemName != requiredItemName)
        {
            Debug.Log($"[Inventory] Попытка использовать '{requiredItemName}', но в руке '{currentItem.itemName}'");
            return false;
        }

        Debug.Log($"[Inventory] Использован: {currentItem.displayName}");

        if (currentItem.isConsumable)
        {
            currentItem = null;
            nextItem = null;
            UpdateVisual();
        }

        return true;
    }

    private void UpdateVisual()
    {
        // Удаляем старый визуал
        if (currentVisual != null)
        {
            DestroyImmediate(currentVisual);
            currentVisual = null;
        }

        // Если нет предмета — выходим
        if (currentItem == null || currentItem.visualPrefab == null)
            return;

        if (heldItemHolder == null)
        {
            Debug.LogWarning("[Inventory] heldItemHolder не назначен!");
            return;
        }

        // Создаём визуал
        currentVisual = Instantiate(currentItem.visualPrefab, heldItemHolder);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;
        currentVisual.transform.localScale = Vector3.one;

        // Отключаем коллайдеры и скрипты
        foreach (var collider in currentVisual.GetComponentsInChildren<Collider>())
            collider.enabled = false;

        foreach (var script in currentVisual.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false;

        Debug.Log($"[Inventory] Визуал: {currentItem.displayName}");
    }
}