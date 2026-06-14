using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("Inventory Settings")]
    public List<string> items = new List<string>(); // e.g. "Key", "Switch", "Fuse"
    public string currentHeldItem = "";

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

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName))
        {
            items.Add(itemName);
            currentHeldItem = itemName;
            Debug.Log($"[Inventory] Added: {itemName}");
        }
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            if (currentHeldItem == itemName)
                currentHeldItem = items.Count > 0 ? items[0] : "";
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        currentHeldItem = "";
    }
}