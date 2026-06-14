using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        if (!string.IsNullOrEmpty(PlayerSpawnManager.nextSpawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(PlayerSpawnManager.nextSpawnPointName);

            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;
                Debug.Log($"[PlayerSpawner] Игрок появился на точке: {PlayerSpawnManager.nextSpawnPointName}");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] Точка спавна '{PlayerSpawnManager.nextSpawnPointName}' не найдена!");
            }

            PlayerSpawnManager.nextSpawnPointName = "";
        }
    }
}