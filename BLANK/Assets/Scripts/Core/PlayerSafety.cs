using UnityEngine;

public static class PlayerSafety
{
    public static Transform elevatorRespawnPoint;

    public static void RespawnPlayerNearElevator()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[PlayerSafety] Игрок не найден!");
            return;
        }

        if (elevatorRespawnPoint != null)
        {
            player.transform.position = elevatorRespawnPoint.position + Vector3.up * 0.5f;
            player.transform.rotation = elevatorRespawnPoint.rotation;
            Debug.Log("[PlayerSafety] Игрок респавнен у лифта");
        }
        else
        {
            Debug.LogWarning("[PlayerSafety] elevatorRespawnPoint не назначен! Респавн в (0,1,0)");
            player.transform.position = new Vector3(0, 1, 0);
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        // Можно добавить эффект (fade, звук и т.д.)
    }
}