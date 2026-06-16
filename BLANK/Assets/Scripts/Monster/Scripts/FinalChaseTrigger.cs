using UnityEngine;

public class FinalChaseTrigger : MonoBehaviour
{
    [Header("Точка спавна монстра в Act 4")]
    public Transform monsterSpawnPoint;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        var actManager = ActManager.Instance;
        if (actManager != null && actManager.currentAct == ActManager.GameAct.Act4)
        {
            alreadyTriggered = true;

            if (actManager.monsterController != null && monsterSpawnPoint != null)
            {
                actManager.monsterController.StartAct4Chase(monsterSpawnPoint);
                Debug.Log("[FinalChaseTrigger] Act 4 — Монстр успешно запущен!");
            }
            else
            {
                Debug.LogError("[FinalChaseTrigger] MonsterController или spawnPoint не назначен!");
            }
        }
    }

    public void ResetTrigger()
    {
        alreadyTriggered = false;
        Debug.Log("[FinalChaseTrigger] Триггер сброшен");
    }
}