using UnityEngine;

public static class PlayerSafety
{
    public static Transform elevatorRespawnPoint; // Assign in Inspector or via code

    public static void RespawnPlayerNearElevator()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (elevatorRespawnPoint != null)
        {
            player.transform.position = elevatorRespawnPoint.position;
            player.transform.rotation = elevatorRespawnPoint.rotation;
        }
        else
        {
            // Fallback: move player to origin or a default safe spot
            player.transform.position = new Vector3(0, 1, 0);
        }

        // Optional: reset velocity if using rigidbody
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }
}