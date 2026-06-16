using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSafety : MonoBehaviour
{
    public static PlayerSafety Instance { get; private set; }

    [Header("Respawn Points")]
    public Transform act3RespawnPoint;
    public Transform act4RespawnPoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void KillPlayer()
    {
        RespawnPlayerAndResetMonster();
    }

    private void RespawnPlayerAndResetMonster()
    {
        Debug.Log($"[PlayerSafety] === СМЕРТЬ === Акт: {ActManager.Instance?.currentAct}");

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("[PlayerSafety] Игрок не найден по тегу Player!");
            return;
        }

        Transform respawnPoint = null;
        if (ActManager.Instance != null)
        {
            if (ActManager.Instance.currentAct == ActManager.GameAct.Act4)
                respawnPoint = act4RespawnPoint;
            else if (ActManager.Instance.currentAct == ActManager.GameAct.Act3)
                respawnPoint = act3RespawnPoint;
        }

        if (respawnPoint != null)
        {
            // === Улучшенный респавн ===
            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerObj.transform.position = respawnPoint.position;
            playerObj.transform.rotation = respawnPoint.rotation;

            if (cc != null) cc.enabled = true;

            Debug.Log($"[PlayerSafety] УСПЕШНЫЙ РЕСПАВН на точку: {respawnPoint.name}");
        }
        else
        {
            Debug.LogError("[PlayerSafety] Respawn point НЕ НАЗНАЧЕН для текущего акта!");
        }

        // === Респавн/сброс монстра ===
        if (ActManager.Instance?.monsterController != null)
        {
            var monster = ActManager.Instance.monsterController;

            if (ActManager.Instance.currentAct == ActManager.GameAct.Act4)
            {
                monster.gameObject.SetActive(false);
                var trigger = FindObjectOfType<FinalChaseTrigger>();
                if (trigger != null) trigger.ResetTrigger();
                Debug.Log("[PlayerSafety] Act 4 — монстр убран и триггер сброшен");
            }
            else if (ActManager.Instance.currentAct == ActManager.GameAct.Act3)
            {
                monster.gameObject.SetActive(true);
                monster.StartChase();
                Debug.Log("[PlayerSafety] Act 3 — монстр перезапущен");
            }
        }
    }
}