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

    [Header("Settings")]
    public AIState currentState = AIState.Wander;

    public float detectionRadius = 8f;
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;
    public float catchDistance = 1.5f;

    private float timer;

    private void Start()
    {
        // Tự tìm NavMeshAgent nếu chưa kéo vào Inspector
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        timer = wanderTimer;

        // Tự tìm Player
        if (playerTransform == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        // Kiểm tra các thành phần cần thiết
        if (agent == null || playerTransform == null || waterSystem == null)
            return;

        // Ma Da phải nằm trên NavMesh mới di chuyển được
        if (!agent.isOnNavMesh)
            return;

        // TÍNH KHOẢNG CÁCH GIỮA MA DA VÀ PLAYER
        // Chỉ tính X và Z, bỏ qua Y

        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0;
        playerPosition.y = 0;

        float distanceToPlayer =
            Vector3.Distance(maDaPosition, playerPosition);


        // XỬ LÝ CÁC TRẠNG THÁI


        switch (currentState)
        {

            // WANDER

            case AIState.Wander:

                HandleWander();

                // Ma Da chỉ phát hiện Player
                // khi Player đang ở trong nước
                if (distanceToPlayer <= detectionRadius &&
                    waterSystem.playerInWater)
                {
                    currentState = AIState.Chase;

                    Debug.Log("Ma Da phát hiện Player trong nước!");
                }

                break;


            // CHASE


            case AIState.Chase:

                // Nếu Player đã lên bờ
                // Ma Da lập tức ngừng đuổi
                if (!waterSystem.playerInWater)
                {
                    agent.ResetPath();

                    currentState = AIState.Wander;

                    timer = wanderTimer;

                    Debug.Log(
                        "Player đã lên bờ! Ma Da không đuổi nữa."
                    );

                    break;
                }


                // ĐUỔI THEO PLAYER

                Vector3 chasePosition =
                    playerTransform.position;

                // Giữ Ma Da ở độ cao hiện tại
                chasePosition.y = transform.position.y;


                // Nếu chưa tới gần Player
                if (distanceToPlayer > catchDistance)
                {
                    agent.SetDestination(chasePosition);
                }
                else
                {
                    // Ma Da đã áp sát Player
                    agent.ResetPath();

                    Debug.Log("Ma Da đã áp sát Player!");
                }


                // PLAYER CHẠY QUÁ XA


                if (distanceToPlayer >
                    detectionRadius * 1.4f)
                {
                    agent.ResetPath();

                    currentState = AIState.Wander;

                    timer = wanderTimer;

                    Debug.Log("Player đã chạy thoát!");
                }

                break;


            // ATTACK


            case AIState.Attack:

                // Chưa xử lý ở Task 8
                // Sẽ làm ở Task 12
                break;
        }
    }


    // MA DA ĐI LANG THANG


    private void HandleWander()
    {
        timer += Time.deltaTime;

        // Đến thời gian hoặc không còn đường đi
        // thì chọn vị trí mới
        if (timer >= wanderTimer || !agent.hasPath)
        {
            Vector3 newPos =
                RandomNavMeshLocation(wanderRadius);

            agent.SetDestination(newPos);

            timer = 0;
        }
    }


    // TÌM VỊ TRÍ NGẪU NHIÊN TRÊN NAVMESH


    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * radius;

        randomDirection += transform.position;

        NavMeshHit hit;

        Vector3 finalPosition =
            transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out hit,
            radius,
            NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }


    // HIỂN THỊ VÙNG PHÁT HIỆN TRONG SCENE


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}