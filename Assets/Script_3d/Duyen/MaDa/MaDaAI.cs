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

    [Header("References")]
    public NavMeshAgent agent;
    public Transform playerTransform;
    public WaterSystem waterSystem;
    public PlayerSurvivalManager survivalManager;

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

    // Biến nội bộ cho chu kỳ 4 lần quan sát
    private bool đangDungQuanSat = false;
    private int soLanDaNhin = 0;
    private float boDemNhinHuong = 0f;
    private Quaternion gocXoayMucTieu;
    private bool đangXoayHuongMoi = false;

    private bool đangTrongVungNuoc = false;
    private Collider vungNuocHienTai = null;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

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

        TuDongTimVungNuocGanNhat();

        timer = wanderTimer;
        CapNhatCauHinhTheoTrangThai();
    }

    private void Update()
    {
        if (agent == null || playerTransform == null || waterSystem == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        // 1. Xử lý thời gian dính muối
        if (dangDinhMuoi)
        {
            boDemThoiGianMuoi -= Time.deltaTime;
            if (boDemThoiGianMuoi <= 0f)
            {
                dangDinhMuoi = false;
                CapNhatCauHinhTheoTrangThai();
            }
        }

        // 2. Kiểm tra nếu ra khỏi vùng nước thì buộc quay về
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

        // 3. State Machine
        switch (currentState)
        {
            case AIState.ReturnToWater:
                XuLyQuayVeVungNuoc();
                break;

            case AIState.Wander:
                HandleWander();

                // Nếu đang đứng lại quan sát mà thấy Player thì lập tức rượt đuổi
                if (playerTrongTamNhin && waterSystem.playerInWater && đangTrongVungNuoc)
                {
                    đangDungQuanSat = false;
                    ChuyenDienTrangThai(AIState.Chase);
                }
                break;

            case AIState.Chase:
                if (!waterSystem.playerInWater || !đangTrongVungNuoc)
                {
                    agent.ResetPath();
                    boDemTimKiem = thoiGianTimKiemKhiMatDau;
                    ChuyenDienTrangThai(AIState.Search);
                    break;
                }

                if (distanceToPlayer > detectionRadius * 1.4f)
                {
                    agent.ResetPath();
                    boDemTimKiem = thoiGianTimKiemKhiMatDau;
                    ChuyenDienTrangThai(AIState.Search);
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

                if (playerTrongTamNhin && waterSystem.playerInWater && đangTrongVungNuoc)
                {
                    ChuyenDienTrangThai(AIState.Chase);
                    break;
                }

                if (boDemTimKiem <= 0f)
                {
                    agent.ResetPath();
                    ChuyenDienTrangThai(AIState.Wander);
                    timer = wanderTimer;
                }
                break;

            case AIState.Attack:
                agent.ResetPath();
                CatchPlayer();
                break;
        }
    }

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
        if (other.CompareTag(tagVungNuoc))
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
        if (survivalManager == null)
        {
            ChuyenDienTrangThai(AIState.Wander);
            timer = wanderTimer;
            return;
        }

        survivalManager.PlayerCaught();
        ChuyenDienTrangThai(AIState.Wander);
        timer = wanderTimer;
    }

    private void HandleWander()
    {
        // 1. Xử lý trạng thái đứng lại thực hiện đủ 4 lần quan sát ngẫu nhiên
        if (đangDungQuanSat)
        {
            if (đangXoayHuongMoi)
            {
                // Xoay mượt mà sang hướng ngẫu nhiên mục tiêu
                transform.rotation = Quaternion.RotateTowards(transform.rotation, gocXoayMucTieu, tocDoXoayQuanSat * Time.deltaTime);

                // Khi đã xoay xong hướng đó thì dừng lại nhìn
                if (Quaternion.Angle(transform.rotation, gocXoayMucTieu) < 1f)
                {
                    đangXoayHuongMoi = false;
                    boDemNhinHuong = thoiGianNhinMoiHuong; // Bắt đầu đếm thời gian nhìn tại chỗ
                }
            }
            else
            {
                // Đứng nhìn tại chỗ trong khoảng thời gian `thoiGianNhinMoiHuong`
                boDemNhinHuong -= Time.deltaTime;
                if (boDemNhinHuong <= 0f)
                {
                    soLanDaNhin++;
                    if (soLanDaNhin >= 4)
                    {
                        // Đã nhìn đủ 4 lần -> Kết thúc quan sát, tiếp tục đi lang thang
                        đangDungQuanSat = false;
                        timer = 0f;
                    }
                    else
                    {
                        // Chưa đủ 4 lần -> Random một hướng mới để xoay sang quan sát tiếp
                        ChonHuongQuanSatNgauNhien();
                    }
                }
            }
            return;
        }

        timer += Time.deltaTime;

        // Kiểm tra xem đã đến đích chưa
        bool daDenDich = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance + 0.2f);

        if (daDenDich && agent.hasPath)
        {
            // Dừng lại, kích hoạt chuỗi 4 lần quan sát
            agent.ResetPath();
            đangDungQuanSat = true;
            soLanDaNhin = 0;
            ChonHuongQuanSatNgauNhien();
            return;
        }

        // Tìm điểm đi tiếp sau khi hết thời gian lang thang
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
        Vector3 rightBoundary = Quaternion.Euler(0, halfFOV, 0) * transform.forward * detectionRadius;

        Vector3 originPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(originPos, originPos + leftBoundary);
        Gizmos.DrawLine(originPos, originPos + rightBoundary);
    }
}