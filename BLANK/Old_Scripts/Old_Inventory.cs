using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<GameObject> items = new List<GameObject>();
    private int index = -1;

    [SerializeField] private Transform handPoint;

    void Awake()
    {
        Instance = this;
    }

    public void Add(GameObject itemPrefab)
    {
        GameObject obj = Instantiate(itemPrefab, handPoint);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        obj.SetActive(true);

        items.Add(obj);

        if (index == -1)
            index = 0;

        UpdateItems();
    }

    public void Next()
    {
        if (items.Count == 0) return;

        index++;
        if (index >= items.Count)
            index = -1;

        UpdateItems();
    }

    public void Previous()
    {
        if (items.Count == 0) return;

        index--;
        if (index < -1)
            index = items.Count - 1;

        UpdateItems();
    }

    void UpdateItems()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].SetActive(false);

        if (index >= 0 && index < items.Count)
            items[index].SetActive(true);
    }
}