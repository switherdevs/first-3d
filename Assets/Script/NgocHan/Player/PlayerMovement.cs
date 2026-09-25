using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Nhận dữ liệu từ bàn phím
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Tạo hướng di chuyển
        Vector3 move = new Vector3(x, 0f, z);

        // Không để nhân vật di chuyển nhanh hơn khi đi chéo
        move = Vector3.ClampMagnitude(move, 1f);

        // Di chuyển nhân vật
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Kiểm tra nhân vật có đang đứng trên mặt đất không
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Tính trọng lực
        velocity.y += gravity * Time.deltaTime;

        // Áp dụng trọng lực
        controller.Move(velocity * Time.deltaTime);
    }
}