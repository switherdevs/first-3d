
using UnityEngine;
using UnityEngine.AI;

public class MaDaAI : MonoBehaviour
{
    public enum AIState
    {
        Wander,
        Chase,
        Attack
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform playerTransform;
    public WaterSystem waterSystem;
    public PlayerSurvivalManager survivalManager;

    [Header("Settings")]
    public AIState currentState = AIState.Wander;
    public float detectionRadius = 8f;
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;
    public float catchDistance = 1.5f;

    [Header("Salt Effect")]
    public string tagSalt = "Salt";
    public float heSoGiamTocDoMuoi = 0.3f;
    public float thoiGianDinhMuoi = 10f;

    private float timer;
    private float tocDoGocAgent = 3.5f;
    private bool dangDinhMuoi = false;
    private float boDemThoiGianMuoi = 0f;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            tocDoGocAgent = agent.speed;

        if (survivalManager == null)
            survivalManager =
                FindFirstObjectByType<PlayerSurvivalManager>();

        if (survivalManager != null &&
            survivalManager.player != null)
        {
            playerTransform = survivalManager.player.transform;
        }

        if (playerTransform == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        if (waterSystem == null)
            waterSystem = FindFirstObjectByType<WaterSystem>();

        timer = wanderTimer;

        if (playerTransform == null)
            Debug.LogWarning("Ma Da AI: Khong tim thay Player!");

        if (waterSystem == null)
            Debug.LogWarning("Ma Da AI: Khong tim thay WaterSystem!");

        if (agent == null)
            Debug.LogWarning("Ma Da AI: Khong tim thay NavMeshAgent!");
    }

    private void Update()
    {
        // Dem nguoc hieu ung dinh muoi
        if (dangDinhMuoi)
        {
            boDemThoiGianMuoi -= Time.deltaTime;

            if (boDemThoiGianMuoi <= 0f)
            {
                dangDinhMuoi = false;

                if (agent != null)
                    agent.speed = tocDoGocAgent;

                Debug.Log("Ma Da da het dinh muoi!");
            }
        }

        if (agent == null ||
            playerTransform == null ||
            waterSystem == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        // Dinh muoi: dung AI tam thoi, khong duoi bat Player
        if (dangDinhMuoi)
            return;

        // Player dang tren thuyen
        if (BoatController.PlayerDangTrenThuyen)
        {
            agent.ResetPath();
            currentState = AIState.Wander;
            timer = wanderTimer;
            return;
        }

        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0f;
        playerPosition.y = 0f;

        float distanceToPlayer =
            Vector3.Distance(maDaPosition, playerPosition);

        switch (currentState)
        {
            case AIState.Wander:
                HandleWander();

                if (distanceToPlayer <= detectionRadius &&
                    waterSystem.playerInWater)
                {
                    currentState = AIState.Chase;
                    Debug.Log("Ma Da phat hien Player trong nuoc!");
                }
                break;

            case AIState.Chase:
                if (!waterSystem.playerInWater)
                {
                    agent.ResetPath();
                    currentState = AIState.Wander;
                    timer = wanderTimer;
                    break;
                }

                if (distanceToPlayer > detectionRadius * 1.4f)
                {
                    agent.ResetPath();
                    currentState = AIState.Wander;
                    timer = wanderTimer;
                    Debug.Log("Player da chay thoat!");
                    break;
                }

                Vector3 chasePosition = playerTransform.position;
                chasePosition.y = transform.position.y;

                if (distanceToPlayer > catchDistance)
                {
                    agent.SetDestination(chasePosition);
                }
                else
                {
                    agent.ResetPath();
                    currentState = AIState.Attack;
                    Debug.Log("Ma Da da bat duoc Player!");
                }
                break;

            case AIState.Attack:
                agent.ResetPath();
                CatchPlayer();
                break;
        }
    }

    // Phat hien muoi bang Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagSalt))
            XuLyDinhMuoi();
    }

    // Phat hien muoi bang Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagSalt))
            XuLyDinhMuoi();
    }

    private void XuLyDinhMuoi()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        // Khung lai ngay lap tuc
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        // Giam toc do
        agent.speed = tocDoGocAgent * heSoGiamTocDoMuoi;

        dangDinhMuoi = true;
        boDemThoiGianMuoi = thoiGianDinhMuoi;

        currentState = AIState.Wander;

        Debug.Log("Ma Da dinh muoi! Bi khung va di cham trong 10 giay.");
    }

    private void CatchPlayer()
    {
        if (BoatController.PlayerDangTrenThuyen)
        {
            currentState = AIState.Wander;
            timer = wanderTimer;
            return;
        }

        if (survivalManager == null)
        {
            Debug.LogWarning("Chua tim thay PlayerSurvivalManager!");
            currentState = AIState.Wander;
            timer = wanderTimer;
            return;
        }

        Debug.Log("Ma Da bat duoc Player!");
        survivalManager.PlayerCaught();

        currentState = AIState.Wander;
        timer = wanderTimer;
    }

    private void HandleWander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer || !agent.hasPath)
        {
            Vector3 newPos = RandomNavMeshLocation(wanderRadius);

            if (NavMesh.SamplePosition(
                newPos,
                out NavMeshHit hit,
                wanderRadius,
                NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            timer = 0f;
        }
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * radius;

        randomDirection += transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            radius,
            NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
