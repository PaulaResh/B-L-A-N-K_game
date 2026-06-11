using UnityEngine;
using UnityEngine.SceneManagement;

public class BabaykaAI : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float viewDistance = 10f;
    public float killDistance = 1.5f;

    [Header("Patrol")]
    public Transform[] points;
    public float waitTimeAtPoint = 2f;

    [Header("Safe Zone")]
    public float waitTimeInSafeZone = 3f;

    private Transform player;
    private CharacterController controller;

    private int index = 0;

    private bool waiting = false;
    private float waitTimer = 0f;

    private bool safeWaiting = false;
    private float safeTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null || points.Length == 0) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 🏠 SAFE ZONE
        if (PlayerSafety.isSafe)
        {
            SafeZoneBehaviour();
            return;
        }
        else
        {
            safeWaiting = false; // сброс при выходе
        }

        // 💀 CHASE
        if (dist < viewDistance)
        {
            waiting = false;
            Chase();
            return;
        }

        PatrolWithWait();
    }

    // ---------------- PATROL ----------------

    void PatrolWithWait()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                waiting = false;
                index = Random.Range(0, points.Length);
            }

            return;
        }

        MoveTo(points[index].position, patrolSpeed);

        if (Vector3.Distance(transform.position, points[index].position) < 1f)
        {
            waiting = true;
            waitTimer = waitTimeAtPoint;
        }
    }

    // ---------------- CHASE ----------------

    void Chase()
    {
        MoveTo(player.position, chaseSpeed);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < killDistance)
            KillPlayer();
    }

    // ---------------- MOVE ----------------

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;

        controller.Move(dir * speed * Time.deltaTime);

        if (dir != Vector3.zero)
            transform.forward = dir;
    }

    // ---------------- KILL ----------------

    void KillPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SafeZoneBehaviour()
    {
        if (!safeWaiting)
        {
            safeWaiting = true;
            safeTimer = waitTimeInSafeZone;
        }

        if (safeTimer > 0)
        {
            safeTimer -= Time.deltaTime;
            return; // стоит
        }

        // после ожидания просто патруль
        PatrolWithWait();
    }
}