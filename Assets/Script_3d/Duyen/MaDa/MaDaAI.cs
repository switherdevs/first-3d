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

    [Header("--- Cấu hình dính Muối (Salt) ---")]
    [Tooltip("Tag nhận diện muối")]
    public string tagSalt = "Salt";
    [Tooltip("Hệ số giảm tốc độ khi dính muối (Ví dụ: 0.5 là giảm còn một nửa)")]
    public float heSoGiamTocDoMuoi = 0.5f;
    [Tooltip("Thời gian (giây) hiệu ứng chậm kết thúc")]
    public float thoiGianDinhMuoi = 10f;

    // Phúc
    private float timer;
    private float tocDoGocAgent = 3.5f; // Lưu tốc độ mặc định của agent
    private bool dangDinhMuoi = false;
    private float boDemThoiGianMuoi = 0f;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            tocDoGocAgent = agent.speed; // Lưu lại tốc độ ban đầu
        }

        if (survivalManager == null)
            survivalManager = FindFirstObjectByType<PlayerSurvivalManager>();

        if (survivalManager != null && survivalManager.player != null)
        {
            playerTransform = survivalManager.player.transform;
        }

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        if (waterSystem == null)
            waterSystem = FindFirstObjectByType<WaterSystem>();

        timer = wanderTimer;

        if (playerTransform == null)
            Debug.LogWarning("Ma Da AI: Không tìm thấy Player!");

        if (waterSystem == null)
            Debug.LogWarning("Ma Da AI: Không tìm thấy WaterSystem!");

        if (agent == null)
            Debug.LogWarning("Ma Da AI: Không tìm thấy NavMeshAgent!");
    }

    private void Update()
    {
        if (agent == null || playerTransform == null || waterSystem == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        // Xử lý bộ đếm thời gian dính muối
        if (dangDinhMuoi)
        {
            boDemThoiGianMuoi -= Time.deltaTime;
            if (boDemThoiGianMuoi <= 0f)
            {
                // Hết thời gian 10 giây, hồi phục tốc độ cũ
                agent.speed = tocDoGocAgent;
                dangDinhMuoi = false;
                Debug.Log("Ma Da đã hết hiệu ứng chậm do muối!");
            }
        }

        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0;
        playerPosition.y = 0;

        float distanceToPlayer = Vector3.Distance(maDaPosition, playerPosition);

        switch (currentState)
        {
            case AIState.Wander:
                HandleWander();

                if (distanceToPlayer <= detectionRadius && waterSystem.playerInWater)
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

    // --- Hàm phát hiện va chạm với Muối ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagSalt))
        {
            XuLyDinhMuoi();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagSalt))
        {
            XuLyDinhMuoi();
        }
    }

    private void XuLyDinhMuoi()
    {
        if (agent == null) return;

        // Khựng lại tức thì (ngắt vận tốc hiện tại)
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        // Giảm tốc độ di chuyển
        agent.speed = tocDoGocAgent * heSoGiamTocDoMuoi;

        // Bật trạng thái dính muối và reset thời gian đếm ngược 10 giây
        dangDinhMuoi = true;
        boDemThoiGianMuoi = thoiGianDinhMuoi;

        Debug.Log("Ma Da đã dính Muối! Bị khựng lại và đi chậm trong 10 giây.");
    }

    private void CatchPlayer()
    {
        if (survivalManager == null)
        {
            currentState = AIState.Wander;
            timer = wanderTimer;
            return;
        }

        Debug.Log("Ma Da bắt được Player!");
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

            if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            timer = 0;
        }
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
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