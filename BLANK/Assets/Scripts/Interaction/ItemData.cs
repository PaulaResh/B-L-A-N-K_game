using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;           // "Key", "Switch", "Fuse"
    public string displayName;        // "Ключ", "Рубильник"

    [Header("Visual")]
    public GameObject visualPrefab;   // 3D-модель в руке

    [Header("Settings")]
    public bool isConsumable = true;  // пропадает после использования?
}
