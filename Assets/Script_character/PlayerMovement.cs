using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Tốc độ di chuyển bình thường
    public float runSpeed = 10f;  // Tốc độ khi chạy
    public float jumpForce = 5f;  // Lực nhảy
    public Transform cameraTransform; // Tham chiếu đến camera

    private Rigidbody rb;         // Tham chiếu đến Rigidbody của nhân vật
    private Vector3 movement;     // Lưu trữ vector di chuyển
    private bool isGrounded;      // Kiểm tra xem nhân vật có đang ở trên mặt đất hay không
    private Animator animator;    // Tham chiếu đến Animator của nhân vật

    // Tham chiếu đến Joystick tự tạo
    public Joystick joystick;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Lấy Rigidbody từ đối tượng nhân vật
        animator = GetComponent<Animator>();  // Lấy Animator từ đối tượng nhân vật

        // Đảm bảo rằng animation bắt đầu ở trạng thái Idle
        animator.SetBool("isWalking", false);  // Không di chuyển khi bắt đầu
        animator.SetBool("isJumping", false);  // Không nhảy khi bắt đầu
    }

    void Update()
    {
        // Lấy thông tin di chuyển từ joystick
        float moveX = joystick != null ? joystick.Horizontal : 0f;
        float moveZ = joystick != null ? joystick.Vertical : 0f;

        // Dựa trên góc của camera để xác định hướng di chuyển
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Chỉ giữ lại hướng ngang của camera, loại bỏ hướng thẳng đứng
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Tạo vector di chuyển dựa trên hướng của camera
        movement = (forward * moveZ + right * moveX).normalized;

        // Kiểm tra nếu người chơi nhấn Shift để chạy
        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        // Cập nhật giá trị Speed trong Animator
        float speed = movement.magnitude;
        animator.SetFloat("Speed", speed);  // Cập nhật Speed trong Animator

        // Kiểm tra nếu tốc độ di chuyển > 0
        if (speed > 0.1f)
        {
            animator.SetBool("isWalking", true);  // Nếu có di chuyển, bật isWalking lên true

            // Xoay nhân vật theo hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            animator.SetBool("isWalking", false); // Nếu không di chuyển, bật isWalking lên false
        }

        // Kiểm tra nhấn Space để nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            animator.SetTrigger("Jump");  // Gọi Trigger Jump khi nhảy
        }

        // Di chuyển nhân vật
        MovePlayer(currentMoveSpeed);
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật bằng cách sử dụng Rigidbody
        rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void MovePlayer(float speed)
    {
        // Cập nhật di chuyển cho nhân vật
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }

    void Jump()
    {
        // Thêm lực lên để nhảy
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Sau khi nhảy, nhân vật không còn ở trên mặt đất nữa
    }

    // Kiểm tra xem nhân vật có va chạm với mặt đất không
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))  // Kiểm tra có va chạm với mặt đất không
        {
            isGrounded = true;  // Nhân vật đã quay lại mặt đất
        }
    }
}