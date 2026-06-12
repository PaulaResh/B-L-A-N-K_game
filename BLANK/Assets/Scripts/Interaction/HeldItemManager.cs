using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public static HeldItemManager Instance { get; private set; }

    [Header("Holder")]
    public Transform heldItemHolder;           // Пустой объект перед камерой

    [Header("Current State")]
    public string currentHeldItem = "";        // Название предмета в руке

    private GameObject currentHeldVisual;      // Текущая модель в руке

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Подбирает предмет и показывает его в руке
    /// </summary>
    public void PickUpItem(string itemName, GameObject worldObject = null)
    {
        // Убираем предыдущий предмет, если был
        DropCurrentItem();

        currentHeldItem = itemName;

        // Если передали объект из мира — создаём его копию в руке
        if (worldObject != null)
        {
            currentHeldVisual = Instantiate(worldObject, heldItemHolder);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
            currentHeldVisual.transform.localScale = Vector3.one * 0.6f; // Можно подкрутить размер

            // Отключаем коллайдер и скрипты, чтобы не мешали
            Collider col = currentHeldVisual.GetComponent<Collider>();
            if (col) col.enabled = false;

            // Отключаем все скрипты на визуальной копии
            MonoBehaviour[] scripts = currentHeldVisual.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != null) script.enabled = false;
            }
        }

        Debug.Log($"[HeldItem] В руке: {itemName}");
    }

    /// <summary>
    /// Убирает текущий предмет из руки
    /// </summary>
    public void DropCurrentItem()
    {
        if (currentHeldVisual != null)
        {
            Destroy(currentHeldVisual);
            currentHeldVisual = null;
        }
        currentHeldItem = "";
    }

    public bool HasItem(string itemName)
    {
        return currentHeldItem == itemName;
    }

    /// <summary>
    /// Использует предмет (например, на щитке) и убирает его
    /// </summary>
    public bool UseCurrentItem(string requiredItem)
    {
        if (currentHeldItem == requiredItem)
        {
            DropCurrentItem();
            return true;
        }
        return false;
    }
}