using UnityEngine.EventSystems;
using UnityEngine;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform background;  // Gán từ Inspector
    [SerializeField] private RectTransform handle;      // Gán từ Inspector
    private Vector2 inputVector;

    // Tham chiếu đến ThirdPersonCameraController
    public ThirdPersonCameraController cameraController;

    private void Start()
    {
        // Kiểm tra xem background và handle đã được gán trong Inspector chưa
        if (background == null || handle == null)
        {
            Debug.LogError("Background và Handle chưa được gán trong Inspector!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out pos);

        pos.x = (pos.x / background.sizeDelta.x);
        pos.y = (pos.y / background.sizeDelta.y);

        inputVector = new Vector2(pos.x * 2, pos.y * 2);
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

        handle.anchoredPosition = new Vector2(
            inputVector.x * (background.sizeDelta.x / 2),
            inputVector.y * (background.sizeDelta.y / 2));

        // Thông báo cho camera biết joystick đang di chuyển
        if (cameraController != null)
        {
            cameraController.SetJoystickMoving(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        // Thông báo cho camera biết joystick không di chuyển nữa
        if (cameraController != null)
        {
            cameraController.SetJoystickMoving(false);
        }
    }

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
}
