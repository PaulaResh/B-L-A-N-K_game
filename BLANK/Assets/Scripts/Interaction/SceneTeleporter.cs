using UnityEngine;

public class SceneTeleporter : MonoBehaviour
{
    [Header("Teleport Settings (внутри одной сцены)")]
    [Tooltip("Имя пустого объекта-точки спавна (например: Spawn_Act2)")]
    public string spawnPointName = "Spawn_Act2";

    [Header("Активация/деактивация карт")]
    [Tooltip("Какую карту включить после телепорта (перетащи Map2)")]
    public GameObject mapToActivate;

    [Tooltip("Какую карту выключить (перетащи текущую Map)")]
    public GameObject mapToDeactivate;

    [Header("Visual (опционально)")]
    public GameObject visualEffect;

    private bool isActive = false;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        if (visualEffect != null)
            visualEffect.SetActive(false);
    }

    public void Activate()
    {
        isActive = true;

        if (col != null)
            col.enabled = true;

        if (visualEffect != null)
            visualEffect.SetActive(true);

        Debug.Log("[SceneTeleporter] Телепорт активирован!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        TeleportPlayer(other.gameObject);

        // Включаем/выключаем карты
        if (mapToDeactivate != null)
            mapToDeactivate.SetActive(false);

        if (mapToActivate != null)
            mapToActivate.SetActive(true);

        Debug.Log($"[SceneTeleporter] Игрок телепортирован на точку: {spawnPointName}");
    }

    private void TeleportPlayer(GameObject player)
    {
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            // Сбрасываем скорость
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            Debug.Log("[SceneTeleporter] Игрок успешно перемещён.");
        }
        else
        {
            Debug.LogWarning($"[SceneTeleporter] Точка спавна '{spawnPointName}' не найдена!");
        }
    }
}