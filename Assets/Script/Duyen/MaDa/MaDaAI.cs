
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

    private float timer;


    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (survivalManager == null)
            survivalManager =
                FindFirstObjectByType<PlayerSurvivalManager>();

        // Ưu tiên lấy Player từ SurvivalManager
        if (survivalManager != null &&
            survivalManager.player != null)
        {
            playerTransform = survivalManager.player.transform;
        }

        // Nếu chưa tìm thấy thì tìm theo tag Player
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
            Debug.LogWarning(
                "Ma Da AI: Không tìm thấy Player. Kiểm tra Player trong SurvivalManager hoặc tag Player!"
            );

        if (waterSystem == null)
            Debug.LogWarning(
                "Ma Da AI: Không tìm thấy WaterSystem trong Scene!"
            );

        if (agent == null)
            Debug.LogWarning(
                "Ma Da AI: Không tìm thấy NavMeshAgent!"
            );
    }

    private void Update()
    {
        if (agent == null || playerTransform == null || waterSystem == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0;
        playerPosition.y = 0;

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
                    Debug.Log("Ma Da phát hiện Player trong nước!");
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
                    Debug.Log("Player đã chạy thoát!");
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
                    Debug.Log("Ma Da đã bắt được Player!");
                }
                break;

            case AIState.Attack:
                agent.ResetPath();
                CatchPlayer();
                break;
        }
    }


    private void CatchPlayer()
    {
        if (survivalManager == null)
        {
            Debug.LogWarning(
                "Chưa tìm thấy PlayerSurvivalManager!"
            );
            currentState = AIState.Wander;
            timer = wanderTimer;
            return;
        }

        Debug.Log("Ma Da bắt được Player!");
        survivalManager.PlayerCaught();

        // Chỉ bắt một lần, không gọi liên tục mỗi frame
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
                newPos, out NavMeshHit hit, wanderRadius,
                NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            timer = 0;
        }
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * radius;

        randomDirection += transform.position;

        if (NavMesh.SamplePosition(
            randomDirection, out NavMeshHit hit, radius,
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