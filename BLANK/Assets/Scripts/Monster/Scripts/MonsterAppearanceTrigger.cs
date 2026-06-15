using UnityEngine;

public class MonsterAppearanceTrigger : MonoBehaviour
{
    [Header("Точки для появления")]
    public Transform spawnPoint;   // Где появляется
    public Transform targetPoint;  // Куда бежит

    [Tooltip("Скорость бега во время испуга")]
    public float scareSpeed = 7.8f;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        ActManager actManager = ActManager.Instance;
        if (actManager != null && actManager.currentAct == ActManager.GameAct.Act2)
        {
            alreadyTriggered = true;

            if (actManager.monsterController != null)
            {
                actManager.monsterController.ScareAppearAndRun(spawnPoint, targetPoint, scareSpeed);
            }
            else
            {
                Debug.LogError("[Trigger] MonsterController не найден!");
            }
        }
    }
}