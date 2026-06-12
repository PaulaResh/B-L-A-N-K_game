using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum Act { Act1, Act2, Act3, Act4 }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Speeds")]
    public float patrolSpeed = 3.2f;
    public float chaseSpeed = 5.5f;
    public float finalChaseSpeed = 7.0f;

    [Header("Settings")]
    public float attackDistance = 2.1f;
    public string screamSound = "Scream";
    public string appearSound = "MonsterAppear";

    private Act currentAct = Act.Act1;
    private bool isActive = false;

    private MeshRenderer[] renderers;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.isStopped = true;
        }

        renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var r in renderers)
            r.enabled = false;
        Debug.Log("[Monster] Готов и выключен в Start()");
    }

    private void Update()
    {
        if (!isActive || player == null || agent == null) return;

        if (currentAct == Act.Act3 || currentAct == Act.Act4)
        {
            agent.SetDestination(player.position);

            if (Vector3.Distance(transform.position, player.position) <= attackDistance)
            {
                AttackPlayer();
            }
        }

        UpdateAnimator();
    }

    /// <summary>
    /// Обновляет параметры аниматора в зависимости от скорости агента.
    /// Используй Blend Tree (Idle -> Walk -> Run) по параметру "Speed".
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator == null) return;

        float currentSpeed = agent.isStopped ? 0f : agent.velocity.magnitude;

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsMoving", currentSpeed > 0.1f);
    }

    private void AttackPlayer()
    {
        AudioManager.Instance?.PlaySound(screamSound);
        animator?.SetTrigger("Attack");

        Debug.Log("[Monster] Игрок пойман!");
        PlayerSafety.RespawnPlayerNearElevator();

        if (currentAct != Act.Act4)
        {
            agent.isStopped = true;
            CancelInvoke(nameof(ResumeChase));
            Invoke(nameof(ResumeChase), 2.5f);
        }
    }

    private void ResumeChase()
    {
        if (agent != null) agent.isStopped = false;
    }

    public void SetAct(Act newAct)
    {
        currentAct = newAct;
        Debug.Log($"[Monster] === SetAct вызван: {newAct} ===");

        switch (newAct)
        {
            case Act.Act1:
                DeactivateMonster();
                break;
            case Act.Act2:
                ActivateAct2();
                break;
            case Act.Act3:
                ActivateAct3();
                break;
            case Act.Act4:
                ActivateAct4();
                break;
        }
    }

    private void DeactivateMonster()
    {
        isActive = false;
        foreach(var r in renderers)
            r.enabled = false;
        Debug.Log("[Monster] Деактивирован");
    }

    private void ActivateAct2()
    {
        gameObject.SetActive(true);
        isActive = true;
        foreach (var r in renderers)
            r.enabled = true;
        agent.speed = patrolSpeed;
        agent.isStopped = true;

        AudioManager.Instance?.PlaySound(appearSound);
        animator?.SetTrigger("Appear");

        Debug.Log("[Monster] Act 2 — Появился");
    }

    private void ActivateAct3()
    {
        gameObject.SetActive(true);
        isActive = true;
        foreach (var r in renderers)
            r.enabled = true;
        agent.speed = chaseSpeed;
        agent.isStopped = false;

        Debug.Log($"[Monster] Act 3 АКТИВИРОВАН! Позиция: {transform.position}");
    }

    private void ActivateAct4()
    {
        gameObject.SetActive(true);
        isActive = true;
        foreach (var r in renderers)
            r.enabled = true;
        agent.speed = finalChaseSpeed;
        agent.isStopped = false;

        if (player != null)
        {
            Vector3 behind = player.position - player.forward * 7f + Vector3.up * 0.5f;
            transform.position = behind;
            transform.LookAt(player);
        }

        Debug.Log("[Monster] Act 4 — Финальная охота!");
    }

    public void StartFinalChase()
    {
        SetAct(Act.Act4);
    }

    public void TeleportTo(Transform spawnPoint)
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
    }
}