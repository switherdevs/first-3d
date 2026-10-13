using System;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] public float skibidi = 3f;

    void Start()
    {
        // Tự động hủy GameObject này sau khoảng thời gian lưu trong biến skibidi (3 giây)
        Destroy(gameObject, skibidi);
    }

    void Update()
    {

    }
}