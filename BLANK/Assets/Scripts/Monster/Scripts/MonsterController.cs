using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    public enum MonsterState { Disabled, Wander, Chase, FinalChase }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Speeds")]
    public float wanderSpeed = 3.7f;
    public float chaseSpeed = 5.8f;
    public float finalChaseSpeed = 7.8f;
    [Tooltip("Скорость в Act 4")]
    public float act4ChaseSpeed = 8.5f;

    [Header("Vision Settings")]
    public float closeVisionRange = 4f;
    public float forwardVisionRange = 15f;
    public float chaseCloseVisionRange = 9f;
    public float chaseForwardVisionRange = 40f;
    public float fieldOfView = 100f;

    [Header("Memory & Behaviour")]
    public float memoryTime = 5f;
    public float pauseAfterLoseTime = 2f;

    [Header("Attack")]
    public float attackDistance = 3.0f;
    public float attackCooldown = 2.5f;

    [Header("Wander Settings")]
    public float wanderRadius = 32f;
    public float minTimeBetweenPoints = 10f;
    public float maxTimeBetweenPoints = 50f;

    [Header("Spawn")]
    public Transform act3SpawnPoint;

    [Header("Sounds")]
    public string screamSound = "Scream";

    [Header("Debug")]
    public bool showDebug = true;
    public bool isBlind = false;

    // ================== Private ==================
    private MonsterState currentState = MonsterState.Disabled;
    private float nextWanderTime = 0f;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float lastAttackTime = 0f;

    private float loseSightTimer = 0f;
    private Vector3 lastKnownPlayerPosition;
    private bool hasMemoryTarget = false;
    private bool isScaring = false;
    private bool isInSafeZone = false;

    private void Awake()
    {
        Debug.Log("[Monster] AWAKE");

        // Инициализируем agent как можно раньше
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent == null)
            Debug.LogError("[Monster] NavMeshAgent НЕ НАЙДЕН НА ОБЪЕКТЕ! Добавь компонент NavMeshAgent.");
    }

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // дублирование на всякий случай

        animator = GetComponent<Animator>(); // уже есть

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.angularSpeed = 280f;
            agent.acceleration = 40f;
            agent.stoppingDistance = 0.8f;
            agent.autoBraking = false;
        }
        else
        {
            Debug.LogError("[Monster] CRITICAL: NavMeshAgent missing!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T)) // Нажми T чтобы протестировать
        {
            animator.SetBool("IsWalking", !animator.GetBool("IsWalking"));
            Debug.Log($"[Monster] Переключили IsWalking на: {animator.GetBool("IsWalking")}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // Нажми 1 для атаки
        {
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isBlind = !isBlind;
            Debug.Log($"[Monster] Blind Mode: {(isBlind ? "ВКЛ" : "ВЫКЛ")}");
        }

        if (agent == null || currentState == MonsterState.Disabled) return;

        bool canSeePlayer = CanSeePlayer();

        switch (currentState)
        {
            case MonsterState.Wander:
                WanderBehaviour();
                if (canSeePlayer && !isBlind && !isScaring)
                    StartChasing();
                break;

            case MonsterState.Chase:
            case MonsterState.FinalChase:
                ChaseBehaviour(canSeePlayer);
                break;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null || isBlind) return false;

        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        float currentClose = (currentState == MonsterState.Wander) ? closeVisionRange : chaseCloseVisionRange;
        float currentForward = (currentState == MonsterState.Wander) ? forwardVisionRange : chaseForwardVisionRange;

        if (dist <= currentClose) return true;

        if (dist <= currentForward)
        {
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle <= fieldOfView * 0.5f)
            {
                return !Physics.Linecast(transform.position + Vector3.up * 1.2f, player.position + Vector3.up * 1.2f);
            }
        }
        return false;
    }

    private void StartChasing()
    {
        currentState = MonsterState.Chase;
        agent.speed = chaseSpeed;
        loseSightTimer = 0f;
        hasMemoryTarget = true;
        lastKnownPlayerPosition = player.position;
        agent.SetDestination(player.position);
        SetWalking(true);
        Debug.Log("[Monster] → Chase (увидел игрока)");
    }

    private void ChaseBehaviour(bool canSeePlayer)
    {
        if (player == null) return;

        if (canSeePlayer && !isBlind)
        {
            lastKnownPlayerPosition = player.position;
            hasMemoryTarget = true;
            loseSightTimer = 0f;
            agent.SetDestination(player.position);
        }
        else
        {
            loseSightTimer += Time.deltaTime;

            if (loseSightTimer <= memoryTime && hasMemoryTarget)
                agent.SetDestination(lastKnownPlayerPosition);
            else
                LosePlayer();
        }

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
            TryPerformAttack();
    }

    private void LosePlayer()
    {
        Debug.Log("[Monster] ПОТЕРЯЛ ИГРОКА полностью");
        agent.isStopped = true;
        SetWalking(false);
        loseSightTimer = 0f;
        hasMemoryTarget = false;
        Invoke("ResumePatrol", pauseAfterLoseTime);
    }

    private void ResumePatrol()
    {
        agent.isStopped = false;
        agent.speed = wanderSpeed;
        currentState = MonsterState.Wander;
        nextWanderTime = Time.time;
        SetWalking(true);
        Debug.Log("[Monster] Возвращаюсь в патрулирование");
    }

    private void WanderBehaviour()
    {
        if (isScaring) return;

        SetWalking(true);   // ← Важно для патруля

        stuckTimer += Time.deltaTime;
        if (Vector3.Distance(transform.position, lastPosition) < 0.3f && stuckTimer > 8f)
        {
            RandomWanderPoint();
            stuckTimer = 0f;
            nextWanderTime = Time.time + Random.Range(minTimeBetweenPoints, maxTimeBetweenPoints);
        }
        lastPosition = transform.position;

        if (Time.time >= nextWanderTime)
        {
            RandomWanderPoint();
            nextWanderTime = Time.time + Random.Range(minTimeBetweenPoints, maxTimeBetweenPoints);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            nextWanderTime = Time.time;
        }
    }

    // Вспомогательный метод для удобства
    private void SetWalking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsWalking", value);
    }

    private void RandomWanderPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius * 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void TryPerformAttack()
    {
        if (isInSafeZone) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        // Звуки и анимация атаки
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(screamSound);

        if (animator != null)
            animator.SetTrigger("Attack");

        Debug.Log("[Monster] ИГРОК ПОЙМАН!");

        // Проверяем акт — в 1 и 2 акте не убиваем
        if (ActManager.Instance != null)
        {
            if (ActManager.Instance.currentAct == ActManager.GameAct.Act1 ||
                ActManager.Instance.currentAct == ActManager.GameAct.Act2)
            {
                Debug.Log("[Monster] Смерть отключена в Act 1 / Act 2");
                return;
            }
        }

        // Смерть только в Act 3 и Act 4
        if (PlayerSafety.Instance != null)
        {
            PlayerSafety.Instance.KillPlayer();
        }
        else
        {
            // Если PlayerSafety нет — просто респавн на месте
            Debug.LogWarning("PlayerSafety не найден! Респавним на месте.");
            if (player != null)
                player.position = transform.position + Vector3.back * 5f; // грубый респавн
        }
    }

    // ====================== JUMP SCARE & ACT 4 ======================
    public void ScareAppearAndRun(Transform spawnPoint, Transform targetPoint, float runSpeed = 7.5f)
    {
        if (spawnPoint == null) return;
        isScaring = true;

        gameObject.SetActive(true);
        transform.position = spawnPoint.position;
        if (targetPoint != null)
            transform.LookAt(targetPoint.position);

        currentState = MonsterState.Wander;
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.avoidancePriority = 0;

        if (targetPoint != null)
            agent.SetDestination(targetPoint.position);

        SetWalking(true);
        Debug.Log("[Monster] Scare: Иду ПРЯМО к targetPoint");
        StartCoroutine(DisappearAfterReach(targetPoint));
    }

    private IEnumerator DisappearAfterReach(Transform targetPoint)
    {
        if (targetPoint != null)
        {
            float startTime = Time.time;
            while (Time.time - startTime < 10f)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 1.5f)
                    break;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        yield return new WaitForSeconds(0.4f);

        agent.isStopped = true;
        SetWalking(false);
        currentState = MonsterState.Disabled;
        isScaring = false;
        gameObject.SetActive(false);

        Debug.Log("[Monster] Act 2 Jump Scare — исчез");
    }

    public void StartAct4Chase(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        gameObject.SetActive(true);
        transform.position = spawnPoint.position;
        if (player != null)
            transform.LookAt(player.position + Vector3.up * 1.5f);

        currentState = MonsterState.FinalChase;
        agent.isStopped = false;
        agent.speed = act4ChaseSpeed;

        hasMemoryTarget = true;
        loseSightTimer = 0f;

        SetWalking(true);
        Debug.Log("[Monster] ACT 4 FINAL CHASE — началась!");
    }

    // ====================== SAFE ZONE ======================
    public void EnterSafeZone()
    {
        isInSafeZone = true;
        Debug.Log("[Monster] Игрок вошёл в безопасную зону");
        if (agent != null)
            agent.speed = Mathf.Min(agent.speed, wanderSpeed * 0.6f);
    }

    public void ExitSafeZone()
    {
        isInSafeZone = false;
        Debug.Log("[Monster] Игрок вышел из безопасной зоны");
        if (agent != null)
        {
            if (currentState == MonsterState.FinalChase)
                agent.speed = finalChaseSpeed;
            else if (currentState == MonsterState.Chase)
                agent.speed = chaseSpeed;
            else
                agent.speed = wanderSpeed;
        }
    }

    // ====================== Публичные ======================
    public void StartChase()
    {
        if (act3SpawnPoint != null)
            transform.position = act3SpawnPoint.position;

        gameObject.SetActive(true);
        currentState = MonsterState.Wander;
        agent.isStopped = false;
        agent.speed = wanderSpeed;
        nextWanderTime = Time.time + 4f;

        SetWalking(true);
        Debug.Log("[Monster] Act 3 — Патрулирование");
    }

    public void AppearBehindPlayer()
    {
        if (player == null) return;
        Vector3 behindPos = player.position - player.forward * 6.5f;
        transform.position = behindPos;
        transform.LookAt(player.position + Vector3.up * 1.5f);

        gameObject.SetActive(true);
        currentState = MonsterState.FinalChase;
        agent.isStopped = false;
        agent.speed = finalChaseSpeed;

        SetWalking(true);
        Debug.Log("[Monster] Final Chase — появился сзади");
    }

    public void AppearAndMoveTo(Transform spawnPoint, Transform targetPoint)
    {
        if (spawnPoint == null) return;
        gameObject.SetActive(true);
        transform.position = spawnPoint.position;
        currentState = MonsterState.Wander;
        agent.isStopped = false;
        agent.speed = wanderSpeed;

        if (targetPoint != null)
            agent.SetDestination(targetPoint.position);
        else
            RandomWanderPoint();

        SetWalking(true);
    }

    // Добавь этот метод в класс
    public void SetState(MonsterState newState)
    {
        currentState = newState;
        if (newState == MonsterState.Disabled)
            agent.isStopped = true;
    }
}