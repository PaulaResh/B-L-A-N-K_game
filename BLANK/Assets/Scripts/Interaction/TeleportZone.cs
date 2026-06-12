using UnityEngine;

/// <summary>
/// Триггер-зона: при входе игрока телепортирует его в destination.
/// Объект с этим скриптом обычно выключен (SetActive(false)) и
/// включается извне (например, ElectricalPanel после использования предмета).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TeleportZone : MonoBehaviour
{
    [Header("Destination")]
    public Transform destination;

    [Header("Settings")]
    public bool oneShot = true; // деактивировать зону после первого срабатывания

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (destination == null)
        {
            Debug.LogWarning("[TeleportZone] Destination не назначен!");
            return;
        }

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        other.transform.position = destination.position;
        other.transform.rotation = destination.rotation;

        if (cc != null) cc.enabled = true;

        Debug.Log("[TeleportZone] Игрок телепортирован в " + destination.name);

        if (oneShot)
            gameObject.SetActive(false);
    }
}