using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 5f;

    private Animator animator;
    private Rigidbody rb;

    private bool isPickingUp = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsInWater", false);
    }

    void Update()
    {
        // =========================
        // PICKUP - KHÓA DI CHUYỂN
        // =========================
        if (isPickingUp)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);

        bool isMoving = move.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // =========================
        // DI CHUYỂN
        // =========================
        if (isMoving)
        {
            float speed = isRunning ? runSpeed : walkSpeed;

            transform.Translate(
                move.normalized * speed * Time.deltaTime,
                Space.World
            );

            transform.forward = move.normalized;
        }

        // =========================
        // ANIMATION IDLE / WALK / RUN
        // =========================
        if (!isMoving)
        {
            animator.SetFloat("Speed", 0f);
        }
        else if (isRunning)
        {
            animator.SetFloat("Speed", 6f);
        }
        else
        {
            animator.SetFloat("Speed", 3f);
        }

        // =========================
        // JUMP
        // =========================
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

        // =========================
        // ATTACK
        // =========================
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }

        // =========================
        // PICKUP
        // =========================
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPickingUp = true;

            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Pickup");
        }
    }

    // Gọi hàm này khi animation Pickup kết thúc
    public void FinishPickup()
    {
        isPickingUp = false;
    }
}