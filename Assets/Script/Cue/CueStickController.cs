using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class CueStickController : MonoBehaviour
{
    [Header("Dependencies")]
    public PocketTowPs pocketManager;
    public Camera mainCamera;
    public Rigidbody cueBall;
    public List<Rigidbody> balls;
    public GameObject aimingLine;
    public CueBallController englishController;
    public Transform cueStickPivot, stickTransform;

    [Header("Settings")]
    public float hitForceAmount = 30f;
    public float stickHitSpeed = 15f;
    public float stopThreshold = 0.15f;

    private Vector3 tableMinBounds = new Vector3 (-3.5f, 0f, -1.7f),
        tableMaxBounds = new Vector3(3.5f, 0f, 1.7f);

    private float topRotationSensitivity = 0.8f, camStickRotationSensitivity = 5f;

    [Header("Logic State")]
    public bool isMoving = false;
    public bool hitPeriod = false; // Trạng thái đang thực hiện cú đánh
    private bool hasProcessedShot = true;
    private bool firstCollisionDetected = false;
    private bool hitTargetBallFirst = false;

    [Header("Camera & Visuals")]
    public CinemachineCamera cameraOnTop;
    public CinemachineCamera cameraOnStick;
    public bool isOnTopCameraActive = false;
    private Transform stickVisual;
    private Vector3 stickLocalOrigin, lastMousePosition,stickPullBack;

    [Header("UI")]
    public Slider powerSlider;
    private float sliderHitForce;
    private CueStickInput inputSystem;

    public Animator powerSliderAnim;
    public TargetBallFinder targetFinder;

    public bool stopTimer = false;

    public bool initialMoveCueBall = false, moveCueBallAllow = true;

    public bool isDraggingStick = false, isDraggingCueBall = false;
    private bool allowRotateStickWhileSlider;
    void Start()
    {
        inputSystem = GetComponent<CueStickInput>();
        stickVisual = transform.GetChild(0);
        stickLocalOrigin = stickVisual.localPosition;
        targetFinder = targetFinder.GetComponent<TargetBallFinder>();
        englishController = cueBall.GetComponent<CueBallController>();

        if (pocketManager == null) pocketManager = GameObject.FindFirstObjectByType<PocketTowPs>();
        if (englishController == null) englishController = cueBall.GetComponent<CueBallController>();

        cameraOnTop.Priority = 10;
        cameraOnStick.Priority = 20;

        initialMoveCueBall = true;
        moveCueBallAllow = true;
    }

    void Update()
    {
        bool movingNow = !AreAllBallsStopped();
        if (isMoving && !movingNow) OnAllBallsStoppedAction();
        isMoving = movingNow;

        if (!isMoving && !hitPeriod)
        {
            SetStickVisibility(true);
            transform.position = cueBall.position;

            // Xoay gậy trong chế độ FPS (lấy delta từ Input System)
            if (!isOnTopCameraActive && inputSystem.IsDragging && !inputSystem.IsEnglishMode)
            {
                transform.Rotate(Vector3.up, inputSystem.RotationDelta, Space.World);
            }

            UpdateStickVisualPosition();
        }
        else if (!hitPeriod) // Nếu bóng đang lăn và không trong lúc đâm cơ thì ẩn gậy
        {
            SetStickVisibility(false);
        }
    }

    private void HandleMouseInput()
    {
        Camera activeCamera = mainCamera;

        Plane plane = new Plane(Vector3.up, cueStickPivot.position);

        if(Input.GetMouseButtonDown(0) && allowRotateStickWhileSlider)
        {
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

            if(plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);

                if(Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("CueBall"))
                {
                    lastMousePosition = hitPoint;
                    isDraggingCueBall = true;
                }
                else
                {
                    lastMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;
                    isDraggingStick = true;
                }
            }
        }
        
        if(Input.GetMouseButton(0))
        {
            if(isDraggingCueBall)
            {
                Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
                if(plane.Raycast(ray, out float distance))
                {
                    if(moveCueBallAllow)
                    {
                        Vector3 hitPoint = ray.GetPoint(distance);

                        float tableMinBound = initialMoveCueBall ? 2f : tableMinBounds.x;

                        float clampedX = Mathf.Clamp(hitPoint.x, tableMinBound, tableMaxBounds.x);
                        float clampedZ = Mathf.Clamp(hitPoint.z, tableMinBounds.z, tableMaxBounds.z);

                        cueBall.position = new Vector3(clampedX, cueBall.position.y, clampedZ);
                    }
                }
                else
                {
                    Debug.Log("CueBall is not ready to move");
                }
            }
        }
        else if (isDraggingStick && allowRotateStickWhileSlider)
        {
            Vector3 currentMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;
           if(isOnTopCameraActive)
            {
                Vector3 lastDirection = lastMousePosition - cueStickPivot.position;
                Vector3 currentDirection =  currentMousePosition - cueStickPivot.position;

                float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);
                cueStickPivot.Rotate(Vector3.up, angle * topRotationSensitivity, Space.World);
            }
           else
            {
                Vector3 mouseDelta = currentMousePosition - lastMousePosition;
                cueStickPivot.Rotate(Vector3.up, mouseDelta.x * camStickRotationSensitivity * Time.deltaTime, Space.World);
            }

            lastMousePosition = currentMousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            isDraggingStick = false;
            isDraggingCueBall = false;
        }
    }

    private void AdjustStickPivotToCueBall()
    {
        if(AreAllBallsStopped())
        {
            cueStickPivot.position = Vector3.MoveTowards(cueStickPivot.position, cueBall.position, Time.deltaTime);

            if(allowRotateStickWhileSlider)
            {
                stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickLocalOrigin, Time.deltaTime);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, cueStickPivot.position);
        return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : Vector3.zero;
    }

    private void UpdateStickVisualPosition()
    {
        Vector3 englishOffset = (englishController != null) ? englishController.GetHitOffset(transform) : Vector3.zero;
        float pullBack = (powerSlider != null) ? powerSlider.value * 0.8f : 0f;
        stickVisual.localPosition = stickLocalOrigin + (Vector3.back * pullBack) + transform.InverseTransformDirection(englishOffset);
    }

    // --- HÀM XỬ LÝ VA CHẠM (NOTIFY FIRST COLLISION) ---
    public void NotifyFirstCollision(GameObject hitObject)
    {
        // Chỉ xử lý nếu đang trong cú đánh và chưa phát hiện va chạm đầu tiên
        if (firstCollisionDetected || !hitPeriod) return;

        // Bỏ qua nếu chạm vào chính bi cái hoặc vật không phải bi mục tiêu
        if (hitObject.CompareTag("CueBall") || !hitObject.tag.StartsWith("BallNo.")) return;

        firstCollisionDetected = true;
        int hitBallNumber = 0;

        if (hitObject.CompareTag("BallNo.9")) hitBallNumber = 9;
        else
        {
            string num = hitObject.tag.Replace("BallNo.", "");
            int.TryParse(num, out hitBallNumber);
        }

        // Kiểm tra xem bi chạm trúng có phải bi mục tiêu hiện tại của luật chơi không
        if (pocketManager != null)
        {
            hitTargetBallFirst = (hitBallNumber == pocketManager.targetBallNumber);
        }
    }

    // --- LOGIC ĐÁNH BÓNG ---
    public IEnumerator HitCueBall()
    {
        hitPeriod = true;
        hasProcessedShot = false;
        firstCollisionDetected = false; // Reset trước khi đánh
        hitTargetBallFirst = false;
        moveCueBallAllow = false;
        initialMoveCueBall = false;

        if (pocketManager != null) pocketManager.RegisterStartShot();

        // 1. Hiệu ứng gậy đâm vào
        Vector3 englishOffset = (englishController != null) ? englishController.GetHitOffset(transform) : Vector3.zero;
        Vector3 hitPositionInLocal = stickLocalOrigin + transform.InverseTransformDirection(englishOffset);

        float t = 0;
        Vector3 startPos = stickVisual.localPosition;
        while (t < 1f)
        {
            stickVisual.localPosition = Vector3.Lerp(startPos, hitPositionInLocal, t);
            t += Time.deltaTime * stickHitSpeed;
            yield return null;
        }

        // 2. Tác động lực vật lý
        Vector3 worldHitPoint = cueBall.position + englishOffset;
        worldHitPoint -= transform.forward * (englishController != null ? englishController.ballRadius : 0.0285f);
        cueBall.AddForceAtPosition(transform.forward * sliderHitForce, worldHitPoint, ForceMode.Impulse);

        // Chờ một chút để bi cái rời gậy trước khi kết thúc "hitPeriod" visual
        yield return new WaitForSeconds(0.1f);
        hitPeriod = false;
    }

    // --- CÁC HÀM UI & LOGIC DỪNG BÓNG ---
    public void OnSliderValueChange()
    {
        if (isMoving || hitPeriod) return;
        sliderHitForce = hitForceAmount * powerSlider.value;
    }

    public void OnSliderReleased()
    {
        if (isMoving || hitPeriod) return;
        if (sliderHitForce > 0.5f)
        { StartCoroutine(HitCueBall());
          StartCoroutine(ResetSlider());
          stopTimer = true;
        }
    }

    private IEnumerator ResetSlider()
    {
        while (powerSlider.value > 0) { powerSlider.value = Mathf.MoveTowards(powerSlider.value, 0, Time.deltaTime * 2f); yield return null; }
    }

    public void CameraTransition()
    {
        isOnTopCameraActive = !isOnTopCameraActive;
        cameraOnTop.Priority = isOnTopCameraActive ? 20 : 1;
        cameraOnStick.Priority = isOnTopCameraActive ? 1 : 20;
    }

    private void OnAllBallsStoppedAction()
    {
        //if (hasProcessedShot) return;
        //if (pocketManager != null) pocketManager.HandleStrokeResult(hitTargetBallFirst);
        //hasProcessedShot = true;

        // 1. Kiểm tra nếu cú đánh này đã được xử lý rồi thì bỏ qua
        if (hasProcessedShot) return;

        Debug.Log("<color=cyan>Tất cả bi đã dừng. Đang xử lý kết quả lượt đánh...</color>");

        if (SkillManager.Instance != null) SkillManager.Instance.NotifyBallStopped();
        // --------------------------------------------------------------------------

        //if (pocketManager != null)
        //{
        //    pocketManager.SetHitResult(hitTargetBallFirst);
        //    pocketManager.HandleStrokeResult();
        //}

        if (pocketManager != null)
        {
            pocketManager.SetHitResult(hitTargetBallFirst);
            pocketManager.HandleStrokeResult();
        }

        // 2. Gửi kết quả va chạm cho PocketManager xử lý luật chơi (Foul/Valid)
        if (pocketManager != null)
        {
            pocketManager.SetHitResult(hitTargetBallFirst);
            pocketManager.HandleStrokeResult();
        }

        // 3. RESET ÉP PHÊ (ENGLISH) VỀ TÂM BI
        if (englishController != null)
        {
            englishController.ResetEnglish();
            Debug.Log("Đã reset SpinValues về (0,0).");
        }

        // 4. RESET JOYSTICK UI (Nếu đang dùng Joystick)
        if (inputSystem != null && inputSystem.englishJoystick != null)
        {
            // Ép cái núm điều khiển về vị trí chính giữa UI
            inputSystem.englishJoystick.handle.anchoredPosition = Vector2.zero;

            // Nếu bạn muốn Joystick ẩn đi sau khi đánh, có thể thêm dòng dưới:
            //inputSystem.englishJoystick.joystickBase.gameObject.SetActive(false);
        }

        // 5. Cập nhật trạng thái để chuẩn bị cho lượt đánh tiếp theo
        hasProcessedShot = true;           // Đánh dấu đã xử lý xong shot này
        firstCollisionDetected = false;    // Reset cảm biến va chạm cho lần sau
        hitTargetBallFirst = false;        // Reset cờ kiểm tra mục tiêu

        // 6. Đảm bảo gậy hiện lại đúng vị trí bi cái (do Update sẽ lo phần visual)
        transform.position = cueBall.position;
        GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
        if (gm != null) gm.PrepareNextTurn();

        Debug.Log("<color=green>Sẵn sàng cho lượt đánh tiếp theo!</color>");
    }

    public bool AreAllBallsStopped()
    {
        if (CheckAndStopBall(cueBall)) return false;
        foreach (Rigidbody ball in balls) { if (ball != null && CheckAndStopBall(ball)) return false; }
        return true;
    }

    private bool CheckAndStopBall(Rigidbody rb)
    {
        float speed = rb.linearVelocity.magnitude;
        if (speed > 0 && speed < stopThreshold) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
        return speed > stopThreshold;
    }

    private void SetStickVisibility(bool visible)
    {
        if (stickVisual.gameObject.activeSelf != visible) stickVisual.gameObject.SetActive(visible);
        if (aimingLine != null && aimingLine.activeSelf != visible) aimingLine.SetActive(visible);
    }

    public void PointAtTarget(Transform target)
    {
        if (target == null || cueBall == null) return;

        // Tính toán hướng từ bi cái đến bi mục tiêu
        Vector3 direction = target.position - cueBall.position;
        direction.y = 0; // Giữ hướng xoay trên mặt phẳng ngang

        if (direction != Vector3.zero)
        {
            // Xoay gậy hướng về bi mục tiêu
            transform.rotation = Quaternion.LookRotation(direction);
            Debug.Log($"Gậy đã tự động hướng vào bi số: {pocketManager.targetBallNumber}</color>");
        }
    }

    //[Header("Dependencies")]
    //public PocketTowPs pocketManager;
    //public Camera mainCamera;
    //public Rigidbody cueBall;
    //public List<Rigidbody> balls;
    //public GameObject aimingLine;
    //public CueBallEnglish englishController;

    //[Header("Settings")]
    //public float hitForceAmount = 30f;
    //public float stickHitSpeed = 15f;
    //public float stopThreshold = 0.15f;

    //[Header("Logic State")]
    //public bool isMoving = false;
    //private bool hitPeriod = false;
    //private bool hasProcessedShot = true;
    //private bool firstCollisionDetected = false;
    //private bool hitTargetBallFirst = false;

    //[Header("Visuals")]
    //private Transform stickTransform;
    //private Vector3 stickOriginalPosition;

    //[Header("Camera System")]
    //public CinemachineCamera cameraOnTop;
    //public CinemachineCamera cameraOnStick;
    //public bool isOnTopCameraActive = false;

    //[Header("UI")]
    //public Slider powerSlider;
    //private float sliderHitForce;

    //private CueStickInput inputSystem;
    //public Animator powerSliderAnim;

    //void Start()
    //{
    //    inputSystem = GetComponent<CueStickInput>();
    //    stickTransform = transform.GetChild(0);
    //    stickOriginalPosition = stickTransform.localPosition;

    //    if (pocketManager == null) pocketManager = GameObject.FindFirstObjectByType<PocketTowPs>();
    //    if (englishController == null) englishController = cueBall.GetComponent<CueBallEnglish>();

    //    // Khởi tạo Camera ban đầu
    //    cameraOnTop.Priority = 10;
    //    cameraOnStick.Priority = 20;
    //}

    //void Update()
    //{
    //    bool movingNow = !AreAllBallsStopped();
    //    if (isMoving && !movingNow) OnAllBallsStoppedAction();
    //    isMoving = movingNow;

    //    if (!isMoving && !hitPeriod)
    //    {
    //        SetStickVisibility(true);
    //        transform.position = cueBall.position;

    //        // Áp dụng xoay từ Input System (Chỉ áp dụng cho chế độ FPS)
    //        if (!isOnTopCameraActive && inputSystem.IsDragging && !inputSystem.IsEnglishMode)
    //        {
    //            transform.Rotate(Vector3.up, inputSystem.RotationDelta, Space.World);
    //        }

    //        UpdateStickVisualPosition();
    //    }
    //    else
    //    {
    //        SetStickVisibility(false);
    //    }
    //}

    //private void UpdateStickVisualPosition()
    //{
    //    // Tính toán vị trí offset do English và lực kéo slider
    //    Vector3 englishOffset = (englishController != null) ? englishController.GetHitOffset(transform) : Vector3.zero;
    //    float pullDistance = (powerSlider != null) ? powerSlider.value * 0.8f : 0f;

    //    // Vị trí gậy = gốc + Kéo về sau + Lệch do English
    //    stickTransform.localPosition = stickOriginalPosition + (Vector3.back * pullDistance) + transform.InverseTransformDirection(englishOffset);
    //}

    //public IEnumerator HitCueBall()
    //{
    //    hitPeriod = true;
    //    hasProcessedShot = false;
    //    if (pocketManager != null) pocketManager.RegisterStartShot();

    //    // 1. Hiệu ứng gậy đâm vào bóng
    //    Vector3 englishOffset = (englishController != null) ? englishController.GetHitOffset(transform) : Vector3.zero;
    //    Vector3 targetHitPos = stickOriginalPosition + transform.InverseTransformDirection(englishOffset);

    //    float t = 0;
    //    Vector3 startPos = stickTransform.localPosition;
    //    while (t < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(startPos, targetHitPos, t);
    //        t += Time.deltaTime * stickHitSpeed;
    //        yield return null;
    //    }

    //    // 2. Tác động lực vật lý
    //    Vector3 hitPoint = cueBall.position + englishOffset;
    //    hitPoint -= transform.forward * (englishController != null ? englishController.ballRadius : 0.0285f);
    //    cueBall.AddForceAtPosition(transform.forward * sliderHitForce, hitPoint, ForceMode.Impulse);

    //    hitPeriod = false;
    //}

    //// --- XỬ LÝ SLIDER ---
    //public void OnSliderValueChange()
    //{
    //    if (isMoving || hitPeriod) return;
    //    sliderHitForce = hitForceAmount * powerSlider.value;
    //}

    //public void OnSliderReleased()
    //{
    //    if (isMoving || hitPeriod) return;
    //    if (sliderHitForce > 0.5f)
    //    {
    //        StartCoroutine(HitCueBall());
    //        StartCoroutine(ResetSlider());
    //    }
    //}

    //private IEnumerator ResetSlider()
    //{
    //    while (powerSlider.value > 0)
    //    {
    //        powerSlider.value = Mathf.MoveTowards(powerSlider.value, 0, Time.deltaTime * 2f);
    //        yield return null;
    //    }
    //}

    //// --- HỆ THỐNG CAMERA ---
    //public void CameraTransition()
    //{
    //    isOnTopCameraActive = !isOnTopCameraActive;
    //    cameraOnTop.Priority = isOnTopCameraActive ? 20 : 1;
    //    cameraOnStick.Priority = isOnTopCameraActive ? 1 : 20;
    //}

    //// --- KIỂM TRA TRẠNG THÁI BI ---
    //public bool AreAllBallsStopped()
    //{
    //    if (CheckAndStopBall(cueBall)) return false;
    //    foreach (Rigidbody ball in balls) { if (ball != null && CheckAndStopBall(ball)) return false; }
    //    return true;
    //}

    //private bool CheckAndStopBall(Rigidbody rb)
    //{
    //    float speed = rb.linearVelocity.magnitude;
    //    if (speed > 0 && speed < stopThreshold) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    //    return speed > stopThreshold;
    //}

    //private void OnAllBallsStoppedAction()
    //{
    //    if (hasProcessedShot) return;
    //    if (pocketManager != null) pocketManager.HandleStrokeResult(hitTargetBallFirst);
    //    hasProcessedShot = true; firstCollisionDetected = false; hitTargetBallFirst = false;
    //}

    //private void SetStickVisibility(bool visible)
    //{
    //    if (stickTransform.gameObject.activeSelf != visible) stickTransform.gameObject.SetActive(visible);
    //    if (aimingLine != null && aimingLine.activeSelf != visible) aimingLine.SetActive(visible);
    //}

    //public void NotifyFirstCollision(GameObject hitObject)
    //{
    //    // Nếu đã va chạm rồi hoặc đang không trong quá trình đánh bóng thì bỏ qua
    //    if (firstCollisionDetected || !hitPeriod) return;

    //    // Bỏ qua nếu va chạm với chính bi cái hoặc vật thể không phải bi mục tiêu
    //    if (hitObject.CompareTag("CueBall") || !hitObject.tag.StartsWith("BallNo.")) return;

    //    firstCollisionDetected = true;

    //    int hitBallNumber = 0;
    //    // Xử lý riêng cho bi số 9 hoặc bóc tách số từ Tag (ví dụ: "BallNo.1" -> 1)
    //    if (hitObject.CompareTag("BallNo.9"))
    //    {
    //        hitBallNumber = 9;
    //    }
    //    else
    //    {
    //        string numberPart = hitObject.tag.Replace("BallNo.", "");
    //        int.TryParse(numberPart, out hitBallNumber);
    //    }

    //    // Kiểm tra xem bi chạm đầu tiên có đúng là bi mục tiêu không
    //    if (pocketManager != null)
    //    {
    //        hitTargetBallFirst = (hitBallNumber == pocketManager.targetBallNumber);
    //        Debug.Log($"Đã chạm bi: {hitBallNumber}. Đúng mục tiêu: {hitTargetBallFirst}");
    //    }
    //}

    //[Header("Dependencies")]
    //public PocketTowPs pocketManager;
    //public Camera mainCamera;
    //public Rigidbody cueBall;
    //public List<Rigidbody> balls;
    //public GameObject aimingLine;
    //public CueBallEnglish englishController; // Kéo CueBall vào đây

    //[Header("Settings")]
    //public float mouseSensitivity = 1.5f;
    //public float hitForceAmount = 30f;
    //public float stickHitSpeed = 10f;
    //public float stopThreshold = 0.15f;

    //[Header("Logic State")]
    //public bool isMoving = false;
    //private bool firstCollisionDetected = false;
    //private bool hitTargetBallFirst = false;
    //private bool hasProcessedShot = true;

    //private Transform cueStickPivot, stickTransform;
    //private Vector3 lastMousePosition;
    //private Vector3 stickOriginalPosition;
    //private float sliderHitForce;
    //private bool hitPeriod = false;

    //[Header("Camera")]
    //public CinemachineCamera cameraOnTop;
    //public CinemachineCamera cameraOnStick;
    //public bool isOnTopCameraActive = false;
    //private bool isDraggingStick = false;
    //public float camStickRotationSensitivity = 50f;

    //[Header("Slider")]
    //public Slider powerSlider;
    //[NonSerialized] public Animator powerSliderAnim;
    //public GameManager gameManager;

    //void Start()
    //{
    //    cueStickPivot = transform;
    //    if (gameManager == null) gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
    //    if (englishController == null) englishController = cueBall.GetComponent<CueBallEnglish>();

    //    stickTransform = cueStickPivot.GetChild(0);
    //    powerSliderAnim = powerSlider.GetComponent<Animator>();
    //    stickOriginalPosition = stickTransform.localPosition;

    //    if (pocketManager == null) pocketManager = UnityEngine.Object.FindFirstObjectByType<PocketTowPs>();
    //    SetStickVisibility(true);

    //    // Khởi tạo Camera
    //    cameraOnTop.Priority = 10;
    //    cameraOnStick.Priority = 20;
    //}

    //void Update()
    //{
    //    bool movingNow = !AreAllBallsStopped();
    //    if (isMoving && !movingNow) OnAllBallsStoppedAction();
    //    isMoving = movingNow;

    //    if (!isMoving && !hitPeriod)
    //    {
    //        SetStickVisibility(true);
    //        cueStickPivot.position = cueBall.position;

    //        // Cập nhật vị trí Visual của cơ dựa trên English
    //        Vector3 offset = (englishController != null) ? englishController.GetHitOffset(cueStickPivot) : Vector3.zero;
    //        stickTransform.localPosition = stickOriginalPosition + cueStickPivot.InverseTransformDirection(offset);

    //        HandleMouseInput();
    //    }
    //    else
    //    {
    //        SetStickVisibility(false);
    //    }


    //}

    //private void HandleMouseInput()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject()) return;

    //    float mouseX = Input.GetAxis("Mouse X");
    //    float mouseY = Input.GetAxis("Mouse Y");

    //    // CHẾ ĐỘ 1: CHỈNH ENGLISH (Giữ Shift)
    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        if (englishController != null)
    //            englishController.UpdateEnglish(mouseX, mouseY);
    //        return;
    //    }

    //    // CHẾ ĐỘ 2: XOAY NGẮM (Chuột trái)
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        lastMousePosition = Input.mousePosition;
    //        isDraggingStick = true;
    //    }

    //    if (Input.GetMouseButton(0) && isDraggingStick)
    //    {
    //        if (isOnTopCameraActive)
    //        {
    //            // Top-down rotation: Xoay dựa trên hướng chuột so với bi
    //            Vector3 currentWorldPos = GetMouseWorldPosition();
    //            Vector3 direction = currentWorldPos - cueStickPivot.position;
    //            if (direction != Vector3.zero)
    //            {
    //                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    //                cueStickPivot.rotation = Quaternion.Euler(0, angle, 0);
    //            }
    //        }
    //        else
    //        {
    //            // FPS rotation: Xoay theo Delta X của chuột
    //            float rotation = mouseX * camStickRotationSensitivity * Time.deltaTime;
    //            cueStickPivot.Rotate(Vector3.up, rotation, Space.World);
    //        }
    //    }

    //    if (Input.GetMouseButtonUp(0)) isDraggingStick = false;
    //}

    //private IEnumerator HitCueBall()
    //{
    //    hitPeriod = true;
    //    hasProcessedShot = false;
    //    if (pocketManager != null) pocketManager.RegisterStartShot();

    //    // Tính điểm chạm thực tế để áp dụng lực xoáy
    //    Vector3 hitPoint = cueBall.position + (englishController != null ? englishController.GetHitOffset(cueStickPivot) : Vector3.zero);
    //    hitPoint -= cueStickPivot.forward * (englishController != null ? englishController.ballRadius : 0.0285f);

    //    // Hiệu ứng thụt cơ
    //    float elapsedTime = 0f;
    //    Vector3 pullPos = stickTransform.localPosition;
    //    Vector3 targetPos = stickOriginalPosition + cueStickPivot.InverseTransformDirection(englishController.GetHitOffset(cueStickPivot));

    //    while (elapsedTime < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(pullPos, targetPos, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickHitSpeed;
    //        yield return null;
    //    }

    //    // TÁC ĐỘNG VẬT LÝ
    //    cueBall.AddForceAtPosition(cueStickPivot.forward * sliderHitForce, hitPoint, ForceMode.Impulse);

    //    hitPeriod = false;
    //}

    //// --- CÁC HÀM CÒN LẠI GIỮ NGUYÊN NHƯ CŨ ---
    //public void OnSliderValueChange()
    //{
    //    if (isMoving || hitPeriod) return;
    //    sliderHitForce = hitForceAmount * powerSlider.value;
    //    float pullDistance = powerSlider.value * 0.8f;
    //    stickTransform.localPosition = (stickOriginalPosition + Vector3.back * pullDistance) + cueStickPivot.InverseTransformDirection(englishController.GetHitOffset(cueStickPivot));
    //}

    //public void OnSliderReleased()
    //{
    //    if (isMoving || hitPeriod) return;
    //    if (sliderHitForce > 0.5f) { StartCoroutine(HitCueBall()); StartCoroutine(ResetSlider()); }
    //    else { stickTransform.localPosition = stickOriginalPosition + cueStickPivot.InverseTransformDirection(englishController.GetHitOffset(cueStickPivot)); }
    //}

    //private IEnumerator ResetSlider()
    //{
    //    while (powerSlider.value > 0) { powerSlider.value = Mathf.MoveTowards(powerSlider.value, 0, Time.deltaTime * 2f); yield return null; }
    //}

    //private Vector3 GetMouseWorldPosition()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, cueStickPivot.position);
    //    return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : Vector3.zero;
    //}

    //public bool AreAllBallsStopped()
    //{
    //    if (CheckAndStopBall(cueBall)) return false;
    //    foreach (Rigidbody ball in balls) { if (ball != null && CheckAndStopBall(ball)) return false; }
    //    return true;
    //}

    //private bool CheckAndStopBall(Rigidbody rb)
    //{
    //    float speed = rb.linearVelocity.magnitude;
    //    if (speed > 0 && speed < stopThreshold) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    //    return speed > stopThreshold;
    //}

    //private void OnAllBallsStoppedAction()
    //{
    //    if (hasProcessedShot) return;
    //    if (pocketManager != null) pocketManager.HandleStrokeResult(hitTargetBallFirst);
    //    hasProcessedShot = true; firstCollisionDetected = false; hitTargetBallFirst = false;
    //}

    //public void NotifyFirstCollision(GameObject hitObject)
    //{
    //    if (firstCollisionDetected || !hitPeriod) return;
    //    if (hitObject.CompareTag("CueBall") || !hitObject.tag.StartsWith("BallNo.")) return;
    //    firstCollisionDetected = true;
    //    int hitBallNumber = 0;
    //    if (hitObject.CompareTag("BallNo.9")) hitBallNumber = 9;
    //    else int.TryParse(hitObject.tag.Replace("BallNo.", ""), out hitBallNumber);
    //    hitTargetBallFirst = (hitBallNumber == pocketManager.targetBallNumber);
    //}

    //private void SetStickVisibility(bool visible)
    //{
    //    if (stickTransform.gameObject.activeSelf != visible) stickTransform.gameObject.SetActive(visible);
    //    if (aimingLine != null && aimingLine.activeSelf != visible) aimingLine.SetActive(visible);
    //}

    //public void CameraTransition()
    //{
    //    isOnTopCameraActive = !isOnTopCameraActive;
    //    cameraOnTop.Priority = isOnTopCameraActive ? 20 : 1;
    //    cameraOnStick.Priority = isOnTopCameraActive ? 1 : 20;
    //}



    // Old Script 16/1/2026

    //[Header("Dependencies")]
    //public PocketTowPs pocketManager;
    //public Camera mainCamera;
    //public Rigidbody cueBall;
    //public List<Rigidbody> balls;

    //public GameObject aimingLine;

    //[Header("Settings")]
    //public float mouseSensitivity = 1.5f;
    //public float hitForceAmount = 30f;
    //public float stickHitSpeed = 10f;
    //public float stickLeavingSpeed = 0.5f;
    //public float stopThreshold = 0.15f;

    //[Header("Logic State")]
    //public bool isMoving = false;
    //private bool firstCollisionDetected = false;
    //private bool hitTargetBallFirst = false;
    //private bool hasProcessedShot = true;

    //private Transform cueStickPivot, stickTransform;
    //private Vector3 lastMousePosition, stickPullBack;
    //private Vector3 stickOriginalPosition;
    //private Vector3 tableMinBounds = new Vector3(-3.5f, 0f, -1.7f),
    //    tableMaxBounds =  new Vector3(3.5f, 0f, 1.7f);

    //private float sliderHitForce;
    //private bool hitPeriod = false;

    //[Header("Camera")]
    //public CinemachineCamera cameraOnTop, cameraOnStick;
    //public bool isOnTopCameraActive = false, isDraggingStick = false, isDraggingCueBall = false, isMoveCueBallAllow = true, isInitialMoveCueBall = false;
    //private float topRotationSensitviity = 0.8f, camStickRotationSensitivity = 5f;

    //[Header("Slider")]
    //public Slider powerSlider;
    //[NonSerialized] public Animator powerSliderAnim;

    //public GameManager gameManager;
    //public CueBallEnglish englishController;
    ////public PocketTowPs PocketTowPs;


    //void Start()
    //{
    //    cueStickPivot = GetComponent<Transform>();
    //    //gameManager = GetComponent<GameManager>();
    //    //pocketManager = GetComponent<PocketTowPs>();
    //    if (gameManager == null) gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
    //    stickTransform = cueStickPivot.GetChild(0);
    //    powerSliderAnim = powerSlider.GetComponent<Animator>();

    //    stickOriginalPosition = stickTransform.localPosition;
    //    if (pocketManager == null) pocketManager = UnityEngine.Object.FindFirstObjectByType<PocketTowPs>();
    //    SetStickVisibility(true);

    //    cameraOnTop.Priority = 10;
    //    cameraOnStick.Priority = 20;

    //    isMoveCueBallAllow = true;
    //    isInitialMoveCueBall = true;
    //}

    //void Update()
    //{
    //    bool movingNow = !AreAllBallsStopped();

    //    if (isMoving && !movingNow) OnAllBallsStoppedAction();

    //    isMoving = movingNow;

    //    if (!isMoving && !hitPeriod)
    //    {
    //        SetStickVisibility(true);
    //        cueStickPivot.position = cueBall.position;
    //        HandleMouseInput();
    //    }
    //    else
    //    {
    //        SetStickVisibility(false);
    //    }
    //}

    //private void SetStickVisibility(bool visible)
    //{
    //    if (stickTransform.gameObject.activeSelf != visible) stickTransform.gameObject.SetActive(visible);
    //    if (aimingLine != null && aimingLine.activeSelf != visible) aimingLine.SetActive(visible);
    //}

    //private void OnAllBallsStoppedAction()
    //{
    //    if (hasProcessedShot) return;
    //    if (pocketManager != null) pocketManager.HandleStrokeResult(hitTargetBallFirst);
    //    hasProcessedShot = true;
    //    firstCollisionDetected = false;
    //    hitTargetBallFirst = false;
    //}

    //public bool AreAllBallsStopped()
    //{
    //    // Kiểm tra và ép bi dừng nếu vận tốc quá nhỏ (Khắc phục lỗi bi trôi)
    //    if (CheckAndStopBall(cueBall)) return false;
    //    foreach (Rigidbody ball in balls)
    //    {
    //        if (ball == null) continue;
    //        if (CheckAndStopBall(ball)) return false;
    //    }
    //    return true;
    //}

    //private bool CheckAndStopBall(Rigidbody rb)
    //{
    //    float speed = rb.linearVelocity.magnitude;
    //    if (speed > 0 && speed < stopThreshold)
    //    {
    //        rb.linearVelocity = Vector3.zero;
    //        rb.angularVelocity = Vector3.zero;
    //    }
    //    return speed > stopThreshold;
    //}

    //public void NotifyFirstCollision(GameObject hitObject)
    //{
    //    if (firstCollisionDetected || !hitPeriod) return;
    //    if (hitObject.CompareTag("CueBall") || !hitObject.tag.StartsWith("BallNo.")) return;

    //    firstCollisionDetected = true;
    //    int hitBallNumber = 0;
    //    if (hitObject.CompareTag("BallNo.9")) hitBallNumber = 9;
    //    else int.TryParse(hitObject.tag.Replace("BallNo.", ""), out hitBallNumber);

    //    hitTargetBallFirst = (hitBallNumber == pocketManager.targetBallNumber);
    //}

    //private IEnumerator HitCueBall()
    //{
    //    hitPeriod = true;
    //    hasProcessedShot = false;
    //    if (pocketManager != null) pocketManager.RegisterStartShot();

    //    float elapsedTime = 0f;
    //    while (elapsedTime < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(stickPullBack, stickOriginalPosition, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickHitSpeed;
    //        yield return null;
    //    }

    //    cueBall.AddForce(cueStickPivot.forward * sliderHitForce, ForceMode.Impulse);
    //    hitPeriod = false;
    //}

    //public void OnSliderValueChange()
    //{
    //    if (isMoving || hitPeriod) return;
    //    sliderHitForce = hitForceAmount * powerSlider.value;
    //    float pullDistance = powerSlider.value * 0.8f;
    //    stickTransform.localPosition = stickOriginalPosition + Vector3.back * pullDistance;
    //    stickPullBack = stickTransform.localPosition;
    //}

    //public void OnSliderReleased()
    //{
    //    if (isMoving || hitPeriod) return;
    //    if (sliderHitForce > 0.5f)
    //    {
    //        StartCoroutine(HitCueBall());
    //        StartCoroutine(ResetSlider());
    //    }
    //    else
    //    {
    //        stickTransform.localPosition = stickOriginalPosition;
    //    }
    //}

    //private IEnumerator ResetSlider()
    //{
    //    while (powerSlider.value > 0)
    //    {
    //        powerSlider.value = Mathf.MoveTowards(powerSlider.value, 0, Time.deltaTime * 2f);
    //        yield return null;
    //    }
    //}

    //private void HandleMouseInput()
    //{
    //    Camera activeCamera = mainCamera;
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, cueStickPivot.position);

    //    if (EventSystem.current.IsPointerOverGameObject()) return;
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        lastMousePosition = GetMouseWorldPosition();

    //        if (plane.Raycast(ray, out float distance))
    //        {
    //            Vector3 hitPoint = ray.GetPoint(distance);

    //            lastMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;
    //            isDraggingStick = true;
    //        }
    //    }
    //    if (Input.GetMouseButton(0))
    //    {
    //        if(isDraggingStick)
    //        {
    //            Vector3 currentMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;

    //            if(isOnTopCameraActive)
    //            {
    //                Vector3 lastDirection = lastMousePosition - cueStickPivot.position;
    //                Vector3 currentDirection = currentMousePosition - cueStickPivot.position;

    //                float angle = Vector3.SignedAngle(lastMousePosition , currentMousePosition , Vector3.up);
    //                cueStickPivot.Rotate(Vector3.up, angle * topRotationSensitviity, Space.World);
    //            }
    //            else
    //            {
    //                Vector3 mouseDelta = currentMousePosition - lastMousePosition;
    //                cueStickPivot.Rotate(Vector3.up, mouseDelta.x * camStickRotationSensitivity * Time.deltaTime, Space.Self);
    //            }

    //            lastMousePosition = currentMousePosition;
    //        }

    //    }

    //    if(Input.GetMouseButtonUp(0))
    //    {
    //        isDraggingStick = false;
    //    }
    //}

    //private Vector3 GetMouseWorldPosition()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, cueStickPivot.position);

    //    return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : Vector3.zero;


    //}

    //public void CameraTransition()
    //{
    //    isOnTopCameraActive = !isOnTopCameraActive;

    //    if(isOnTopCameraActive)
    //    {
    //        cameraOnTop.Priority = 10;
    //        cameraOnStick.Priority = 1;

    //        if (!gameManager.UpperUIAnimator) return;
    //        gameManager.UpperUIAnimator.SetBool("IsIldePlace", false);
    //        gameManager.UpperUIAnimator.SetBool("IsGoBack", false);
    //    }
    //    else
    //    {
    //        cameraOnTop.Priority = 1;
    //        cameraOnStick.Priority = 10;


    //        if (!gameManager.UpperUIAnimator) return;
    //        gameManager.UpperUIAnimator.SetBool("IsIldePlace", true);
    //        gameManager.UpperUIAnimator.SetBool("IsGoBack", false);
    //    }
    //}


    // Base Script 

    //public SkillSlowMotion skillManager; // 🚨 Kéo thả Game Manager vào đây! (Bắt buộc)
    //public Camera mainCamera;
    //public Rigidbody cueBall; // Bi trắng (Bắt buộc)
    //public List<Rigidbody> balls; // Danh sách các bi mục tiêu
    //public Slider forceSlider;

    //// === CÁC BIẾN CÀI ĐẶT ===
    //float mouseSensitivity = 1, hitForceAmount = 20, sliderHitForce, stickHitSpeed = 5f, stickLeavingSpeed = 0.5f;

    //// === CÁC BIẾN NỘI BỘ VÀ TRẠNG THÁI ===
    //private Transform cueStickPivot, stickTransform;
    //Vector3 lastMousePosition, stickPullBack;
    //public Vector3 stickOriginalPosition, stickFarPosition, stickHitPositon;

    //private bool allowStickToRotate, hitPeriod, allowRotateStickWhileSlider;
    //private bool skillFiredInThisShot = false; // Theo dõi skill đã được kích hoạt trong cú đánh này

    //private PocketTowPs twoPlayersPocketScript;

    //// *******************************************************************

    //void Start()
    //{
    //    cueStickPivot = GetComponent<Transform>();
    //    stickTransform = cueStickPivot.transform.GetChild(0);
    //    stickOriginalPosition = stickTransform.localPosition;
    //    allowStickToRotate = true;
    //}

    //void Update()
    //{
    //    HandleMouseInput();
    //    AdjustStickPivotToCueBalls();
    //}

    //// --- HÀM XỬ LÝ CHUYỂN ĐỘNG GẬY VÀ BI ---

    //public void AdjustStickPivotToCueBalls()
    //{
    //    // 1. Kiểm tra bi đã dừng
    //    if (AreAllBallsStopped())
    //    {
    //        // 🚨 LOGIC QUAN TRỌNG: KẾT THÚC VÀ RESET SKILL
    //        if (skillManager != null)
    //        {
    //            // A. Nếu Slow Motion đang chạy, kết thúc nó
    //            if (skillManager.skillActive)
    //            {
    //                skillManager.EndSkill();
    //                Debug.Log("<color=yellow>CUESTICK: Bi đã dừng, gọi EndSkill()!</color>");
    //            }

    //            // B. Nếu skill đã được bắn (đã được sử dụng), reset bộ đếm bi
    //            if (skillFiredInThisShot)
    //            {
    //                skillManager.ConsumeSkill(); // Reset bộ đếm bi và trạng thái sẵn sàng
    //                skillFiredInThisShot = false;
    //            }
    //        }

    //        // Khôi phục trạng thái chơi
    //        allowStickToRotate = true;
    //        hitPeriod = false;

    //        // Di chuyển Pivot về vị trí bi trắng
    //        cueStickPivot.position = Vector3.MoveTowards(cueStickPivot.position, cueBall.position, Time.deltaTime * stickHitSpeed);

    //        if (allowRotateStickWhileSlider)
    //        {
    //            stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickOriginalPosition, Time.deltaTime * 3);
    //            hitPeriod = false;
    //        }
    //    }
    //    else
    //    {
    //        allowStickToRotate = false; // Không cho xoay gậy khi bi đang di chuyển
    //    }
    //}

    //public bool AreAllBallsStopped()
    //{
    //    // Kiểm tra bi trắng
    //    if (cueBall != null && cueBall.linearVelocity.sqrMagnitude > 0.1f) return false;

    //    // Khắc phục MissingReferenceException: Kiểm tra Null cho danh sách bi mục tiêu
    //    foreach (Rigidbody ball in balls)
    //    {
    //        if (ball == null) continue; // Bỏ qua bi đã bị lỗ hủy

    //        if (ball.linearVelocity.sqrMagnitude > 0.1f)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //// --- COROUTINE THỰC HIỆN CÚ ĐÁNH ---

    //private IEnumerator HitCueBall()
    //{
    //    // 1. Gậy di chuyển vào 
    //    float elapsedTime = 0f;
    //    while (elapsedTime < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(stickPullBack, stickOriginalPosition, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickHitSpeed;
    //        yield return null;
    //    }

    //    // 2. TÁC ĐỘNG LỰC
    //    Vector3 hitDirection = cueStickPivot.forward;
    //    cueBall.AddForce(hitDirection * sliderHitForce, ForceMode.Impulse);

    //    Aiming.lineIsDisplaying = false;

    //    // 3. 💥 KÍCH HOẠT SLOW MOTION NGAY SAU CÚ ĐÁNH
    //    /*if (skillManager != null && skillManager.isSkillReadyToFire)
    //    {
    //        Debug.Log("<color=yellow>CUESTICK: Đã thấy điều kiện Sẵn sàng. Gọi TriggerSkill().</color>");
    //        skillManager.TriggerSkill(); // DÒNG GỌI HÀM KÍCH HOẠT
    //        skillFiredInThisShot = true; // Đánh dấu đã kích hoạt
    //    }*/

    //    // 4. Gậy lùi ra xa (stick leaving)
    //    elapsedTime = 0;
    //    while (elapsedTime < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(stickOriginalPosition, stickFarPosition, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickLeavingSpeed;
    //        yield return null;
    //    }

    //    hitPeriod = false;
    //    allowStickToRotate = true;

    //    StartCoroutine(DisplayLines(3f));
    //}

    //// --- CÁC HÀM XỬ LÝ INPUT VÀ UI ---

    //public void OnSliderValueChange()
    //{
    //    if (hitPeriod) return;

    //    allowStickToRotate = false;
    //    allowRotateStickWhileSlider = false;
    //    sliderHitForce = hitForceAmount * forceSlider.value;
    //    PullBackStick();
    //}
    //public void OnSliderReleased()
    //{
    //    if (hitPeriod) return;

    //    if (sliderHitForce > 0)
    //    {
    //        StartCoroutine(HitCueBall());
    //        StartCoroutine(ResetSlider());
    //    }
    //    else
    //    {
    //        allowRotateStickWhileSlider = true;
    //    }
    //}

    //private IEnumerator ResetSlider()
    //{
    //    float resetingSpeed = 0.5f;
    //    while (forceSlider.value > 0)
    //    {
    //        forceSlider.value = Mathf.MoveTowards(forceSlider.value, 0, Time.deltaTime * resetingSpeed);
    //        yield return null;
    //    }
    //}

    //private IEnumerator DisplayLines(float delay)
    //{
    //    while (!AreAllBallsStopped())
    //    {
    //        yield return null;
    //    }

    //    yield return new WaitForSeconds(delay);

    //    hitPeriod = false;
    //    Aiming.lineIsDisplaying = true;

    //    //StartCoroutine(twoPlayersPocketScript.HitMissedOrNot());
    //}

    //public void PullBackStick()
    //{
    //    Vector3 pullDirection = Vector3.back;
    //    float pullDistance = sliderHitForce / hitForceAmount;

    //    stickPullBack = stickOriginalPosition + pullDirection * pullDistance;

    //    if (sliderHitForce > 0)
    //    {
    //        stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickPullBack, Time.deltaTime * stickHitSpeed);
    //    }
    //    else
    //    {
    //        stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickOriginalPosition, Time.deltaTime * stickHitSpeed);
    //    }
    //}

    //private void HandleMouseInput()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        return;
    //    }

    //    if (Input.GetMouseButtonDown(0) && allowStickToRotate)
    //    {
    //        lastMousePosition = GetMouseWorldPosition();
    //    }
    //    if (Input.GetMouseButton(0) && allowStickToRotate)
    //    {
    //        Vector3 currentMousePosition = GetMouseWorldPosition();

    //        Vector3 lastDirection = lastMousePosition - cueStickPivot.position;
    //        Vector3 currentDirection = currentMousePosition - cueStickPivot.position;

    //        float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

    //        cueStickPivot.Rotate(Vector3.up, angle * mouseSensitivity, Space.World);

    //        lastMousePosition = currentMousePosition;
    //    }
    //}

    //public Vector3 GetMouseWorldPosition()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, cueStickPivot.position);

    //    if (plane.Raycast(ray, out float distance))
    //    {
    //        return ray.GetPoint(distance);
    //    }

    //    return Vector3.zero;
    //}

    //public SkillSlowMotion skillManager;
    //private Transform cueStickPivot, stickTransform;
    //public Camera mainCamera;
    //public Rigidbody cueBall;
    //public List<Rigidbody> balls;
    //float mouseSensitivity = 1, hitForceAmount = 20, sliderHitForce, stickHitSpeed = 5f, stickLeavingSpeed = 0.5f;

    //Vector3 lastMousePosition, stickPullBack;
    //public Vector3 stickOriginalPosition, stickFarPosition, stickHitPositon;
    //public Slider forceSlider;

    //private bool allowStickToRotate, hitPeriod, allowRotateStickWhileSlider;
    //private bool skillFiredInThisShot = false;

    ////PocketTowPs twoPlayerPocket; 
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    cueStickPivot = GetComponent<Transform>();
    //    stickTransform = cueStickPivot.transform.GetChild(0);
    //    stickOriginalPosition = stickTransform.localPosition;

    //    allowStickToRotate = true;

    //    if (skillManager == null)
    //    {
    //        Debug.LogError("CUESTICK: Thiếu Skill Manager. Hãy gán đối tượng chứa SkillSlowMotion trong Inspector.");
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    HandleMouseInput();
    //    AdjustStickPivotToCueBalls();
    //    Debug.Log(allowRotateStickWhileSlider);
    //}

    //private void HandleMouseInput()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        return;
    //    }

    //    if (Input.GetMouseButtonDown(0) && allowStickToRotate)
    //    {
    //        lastMousePosition = GetMouseWorldPosition();
    //    }
    //    if(Input.GetMouseButton(0) && allowStickToRotate)
    //    {
    //        Vector3 currentMousePosition = GetMouseWorldPosition();

    //        Vector3 lastDirection = lastMousePosition - cueStickPivot.position;
    //        Vector3 currentDirection= currentMousePosition - cueStickPivot.position;

    //        float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

    //        cueStickPivot.Rotate(Vector3.up, angle * mouseSensitivity, Space.World);

    //        lastMousePosition = currentMousePosition;
    //    }
    //}

    //public void AdjustStickPivotToCueBalls()
    //{
    //    if(AreAllBallsStopped())
    //    {

    //        if (skillFiredInThisShot && skillManager != null)
    //        {
    //            skillManager.ConsumeSkill(); // Gọi hàm reset bộ đếm bi và trạng thái sẵn sàng
    //            skillFiredInThisShot = false; // Đặt lại trạng thái bắn skill
    //        }

    //        allowStickToRotate = true;
    //        hitPeriod = false;
    //        cueStickPivot.position = Vector3.MoveTowards(cueStickPivot.position, cueBall.position, Time.deltaTime * stickHitSpeed);

    //        if(allowRotateStickWhileSlider)
    //        {
    //            stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickOriginalPosition,Time.deltaTime * 3);
    //            hitPeriod = false;
    //        }
    //    }
    //    else
    //    {
    //        allowStickToRotate = false;
    //    }


    //}

    //public void OnSliderValueChange()
    //{
    //    if (hitPeriod)
    //        return;

    //    allowStickToRotate = false;
    //    allowRotateStickWhileSlider = false;
    //    sliderHitForce = hitForceAmount * forceSlider.value;
    //    PullBackStick();
    //}
    //public void OnSliderReleased()
    //{
    //    if (hitPeriod)
    //        return;

    //    if (sliderHitForce > 0)
    //    {
    //        StartCoroutine(HitCueBall());
    //        StartCoroutine(ResetSlider());
    //    }
    //   else
    //    {
    //        allowRotateStickWhileSlider = true;
    //    }
    //}

    //private IEnumerator ResetSlider()
    //{
    //    float resetingSpeed = 0.5f;
    //    while(forceSlider.value > 0)
    //    {
    //        forceSlider.value = Mathf.MoveTowards(forceSlider.value, 0 , Time.deltaTime * resetingSpeed);
    //        yield return null;
    //    }
    //}

    //private IEnumerator HitCueBall()
    //{
    //    float elapsedTime = 0f;
    //    while(elapsedTime <1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(stickPullBack, stickOriginalPosition, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickHitSpeed;
    //        yield return null;
    //    }

    //    Vector3 hitDirection = cueStickPivot.forward;

    //    cueBall.AddForce(hitDirection * sliderHitForce, ForceMode.Impulse);

    //    if (skillManager != null && skillManager.isSkillReadyToFire)
    //    {
    //        Debug.Log("<color=yellow>CUESTICK: Điều kiện đủ! Kích hoạt Slow Motion.</color>");
    //        skillManager.TriggerSkill(); // Kích hoạt Slow Motion
    //        skillFiredInThisShot = true; // Đánh dấu đã kích hoạt
    //    }

    //    allowRotateStickWhileSlider = true;

    //    //allowStickToRotate = false;

    //    hitPeriod = true;
    //    allowRotateStickWhileSlider = true; 

    //    yield return new WaitForSeconds(5.0f);

    //    elapsedTime = 0;
    //    while( elapsedTime < 1f)
    //    {
    //        stickTransform.localPosition = Vector3.Lerp(stickOriginalPosition, stickFarPosition, elapsedTime);
    //        elapsedTime += Time.deltaTime * stickLeavingSpeed;
    //        yield return null;
    //    }

    //    hitPeriod = false;
    //    allowStickToRotate = true;
    //}
    //public void PullBackStick()
    //{
    //    Vector3 pullDirection = Vector3.back;
    //    float pullDistance = sliderHitForce / hitForceAmount;

    //    //stickPullBack = stickOriginalPosition - stickTransform.localRotation * Vector3.forward * (sliderHitForce / hitForceAmount); 
    //    stickPullBack = stickOriginalPosition + pullDirection * pullDistance;

    //    if (sliderHitForce  > 0)
    //    {
    //        stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickPullBack, Time.deltaTime * stickHitSpeed);
    //    }
    //    else
    //    {
    //        stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition,stickOriginalPosition, Time.deltaTime * stickHitSpeed);
    //    }
    //}

    //private bool AreAllBallsStopped()
    //{
    //    if (cueBall != null && cueBall.linearVelocity.sqrMagnitude > 0.1f)
    //        return false;

    //    foreach (Rigidbody ball in balls)
    //    {
    //        if (ball == null) continue;

    //        if (ball.linearVelocity.sqrMagnitude > 0.1f)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public Vector3 GetMouseWorldPosition()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, cueStickPivot.position);

    //    if (plane.Raycast(ray, out float distance))
    //    {
    //        return ray.GetPoint(distance);
    //    }

    //    return Vector3.zero;
    //}

}
