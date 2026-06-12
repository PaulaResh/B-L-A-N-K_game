using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string displayName;

    [Header("Visual")]
    public GameObject visualPrefab;

    [Header("Settings")]
    public bool isConsumable = true;

    [Header("Key Settings")]
    public bool isKey = false;           // true, если это ключ
    public string targetDoorId = "";     // ID двери, которую открывает
}