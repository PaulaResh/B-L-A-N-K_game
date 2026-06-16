using UnityEngine;

public class MonsterAppearanceTrigger : MonoBehaviour
{
    [Header("Точки для появления")]
    public Transform spawnPoint;
    public Transform targetPoint;

    [Tooltip("Скорость бега во время испуга")]
    public float scareSpeed = 7.8f;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        var actManager = ActManager.Instance;
        if (actManager == null)
        {
            Debug.LogError("[MonsterAppearanceTrigger] ActManager.Instance == null!");
            return;
        }

        if (actManager.currentAct != ActManager.GameAct.Act2)
        {
            Debug.Log($"[Trigger] Игнорируем — текущий акт: {actManager.currentAct}");
            return;
        }

        if (actManager.monsterController == null)
        {
            Debug.LogError("[Trigger] MonsterController не найден в ActManager!");
            // Попытка найти вручную
            var monster = FindObjectOfType<MonsterController>();
            if (monster != null)
            {
                alreadyTriggered = true;
                monster.ScareAppearAndRun(spawnPoint, targetPoint, scareSpeed);
                Debug.Log("[Trigger] Монстр найден вручную");
            }
            return;
        }

        alreadyTriggered = true;
        actManager.monsterController.ScareAppearAndRun(spawnPoint, targetPoint, scareSpeed);
        Debug.Log("[Trigger] Act 2 — Jump Scare активирован!");
    }

    // Для отладки
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawnPoint.position, 0.3f);
        }
        if (targetPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPoint.position, 0.3f);
        }
    }

    // Если нужно сбросить триггер (например после смерти)
    public void ResetTrigger()
    {
        alreadyTriggered = false;
        Debug.Log("[Trigger] Триггер сброшен");
    }
}