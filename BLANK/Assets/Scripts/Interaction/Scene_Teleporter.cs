using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public string sceneToLoad;
    public string spawnPointName = "SpawnPoint"; // имя объекта в следующей сцене

    private bool isActive = false;

    private void Start()
    {
        // По умолчанию телепорт выключен
        GetComponent<Collider>().enabled = false;
        // Можно также выключить MeshRenderer, если есть визуал
    }

    public void Activate()
    {
        isActive = true;
        GetComponent<Collider>().enabled = true;
        Debug.Log("[Teleporter] Телепорт активирован!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        // Сохраняем, куда игрок должен появиться в следующей сцене
        PlayerSpawnManager.nextSpawnPointName = spawnPointName;

        SceneManager.LoadScene(sceneToLoad);
    }
}