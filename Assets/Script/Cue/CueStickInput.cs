using UnityEngine;
using UnityEngine.EventSystems;
using PinePie.SimpleJoystick;

public class CueStickInput : MonoBehaviour
{
    [Header("Settings")]
    public float camRotationSensitivity = 50f;

    [Header("References")]
    public JoystickController englishJoystick; // Kéo Joystick UI vào đây
    private CueStickController controller;

    public float RotationDelta { get; private set; }
    public bool IsDragging { get; private set; }

    void Start() => controller = GetComponent<CueStickController>();

    void Update()
    {
        HandleEnglishLogic();

        // Nếu đang dùng Joystick thì KHÔNG cho xoay gậy/cam
        if (englishJoystick != null && englishJoystick.InputDirection != Vector2.zero)
        {
            IsDragging = false;
            RotationDelta = 0;
            return;
        }

        // Chặn input nếu chạm vào Slider hoặc nút UI khác
        if (EventSystem.current.IsPointerOverGameObject()) return;

        HandleRotationLogic();
    }

    public bool IsEnglishMode
    {
        get
        {
            // Trả về true nếu đang kéo Joystick HOẶC đang giữ phím Shift
            bool usingJoystick = englishJoystick != null && englishJoystick.InputDirection != Vector2.zero;
            bool usingShift = Input.GetKey(KeyCode.LeftShift);
            return usingJoystick || usingShift;
        }
    }

    private void HandleEnglishLogic()
    {
        if (controller.englishController == null) return;

        // Ưu tiên 1: Joystick
        if (englishJoystick != null && englishJoystick.InputDirection != Vector2.zero)
        {
            controller.englishController.SetEnglishExplicit(englishJoystick.InputDirection);
        }
        // Ưu tiên 2: Shift + Chuột
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.englishController.UpdateEnglish(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    private void HandleRotationLogic()
    {
        if (Input.GetMouseButtonDown(0)) IsDragging = true;
        if (Input.GetMouseButtonUp(0)) IsDragging = false;

        if (IsDragging)
        {
            if (controller.isOnTopCameraActive)
            {
                // Xoay Top-down bằng Raycast
                Ray ray = controller.mainCamera.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, controller.cueBall.position);
                if (plane.Raycast(ray, out float dist))
                {
                    Vector3 dir = ray.GetPoint(dist) - controller.cueBall.position;
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    controller.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
            }
            else
            {
                // Xoay FPS bằng Delta chuột
                RotationDelta = Input.GetAxis("Mouse X") * camRotationSensitivity * Time.deltaTime;
            }
        }
        else RotationDelta = 0;
    }
}
