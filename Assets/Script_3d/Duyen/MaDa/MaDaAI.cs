using UnityEngine;
using UnityEngine.AI;

public class MaDaAI : MonoBehaviour
{
    public enum AIState
    {
        Wander,
        Chase,
        Attack,
        ReturnToWater,
        Search
    }

    [Header("References (Tự động tìm kiếm)")]
    public NavMeshAgent agent;
    public Transform playerTransform;
    public WaterSystem waterSystem;
    public PlayerSurvivalManager survivalManager;
    public Animator animator;
    public AudioSource audioSource;

    [Header("Settings - Âm Thanh (Audio Configuration)")]
    [Tooltip("Danh sách các âm thanh rên rỉ/hù dọa ngẫu nhiên")]
    public AudioClip[] randomAmbientClips;
    [Tooltip("Khoảng thời gian (giây) giữa các lần phát âm thanh ngẫu nhiên")]
    public float thoiGianAmThanhRandom = 10f;
    [Tooltip("Âm thanh phát lên đúng 1 lần khi phát hiện ra Player")]
    public AudioClip detectionClip;
    [Range(0f, 1f)]
    [Tooltip("Tùy chỉnh âm lượng của âm thanh phát hiện Player")]
    public float detectionVolume = 1.0f;

    [Header("Settings - Tên Thông Số Animator (Animation Parameters)")]
    [Tooltip("Tên tham số Bool điều khiển trạng thái đi bộ (ví dụ: IsWalking)")]
    public string animParamIsWalking = "IsWalking";
    [Tooltip("Tên tham số Bool điều khiển trạng thái chạy rượt đuổi (ví dụ: IsRunning)")]
    public string animParamIsRunning = "IsRunning";
    [Tooltip("Tên tham số Trigger kích hoạt hành động tấn công (ví dụ: Attack)")]
    public string animParamAttackTrigger = "Attack";

    [Header("Settings - Vùng Nước (Water Boundary)")]
    [Tooltip("Tag nhận diện các vùng nước")]
    public string tagVungNuoc = "Water";

    [Header("Settings - Tốc độ & Quay (Speed & Rotation)")]
    [Tooltip("Tốc độ di chuyển khi đi lang thang hoặc quay về vùng nước")]
    public float tocDoLangThang = 2.0f;
    [Tooltip("Tốc độ chạy khi rượt đuổi hoặc lùng sục tìm kiếm Player")]
    public float tocDoChayRuuet = 5.0f;
    [Tooltip("Tốc độ xoay người khi lang thang (để đi thẳng mượt mà)")]
    public float tocDoXoayLangThang = 60f;
    [Tooltip("Tốc độ xoay người khi rượt đuổi Player")]
    public float tocDoXoayRuuet = 360f;

    [Header("Settings - Quan Sát 4 Hướng (Look Around)")]
    [Tooltip("Thời gian (giây) đứng nhìn ở mỗi hướng trong 4 lần quan sát")]
    public float thoiGianNhinMoiHuong = 1.0f;
    [Tooltip("Tốc độ xoay chuyển góc nhìn giữa các hướng")]
    public float tocDoXoayQuanSat = 120f;

    [Header("Settings - Tầm nhìn (Vision FOV)")]
    [Tooltip("Bán kính tầm nhìn của Ma Đa")]
    public float detectionRadius = 8f;
    [Tooltip("Góc nhìn phía trước của Ma Đa (Ví dụ: 120 độ)")]
    [Range(0f, 360f)]
    public float fieldOfViewAngle = 120f;
    [Tooltip("Thời gian (giây) lùng sục xung quanh khi mất dấu Player")]
    public float thoiGianTimKiemKhiMatDau = 5f;

    [Header("Settings - AI Behavior")]
    public AIState currentState = AIState.Wander;
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

    // Biến nội bộ quản lý trạng thái
    private float timer;
    private bool dangDinhMuoi = false;
    private float boDemThoiGianMuoi = 0f;
    private float boDemTimKiem = 0f;
    private float boDemAmThanhRandom = 0f;

    // Biến quản lý trạng thái phát âm thanh phát hiện
    private bool daPhatAmThanhPhatHien = false;
    private bool daBatDuocPlayer = false; // Cờ khóa hẳn AI khi đã bắt được người chơi

