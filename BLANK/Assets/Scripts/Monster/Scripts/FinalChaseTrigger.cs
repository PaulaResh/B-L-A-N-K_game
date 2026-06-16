using UnityEngine;

public class FinalChaseTrigger : MonoBehaviour
{
    [Header("Точка спавна монстра в Act 4")]
    public Transform monsterSpawnPoint;

    [Tooltip("Сколько раз можно активировать триггер за уровень")]
    public int maxTriggers = 999;

    private int triggerCount = 0;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerCount >= maxTriggers) return;
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        var actManager = ActManager.Instance;
        if (actManager == null || actManager.currentAct != ActManager.GameAct.Act4)
            return;

        if (monsterSpawnPoint == null)
        {
            Debug.LogError("[FinalChaseTrigger] monsterSpawnPoint не назначен!");
            return;
        }

        if (actManager.monsterController == null)
        {
            Debug.LogError("[FinalChaseTrigger] MonsterController не найден в ActManager!");
            return;
        }

        alreadyTriggered = true;
        triggerCount++;

        actManager.monsterController.StartAct4Chase(monsterSpawnPoint);
        Debug.Log($"[FinalChaseTrigger] Act 4 — Монстр успешно спавнится в {monsterSpawnPoint.name}");
    }

    public void ResetTrigger()
    {
        alreadyTriggered = false;
        Debug.Log("[FinalChaseTrigger] Триггер сброшен — можно активировать снова");
    }

    // Для отладки
    private void OnDrawGizmosSelected()
    {
        if (monsterSpawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(monsterSpawnPoint.position, 0.4f);
            Gizmos.DrawLine(transform.position, monsterSpawnPoint.position);
        }
    }
}