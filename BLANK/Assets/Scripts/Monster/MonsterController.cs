using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum Act { Act1, Act2, Act3, Act4 }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Act 2 - Appearance")]
    public Transform[] act2SpawnPoints;
    public float appearDistance = 12f;

    [Header("Act 3 - Hunt Settings")]
    public float detectionRange = 18f;
    public float chaseSpeed = 5.5f;
    public float normalSpeed = 2.8f;
    public float roomStopDistance = 4f;

    [Header("Act 4 - Final Chase")]
    public float finalChaseSpeed = 6.5f;

    [Header("Attack")]
    public float attackDistance = 2.2f;
    public string screamSound = "Scream";

    private Act currentAct = Act.Act1;
    private bool isChasing = false;
    private bool finalChaseActive = false;
    private Vector3 lastKnownPlayerPosition;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("[Monster] NavMeshAgent component is missing!");
            enabled = false;
            return;
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        gameObject.SetActive(false); // Start hidden in Act 1
    }

    public void SetAct(Act newAct)
    {
        currentAct = newAct;
        isChasing = false;
        finalChaseActive = false;

        switch (currentAct)
        {
            case Act.Act1:
                gameObject.SetActive(false);
                break;

            case Act.Act2:
                gameObject.SetActive(true);
                AppearInAct2();
                break;

            case Act.Act3:
                gameObject.SetActive(true);
                agent.speed = normalSpeed;
                isChasing = false;
                break;

            case Act.Act4:
                gameObject.SetActive(true);
                StartFinalChase();
                break;
        }
    }

    private void AppearInAct2()
    {
        if (act2SpawnPoints == null || act2SpawnPoints.Length == 0 || player == null) return;

        // Find a spawn point far enough from player
        Transform bestPoint = act2SpawnPoints[0];
        float bestDist = 0;

        foreach (Transform point in act2SpawnPoints)
        {
            float dist = Vector3.Distance(point.position, player.position);
            if (dist > bestDist && dist > appearDistance)
            {
                bestDist = dist;
                bestPoint = point;
            }
        }

        transform.position = bestPoint.position;
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);

        if (animator != null)
            animator.SetTrigger("Appear");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("MonsterAppear");

        Debug.Log("[Monster] Появился в Акте 2");
    }

    public void StartFinalChase()
    {
        finalChaseActive = true;
        isChasing = true;
        agent.speed = finalChaseSpeed;
        agent.isStopped = false;

        if (player != null)
            agent.SetDestination(player.position);

        Debug.Log("[Monster] ФИНАЛЬНАЯ ПОГОНЯ НАЧАЛАСЬ!");
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        switch (currentAct)
        {
            case Act.Act2:
                // Monster just stands or slowly moves toward player but doesn't attack
                if (Vector3.Distance(transform.position, player.position) > 8f)
                    agent.SetDestination(player.position);
                break;

            case Act.Act3:
                HandleAct3Hunt();
                break;

            case Act.Act4:
                if (finalChaseActive && player != null)
                {
                    agent.SetDestination(player.position);
                    CheckAttack();
                }
                break;
        }
    }

    private void HandleAct3Hunt()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Simple vision check
        bool canSeePlayer = distance < detectionRange;

        if (canSeePlayer && !isChasing)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("MonsterChase");
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);
            CheckAttack();

            // Stop if player enters "room" (you can improve with trigger zones)
            if (distance < roomStopDistance)
            {
                // Optional: stop chasing when player is in safe room
                // agent.isStopped = true;
            }
        }
    }

    private void CheckAttack()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < attackDistance)
        {
            // Player caught - Scream + respawn near elevator
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound(screamSound);

            if (animator != null)
                animator.SetTrigger("Attack");

            Debug.Log("[Monster] Игрок пойман! Скример + респаун у лифта.");

            // Respawn player near elevator (you need to set up a respawn point)
            PlayerSafety.RespawnPlayerNearElevator();

            isChasing = false;
            agent.isStopped = true;

            // After scream, continue chase in Act 3 or keep chasing in Act 4
            Invoke(nameof(ResumeChase), 2.5f);
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

    // Called by trigger zones in the scene (you can add TriggerZone script)
    public void OnPlayerEnteredRoom()
    {
        if (currentAct == Act.Act3 && isChasing)
        {
            agent.isStopped = true;
            isChasing = false;
        }
    }

    public void OnPlayerExitedRoom()
    {
        if (currentAct == Act.Act3)
        {
            isChasing = true;
            agent.isStopped = false;
        }
    }
}