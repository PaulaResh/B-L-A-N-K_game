using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ActManager.Instance?.monsterController != null)
                ActManager.Instance.monsterController.EnterSafeZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ActManager.Instance?.monsterController != null)
                ActManager.Instance.monsterController.ExitSafeZone();
        }
    }
}