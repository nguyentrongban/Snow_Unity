using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Đối tượng mà camera sẽ theo dõi
    public Vector3 offset;        // Khoảng cách giữa camera và đối tượng

    void LateUpdate()
    {
        if (target != null)
        {
            // Chỉ thay đổi vị trí của camera mà không thay đổi góc xoay
            transform.position = target.position + offset;
            transform.LookAt(target);  // Quay camera về phía đối tượng, nhưng không xoay quanh.
        }
    }
}
