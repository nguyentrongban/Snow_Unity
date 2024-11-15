using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target;           // Đối tượng mà camera sẽ xoay quanh (nhân vật)
    public float distance = 5f;        // Khoảng cách giữa camera và nhân vật
    public float xSpeed = 60f;        // Tốc độ xoay camera theo trục X
    public float ySpeed = 60f;        // Tốc độ xoay camera theo trục Y

    public float yMinLimit = 10f;      // Giới hạn góc nhìn thấp nhất (bằng với vị trí nhân vật)
    public float yMaxLimit = 80f;      // Giới hạn góc nhìn cao nhất theo trục Y

    private float x = 0f;              // Góc hiện tại theo trục X
    private float y = 0f;              // Góc hiện tại theo trục Y

    private Vector2 lastTouchPosition; // Vị trí chạm cuối cùng, dùng để tính toán độ vuốt
    private bool isTouching = false;   // Trạng thái chạm màn hình (để kiểm tra khi vuốt)

    private bool isJoystickMoving = false; // Kiểm tra nếu joystick đang di chuyển

    void Start()
    {

        // Giới hạn lại các giá trị để đảm bảo chúng được khởi tạo chính xác
        xSpeed = 60f;
        ySpeed = 60f;
        yMinLimit = 10f;
        yMaxLimit = 80f;

        // Lấy góc khởi tạo từ vị trí hiện tại của camera
        Vector3 angles = transform.eulerAngles;
        x = angles.y; // Xoay quanh trục Y
        y = angles.x; // Xoay từ trên xuống (trục X)
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Kiểm tra xem joystick có đang di chuyển không
            if (isJoystickMoving)
            {
                return; // Không xoay camera khi joystick di chuyển
            }

            // Kiểm tra nếu đang chạy trên thiết bị di động
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Chỉ lấy ngón tay đầu tiên

                // Kiểm tra trạng thái của ngón tay
                if (touch.phase == TouchPhase.Began)
                {
                    isTouching = true;
                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && isTouching)
                {
                    // Tính toán sự thay đổi vị trí của ngón tay khi vuốt
                    Vector2 deltaTouch = touch.position - lastTouchPosition;

                    // Giảm hệ số nhân để giảm độ nhạy
                    x += deltaTouch.x * xSpeed * 0.005f;  // Giảm tốc độ xoay
                    y -= deltaTouch.y * ySpeed * 0.005f;  // Giảm tốc độ xoay

                    // Giới hạn góc nhìn theo trục Y (trục X) để không nhìn xuyên qua mặt đất
                    y = Mathf.Clamp(y, yMinLimit, yMaxLimit); // Giới hạn góc nhìn theo trục X

                    lastTouchPosition = touch.position; // Cập nhật lại vị trí chạm
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isTouching = false;
                }
            }
            else // Dành cho PC khi dùng chuột
            {
                // Giảm tốc độ xoay khi di chuyển chuột
                x += Input.GetAxis("Mouse X") * xSpeed * 0.005f;  // Giảm tốc độ xoay
                // Không cho phép thay đổi giá trị của y bằng chuột hoặc joystick
            }

            // Giới hạn góc nhìn chỉ xoay theo trục Y (ngang) và giữ góc X cố định
            // Hạn chế góc theo trục Y để không kéo camera xuống dưới quá thấp
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            // Tính toán góc xoay của camera và vị trí mới quanh nhân vật
            Quaternion rotation = Quaternion.Euler(y, x, 0); // Xoay chỉ theo trục X và Y
            Vector3 position = rotation * new Vector3(0, 0, -distance) + target.position;

            // Cập nhật vị trí và góc nhìn của camera
            transform.rotation = rotation;
            transform.position = position;

            // Đảm bảo camera luôn nhìn vào nhân vật
            transform.LookAt(target);
        }
    }

    // Hàm để cập nhật trạng thái của joystick
    public void SetJoystickMoving(bool moving)
    {
        isJoystickMoving = moving;
    }
}
