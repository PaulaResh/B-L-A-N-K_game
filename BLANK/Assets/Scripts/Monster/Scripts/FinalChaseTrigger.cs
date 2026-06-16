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

            if (actManager.monsterController != null)
            {
                actManager.monsterController.StartAct4Chase(monsterSpawnPoint);
                Debug.Log("[FinalChaseTrigger] Монстр появился в Act 4");
            }
        }
    }

    // Этот метод будет вызываться при смерти игрока
    public void ResetTrigger()
    {
        alreadyTriggered = false;
        Debug.Log("[FinalChaseTrigger] Триггер сброшен — монстр может появиться снова");
    }
}