    // Biến nội bộ cho chu kỳ 4 lần quan sát
    private bool đangDungQuanSat = false;
    private int soLanDaNhin = 0;
    private float boDemNhinHuong = 0f;
    private Quaternion gocXoayMucTieu;
    private bool đangXoayHuongMoi = false;

    private bool đangTrongVungNuoc = false;
    private Collider vungNuocHienTai = null;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (waterSystem == null) waterSystem = FindFirstObjectByType<WaterSystem>();
        if (survivalManager == null) survivalManager = FindFirstObjectByType<PlayerSurvivalManager>();

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
    }

    private void Start()
    {
        TuDongTimVungNuocGanNhat();

        timer = wanderTimer;
        boDemAmThanhRandom = thoiGianAmThanhRandom;
        CapNhatCauHinhTheoTrangThai();
    }

    private void Update()
    {
        // Nếu đã bắt được người chơi hoặc chết thì dừng toàn bộ AI, không xử lý nữa
        if (daBatDuocPlayer || agent == null || playerTransform == null || waterSystem == null)
            return;

        // Bổ sung kiểm tra: Nếu tag của player không còn là "Player" nữa (đã chuyển thành default/Untagged khi chết) thì dừng phát hiện
        if (playerTransform.CompareTag("Untagged") || !playerTransform.gameObject.activeInHierarchy)
        {
            agent.ResetPath();
            CapNhatAnimation(false, false);
            return;
        }

        if (!agent.isOnNavMesh)
            return;

        // 1. Xử lý âm thanh ngẫu nhiên theo thời gian
        XuLyAmThanhRandom();

        // 2. Xử lý thời gian dính muối
        if (dangDinhMuoi)
        {
            boDemThoiGianMuoi -= Time.deltaTime;
            if (boDemThoiGianMuoi <= 0f)
            {
                dangDinhMuoi = false;
                CapNhatCauHinhTheoTrangThai();
            }
        }

        // 3. Kiểm tra nếu ra khỏi vùng nước thì buộc quay về
        if (!đangTrongVungNuoc && currentState != AIState.ReturnToWater)
        {
            ChuyenDienTrangThai(AIState.ReturnToWater);
        }

        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0;
        playerPosition.y = 0;

        float distanceToPlayer = Vector3.Distance(maDaPosition, playerPosition);

        Vector3 huongToPlayer = (playerPosition - maDaPosition).normalized;
        float gocQuetToPlayer = Vector3.Angle(transform.forward, huongToPlayer);
        bool playerTrongTamNhin = distanceToPlayer <= detectionRadius && gocQuetToPlayer <= (fieldOfViewAngle / 2f);

        // 4. State Machine
        switch (currentState)
        {
            case AIState.ReturnToWater:
                XuLyQuayVeVungNuoc();
                CapNhatAnimation(true, false);
                break;

            case AIState.Wander:
                HandleWander();

                if (playerTrongTamNhin && waterSystem.playerInWater && đangTrongVungNuoc)
                {
                    đangDungQuanSat = false;
                    PhatAmThanhPhatHien();
                    ChuyenDienTrangThai(AIState.Chase);
                }
                break;

            case AIState.Chase:
                if (!waterSystem.playerInWater || !đangTrongVungNuoc)
                {
                    agent.ResetPath();
                    boDemTimKiem = thoiGianTimKiemKhiMatDau;
                    daPhatAmThanhPhatHien = false;
                    ChuyenDienTrangThai(AIState.Search);
                    break;
                }

                if (distanceToPlayer > detectionRadius * 1.4f)
                {
                    agent.ResetPath();
                    boDemTimKiem = thoiGianTimKiemKhiMatDau;
                    daPhatAmThanhPhatHien = false;
                    ChuyenDienTrangThai(AIState.Search);
                    break;
                }

                Vector3 chasePosition = playerTransform.position;
                chasePosition.y = transform.position.y;

                if (distanceToPlayer > catchDistance)
                {
                    agent.SetDestination(chasePosition);
                    CapNhatAnimation(false, true);
                }
                else
                {
                    agent.ResetPath();
                    ChuyenDienTrangThai(AIState.Attack);
                }
                break;

            case AIState.Search:
                boDemTimKiem -= Time.deltaTime;

                if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.5f))
                {
                    Vector3 randomSearchPos = RandomNavMeshLocation(wanderRadius);
                    agent.SetDestination(randomSearchPos);
                }

                CapNhatAnimation(false, true);

                if (playerTrongTamNhin && waterSystem.playerInWater && đangTrongVungNuoc)
                {
                    PhatAmThanhPhatHien();
                    ChuyenDienTrangThai(AIState.Chase);
                    break;
                }

                if (boDemTimKiem <= 0f)
                {
                    agent.ResetPath();
                    daPhatAmThanhPhatHien = false;
                    ChuyenDienTrangThai(AIState.Wander);
                    timer = wanderTimer;
                }
                break;

            case AIState.Attack:
                agent.ResetPath();
                CapNhatAnimation(false, false);
                TriggerAttackAnimation();
                CatchPlayer();
                break;
        }
    }

    #region Quản lý Âm Thanh (Audio Management)
    private void XuLyAmThanhRandom()
    {
        if (audioSource == null || randomAmbientClips == null || randomAmbientClips.Length == 0 || daBatDuocPlayer)
            return;

        boDemAmThanhRandom -= Time.deltaTime;
        if (boDemAmThanhRandom <= 0f)
        {
            AudioClip clipNgauNhien = randomAmbientClips[Random.Range(0, randomAmbientClips.Length)];
            if (clipNgauNhien != null)
            {
                audioSource.PlayOneShot(clipNgauNhien);
            }
            boDemAmThanhRandom = thoiGianAmThanhRandom;
        }
    }

    private void PhatAmThanhPhatHien()
    {
        // Chỉ phát nếu chưa từng phát và người chơi chưa chết
        if (!daPhatAmThanhPhatHien && !daBatDuocPlayer && audioSource != null && detectionClip != null)
        {
            audioSource.PlayOneShot(detectionClip, detectionVolume);
            daPhatAmThanhPhatHien = true;
        }
    }
    #endregion

    #region Quản lý Hoạt hoạ (Animation Management)
    private void CapNhatAnimation(bool isWalking, bool isRunning)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(animParamIsWalking))
            animator.SetBool(animParamIsWalking, isWalking);

        if (!string.IsNullOrEmpty(animParamIsRunning))
            animator.SetBool(animParamIsRunning, isRunning);
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(animParamAttackTrigger))
            animator.SetTrigger(animParamAttackTrigger);
    }
    #endregion

    #region Quản lý Trạng thái, Tốc độ & Góc xoay
    private void ChuyenDienTrangThai(AIState trangThaiMoi)
    {
        currentState = trangThaiMoi;
        CapNhatCauHinhTheoTrangThai();
    }

    private void CapNhatCauHinhTheoTrangThai()
    {
        if (agent == null) return;

        bool isChasingOrSearching = (currentState == AIState.Chase || currentState == AIState.Search);
        float tocDoCoSo = isChasingOrSearching ? tocDoChayRuuet : tocDoLangThang;

        if (dangDinhMuoi)
        {
            agent.speed = tocDoCoSo * heSoGiamTocDoMuoi;
        }
        else
        {
            agent.speed = tocDoCoSo;
        }

        agent.angularSpeed = isChasingOrSearching ? tocDoXoayRuuet : tocDoXoayLangThang;
    }
    #endregion

    #region Xử lý Vùng Nước & Va Chạm Muối
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagVungNuoc) || other.CompareTag(tagVungNuoc))
        {
            đangTrongVungNuoc = true;
            vungNuocHienTai = other;
        }
        else if (other.CompareTag(tagSalt))
        {
            XuLyDinhMuoi();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagVungNuoc) && other == vungNuocHienTai)
        {
            đangTrongVungNuoc = false;
            vungNuocHienTai = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagSalt))
        {
            XuLyDinhMuoi();
        }
    }

    private void TuDongTimVungNuocGanNhat()
    {
        GameObject[] danhSachVungNuoc = GameObject.FindGameObjectsWithTag(tagVungNuoc);
        if (danhSachVungNuoc.Length == 0) return;

        Transform vungNuocGanNhat = null;
        float khoangCachNganNhat = Mathf.Infinity;

        foreach (GameObject nuocObj in danhSachVungNuoc)
        {
            float khoangCach = Vector3.Distance(transform.position, nuocObj.transform.position);
            if (khoangCach < khoangCachNganNhat)
            {
                khoangCachNganNhat = khoangCach;
                vungNuocGanNhat = nuocObj.transform;
            }
        }

        if (vungNuocGanNhat != null)
        {
            vungNuocHienTai = vungNuocGanNhat.GetComponent<Collider>();
            đangTrongVungNuoc = true;
        }
    }

    private void XuLyQuayVeVungNuoc()
    {
        GameObject[] danhSachVungNuoc = GameObject.FindGameObjectsWithTag(tagVungNuoc);
        if (danhSachVungNuoc.Length == 0) return;

        Transform vungNuocGanNhat = null;
        float khoangCachNganNhat = Mathf.Infinity;

        foreach (GameObject nuocObj in danhSachVungNuoc)
        {
            float khoangCach = Vector3.Distance(transform.position, nuocObj.transform.position);
            if (khoangCach < khoangCachNganNhat)
            {
                khoangCachNganNhat = khoangCach;
                vungNuocGanNhat = nuocObj.transform;
            }
        }

        if (vungNuocGanNhat != null)
        {
            agent.SetDestination(vungNuocGanNhat.position);

            if (đangTrongVungNuoc)
            {
                agent.ResetPath();
                daPhatAmThanhPhatHien = false;
                ChuyenDienTrangThai(AIState.Wander);
                timer = wanderTimer;
            }
        }
    }
    #endregion

    private void XuLyDinhMuoi()
    {
        if (agent == null) return;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        dangDinhMuoi = true;
        boDemThoiGianMuoi = thoiGianDinhMuoi;

        CapNhatCauHinhTheoTrangThai();
    }

    private void CatchPlayer()
    {
        daBatDuocPlayer = true; // Đánh dấu đã tóm được player, khóa vĩnh viễn AI

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (survivalManager != null)
        {
            survivalManager.PlayerCaught();
        }
    }

    private void HandleWander()
    {
        if (đangDungQuanSat)
        {
            CapNhatAnimation(false, false);

            if (đangXoayHuongMoi)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, gocXoayMucTieu, tocDoXoayQuanSat * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, gocXoayMucTieu) < 1f)
                {
                    đangXoayHuongMoi = false;
                    boDemNhinHuong = thoiGianNhinMoiHuong;
                }
            }
            else
            {
                boDemNhinHuong -= Time.deltaTime;
                if (boDemNhinHuong <= 0f)
                {
                    soLanDaNhin++;
                    if (soLanDaNhin >= 4)
                    {
                        đangDungQuanSat = false;
                        timer = 0f;
                    }
                    else
                    {
                        ChonHuongQuanSatNgauNhien();
                    }
                }
            }
            return;
        }

        timer += Time.deltaTime;

        bool daDenDich = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance + 0.2f);

        if (daDenDich && agent.hasPath)
        {
            agent.ResetPath();
            đangDungQuanSat = true;
            soLanDaNhin = 0;
            ChonHuongQuanSatNgauNhien();
            return;
        }

        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            CapNhatAnimation(true, false);
        }

        if (timer >= wanderTimer || (!agent.pathPending && !agent.hasPath))
        {
            Vector3 newPos = RandomNavMeshLocation(wanderRadius);
            agent.SetDestination(newPos);
            timer = 0f;
        }
    }

    private void ChonHuongQuanSatNgauNhien()
    {
        float randomAngle = Random.Range(0f, 360f);
        gocXoayMucTieu = Quaternion.Euler(0, randomAngle, 0);
        đangXoayHuongMoi = true;
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * detectionRadius);

        Gizmos.color = Color.cyan;
        float halfFOV = fieldOfViewAngle / 2f;
        Vector3 leftBoundary = Quaternion.Euler(0, -halfFOV, 0) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward * detectionRadius;

        Vector3 originPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(originPos, originPos + leftBoundary);
        Gizmos.DrawLine(originPos, originPos + rightBoundary);
    }
}