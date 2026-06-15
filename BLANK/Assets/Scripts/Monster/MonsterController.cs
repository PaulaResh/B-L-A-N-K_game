using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum Act { Act1, Act2, Act3, Act4 }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Chase Settings")]
    public float chaseSpeed = 5.5f;
    public float attackDistance = 2.2f;
    public string screamSound = "Scream";

    private bool isChasing = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
        }

        Debug.Log("[Monster] === АГРЕССИВНЫЙ РЕЖИМ ВКЛЮЧЁН ===");
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        if (isChasing)
        {
            agent.SetDestination(player.position);

            // Проверка атаки
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < attackDistance)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound(screamSound);

                if (animator != null)
                    animator.SetTrigger("Attack");

                Debug.Log("[Monster] Игрок пойман!");

                //PlayerSafety.RespawnPlayerNearElevator();

                isChasing = false;
                agent.isStopped = true;

                Invoke(nameof(ResumeChase), 2.5f);
            }
        }
    }

    private void ResumeChase()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            isChasing = true;
        }
    }

    // ==================== МЕТОДЫ ДЛЯ СОВМЕСТИМОСТИ С ActManager ====================
    public void SetAct(Act newAct)
    {
        // Пока ничего не делаем (агрессивный режим)
    }

    public void StartFinalChase()
    {
        isChasing = true;
        if (agent != null)
            agent.speed = 6.5f;
    }

    public void AppearInAct2() { }
    public void OnPlayerEnteredRoom() { }
    public void OnPlayerExitedRoom() { }
    // =================================================================================
}