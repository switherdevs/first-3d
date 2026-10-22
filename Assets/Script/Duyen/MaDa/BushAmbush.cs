
using UnityEngine;
using UnityEngine.AI;

public class BushAmbush : MonoBehaviour
{
    [Header("Tham chieu")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Cai dat")]
    public float khoangPhatHien = 5f;
    public float khoangBat = 1.5f;
    public float tocDoLaoRa = 6f;

    [Header("Mat dau va quay ve")]
    public float khoangMatDau = 8f;
    public float thoiGianMatDau = 3f;
    public float khoangVeBui = 0.5f;

    private Renderer[] renderers;
    private Vector3 viTriBui;
    private Quaternion gocQuayBanDau;

    private enum TrangThai
    {
        AnNup,
        TruyDuoi,
        QuayVe
    }

    private TrangThai trangThai = TrangThai.AnNup;
    private float thoiGianMatDauHienTai = 0f;
    private bool daBatPlayer = false;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        // Luu vi tri va goc quay ban dau trong bui cay
        viTriBui = transform.position;
        gocQuayBanDau = transform.rotation;

        // Lay hinh the Ma Da
        renderers = GetComponentsInChildren<Renderer>(true);

        AnMaDa();

        if (agent != null)
        {
            agent.speed = tocDoLaoRa;
            agent.isStopped = true;
        }
    }

    void Update()
    {
        if (player == null || agent == null || daBatPlayer)
            return;

        float khoangCach = Vector3.Distance(
            transform.position,
            player.position
        );

        switch (trangThai)
        {
            case TrangThai.AnNup:
                if (khoangCach <= khoangPhatHien)
                {
                    trangThai = TrangThai.TruyDuoi;
                    thoiGianMatDauHienTai = 0f;

                    HienMaDa();
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

                // Player chay qua xa thi bat dau dem thoi gian
                if (khoangCach > khoangMatDau)
                {
                    thoiGianMatDauHienTai += Time.deltaTime;

                    if (thoiGianMatDauHienTai >= thoiGianMatDau)
                    {
                        trangThai = TrangThai.QuayVe;
                        agent.isStopped = false;
                        agent.SetDestination(viTriBui);
                    }
                }
                else
                {
                    thoiGianMatDauHienTai = 0f;
                }

                // Cap nhat huong duoi theo Player
                if (trangThai == TrangThai.TruyDuoi)
                    agent.SetDestination(player.position);

                break;

            case TrangThai.QuayVe:
                // Player quay lai gan thi Ma Da tiep tuc duoi
                if (khoangCach <= khoangPhatHien)
                {
                    trangThai = TrangThai.TruyDuoi;
                    thoiGianMatDauHienTai = 0f;

                    HienMaDa();
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    break;
                }

                // Ma Da da quay ve toi bui cay
                if (!agent.pathPending &&
                    agent.remainingDistance <= khoangVeBui)
                {
                    agent.isStopped = true;
                    agent.ResetPath();

                    transform.rotation = gocQuayBanDau;
                    AnMaDa();

                    trangThai = TrangThai.AnNup;
                    thoiGianMatDauHienTai = 0f;
                }
                break;
        }
    }

    void AnMaDa()
    {
        if (renderers == null) return;

        foreach (Renderer r in renderers)
            r.enabled = false;
    }

    void HienMaDa()
    {
        if (renderers == null) return;

        foreach (Renderer r in renderers)
            r.enabled = true;
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
                "Khong tim thay PlayerSurvivalManager!"
            );
        }
    }
}
