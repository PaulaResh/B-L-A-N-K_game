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

        if (ActManager.Instance != null && ActManager.Instance.currentAct == ActManager.GameAct.Act4)
        {
            alreadyTriggered = true;

            if (ActManager.Instance.monsterController != null)
            {
                ActManager.Instance.monsterController.StartAct4Chase(monsterSpawnPoint);
            }
            else
            {
                Debug.LogError("[FinalChaseTrigger] MonsterController не найден!");
            }
        }
    }
}