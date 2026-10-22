
using UnityEngine;
using UnityEngine.AI;

public class UnderwaterAmbush : MonoBehaviour
{
    [Header("Tham chieu")]
    public Transform player;
    public NavMeshAgent agent;
    public GameObject blackShadow;
    public GameObject maDaVisual;

    [Header("Cai dat")]
    public float khoangPhatHien = 5f;
    public float khoangBat = 1.5f;
    public float tocDoDuoi = 6f;

    [Header("Mat dau va quay ve")]
    public float khoangMatDau = 8f;
    public float thoiGianMatDau = 3f;
    public float khoangVe = 0.5f;

    private Vector3 viTriBanDau;
    private Quaternion gocQuayBanDau;

    private float thoiGianMatDauHienTai;
    private bool daBatPlayer;

    private enum TrangThai
    {
        AnDuoiNuoc,
        TruyDuoi,
        QuayVe
    }

    private TrangThai trangThai = TrangThai.AnDuoiNuoc;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p =
                GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        viTriBanDau = transform.position;
        gocQuayBanDau = transform.rotation;

        if (agent != null)
        {
            agent.speed = tocDoDuoi;
            agent.isStopped = true;
        }

        // Ban dau chi hien bong den
        if (blackShadow != null)
            blackShadow.SetActive(true);

        if (maDaVisual != null)
            maDaVisual.SetActive(false);
    }

    void Update()
    {
        if (player == null || agent == null || daBatPlayer)
            return;

        float khoangCach = Vector3.Distance(
            transform.position, player.position);

        switch (trangThai)
        {
            case TrangThai.AnDuoiNuoc:

                if (khoangCach <= khoangPhatHien)
                {
                    HienMaDa();

                    trangThai = TrangThai.TruyDuoi;
                    thoiGianMatDauHienTai = 0f;

                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
                break;

            case TrangThai.TruyDuoi:

                if (khoangCach <= khoangBat)
                {
                    BatPlayer();
                    return;
                }

                if (khoangCach > khoangMatDau)
                {
                    thoiGianMatDauHienTai += Time.deltaTime;

                    if (thoiGianMatDauHienTai >= thoiGianMatDau)
                    {
                        trangThai = TrangThai.QuayVe;
                        agent.isStopped = false;
                        agent.SetDestination(viTriBanDau);
                    }
                }
                else
                {
                    thoiGianMatDauHienTai = 0f;
                }

                if (trangThai == TrangThai.TruyDuoi)
                    agent.SetDestination(player.position);

                break;

            case TrangThai.QuayVe:

                if (khoangCach <= khoangPhatHien)
                {
                    trangThai = TrangThai.TruyDuoi;
                    thoiGianMatDauHienTai = 0f;

                    HienMaDa();
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    break;
                }

                if (!agent.pathPending &&
                    agent.remainingDistance <= khoangVe)
                {
                    agent.isStopped = true;
                    agent.ResetPath();

                    transform.rotation = gocQuayBanDau;

                    if (maDaVisual != null)
                        maDaVisual.SetActive(false);

                    if (blackShadow != null)
                        blackShadow.SetActive(true);

                    trangThai = TrangThai.AnDuoiNuoc;
                    thoiGianMatDauHienTai = 0f;
                }
                break;
        }
    }

    void HienMaDa()
    {
        if (blackShadow != null)
            blackShadow.SetActive(false);

        if (maDaVisual != null)
            maDaVisual.SetActive(true);
    }

    void BatPlayer()
    {
        daBatPlayer = true;
        agent.isStopped = true;

        PlayerSurvivalManager survival =
            FindFirstObjectByType<PlayerSurvivalManager>();

        if (survival != null)
        {
            survival.PlayerCaught();
        }
        else
        {
            Debug.LogWarning(
                "Khong tim thay PlayerSurvivalManager!");
        }
    }
}
