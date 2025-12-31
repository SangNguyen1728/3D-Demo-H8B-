using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

public class CueStickController : MonoBehaviour
{
    [Header("Dependencies")]
    public PocketTowPs pocketManager;
    public Camera mainCamera;
    public Rigidbody cueBall;
    public List<Rigidbody> balls;
    public Slider forceSlider;
    public GameObject aimingLine;

    [Header("Settings")]
    public float mouseSensitivity = 1.5f;
    public float hitForceAmount = 30f;
    public float stickHitSpeed = 10f;
    public float stopThreshold = 0.15f;

    [Header("Logic State")]
    public bool isMoving = false;
    private bool firstCollisionDetected = false;
    private bool hitTargetBallFirst = false;
    private bool hasProcessedShot = true;

    private Transform cueStickPivot, stickTransform;
    private Vector3 lastMousePosition, stickPullBack;
    private Vector3 stickOriginalPosition;
    private float sliderHitForce;
    private bool hitPeriod = false;

    public CinemachineCamera cameraOnTop, cameraOnStick;
    public bool isOnTopCameraActive = false, isDraggingStick = false;
    private float topRotationSensitviity = 0.8f, camStickRotationSensitivity = 5f;

    public GameManager gameManager;

    void Start()
    {
        cueStickPivot = GetComponent<Transform>();
        gameManager = GetComponent<GameManager>();
        if (gameManager == null) gameManager = Object.FindFirstObjectByType<GameManager>();
        stickTransform = cueStickPivot.GetChild(0);
        stickOriginalPosition = stickTransform.localPosition;
        if (pocketManager == null) pocketManager = Object.FindFirstObjectByType<PocketTowPs>();
        SetStickVisibility(true);

        cameraOnTop.Priority = 10;
        cameraOnStick.Priority = 20;
    }

    void Update()
    {
        bool movingNow = !AreAllBallsStopped();

        if (isMoving && !movingNow) OnAllBallsStoppedAction();

        isMoving = movingNow;

        if (!isMoving && !hitPeriod)
        {
            SetStickVisibility(true);
            cueStickPivot.position = cueBall.position;
            HandleMouseInput();
        }
        else
        {
            SetStickVisibility(false);
        }
    }

    private void SetStickVisibility(bool visible)
    {
        if (stickTransform.gameObject.activeSelf != visible) stickTransform.gameObject.SetActive(visible);
        if (aimingLine != null && aimingLine.activeSelf != visible) aimingLine.SetActive(visible);
    }

    private void OnAllBallsStoppedAction()
    {
        if (hasProcessedShot) return;
        if (pocketManager != null) pocketManager.HandleStrokeResult(hitTargetBallFirst);
        hasProcessedShot = true;
        firstCollisionDetected = false;
        hitTargetBallFirst = false;
    }

    public bool AreAllBallsStopped()
    {
        // Kiểm tra và ép bi dừng nếu vận tốc quá nhỏ (Khắc phục lỗi bi trôi)
        if (CheckAndStopBall(cueBall)) return false;
        foreach (Rigidbody ball in balls)
        {
            if (ball == null) continue;
            if (CheckAndStopBall(ball)) return false;
        }
        return true;
    }

    private bool CheckAndStopBall(Rigidbody rb)
    {
        float speed = rb.linearVelocity.magnitude;
        if (speed > 0 && speed < stopThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        return speed > stopThreshold;
    }

    public void NotifyFirstCollision(GameObject hitObject)
    {
        if (firstCollisionDetected || !hitPeriod) return;
        if (hitObject.CompareTag("CueBall") || !hitObject.tag.StartsWith("BallNo.")) return;

        firstCollisionDetected = true;
        int hitBallNumber = 0;
        if (hitObject.CompareTag("BallNo.9")) hitBallNumber = 9;
        else int.TryParse(hitObject.tag.Replace("BallNo.", ""), out hitBallNumber);

        hitTargetBallFirst = (hitBallNumber == pocketManager.targetBallNumber);
    }

    private IEnumerator HitCueBall()
    {
        hitPeriod = true;
        hasProcessedShot = false;
        if (pocketManager != null) pocketManager.RegisterStartShot();

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            stickTransform.localPosition = Vector3.Lerp(stickPullBack, stickOriginalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * stickHitSpeed;
            yield return null;
        }

        cueBall.AddForce(cueStickPivot.forward * sliderHitForce, ForceMode.Impulse);
        hitPeriod = false;
    }

    public void OnSliderValueChange()
    {
        if (isMoving || hitPeriod) return;
        sliderHitForce = hitForceAmount * forceSlider.value;
        float pullDistance = forceSlider.value * 0.8f;
        stickTransform.localPosition = stickOriginalPosition + Vector3.back * pullDistance;
        stickPullBack = stickTransform.localPosition;
    }

    public void OnSliderReleased()
    {
        if (isMoving || hitPeriod) return;
        if (sliderHitForce > 0.5f)
        {
            StartCoroutine(HitCueBall());
            StartCoroutine(ResetSlider());
        }
        else
        {
            stickTransform.localPosition = stickOriginalPosition;
        }
    }

    private IEnumerator ResetSlider()
    {
        while (forceSlider.value > 0)
        {
            forceSlider.value = Mathf.MoveTowards(forceSlider.value, 0, Time.deltaTime * 2f);
            yield return null;
        }
    }

    private void HandleMouseInput()
    {
        Camera activeCamera = mainCamera;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, cueStickPivot.position);

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = GetMouseWorldPosition();

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);

                lastMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;
                isDraggingStick = true;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if(isDraggingStick)
            {
                Vector3 currentMousePosition = isOnTopCameraActive ? GetMouseWorldPosition() : Input.mousePosition;

                if(isOnTopCameraActive)
                {
                    Vector3 lastDirection = lastMousePosition - cueStickPivot.position;
                    Vector3 currentDirection = currentMousePosition - cueStickPivot.position;

                    float angle = Vector3.SignedAngle(lastMousePosition , currentMousePosition , Vector3.up);
                    cueStickPivot.Rotate(Vector3.up, angle * topRotationSensitviity, Space.World);
                }
                else
                {
                    Vector3 mouseDelta = currentMousePosition - lastMousePosition;
                    cueStickPivot.Rotate(Vector3.up, mouseDelta.x * camStickRotationSensitivity * Time.deltaTime, Space.Self);
                }

                lastMousePosition = currentMousePosition;
            }
            
        }

        if(Input.GetMouseButtonUp(0))
        {
            isDraggingStick = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, cueStickPivot.position);
        
        return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : Vector3.zero;

        
    }

    public void CameraTransition()
    {
        isOnTopCameraActive = !isOnTopCameraActive;

        if(isOnTopCameraActive)
        {
            cameraOnTop.Priority = 10;
            cameraOnStick.Priority = 1;

            if (!gameManager.UpperUIAnimator) return;
            gameManager.UpperUIAnimator.SetBool("IsIldePlace", false);
            gameManager.UpperUIAnimator.SetBool("IsGoBack", false);
        }
        else
        {
            cameraOnTop.Priority = 1;
            cameraOnStick.Priority = 10;


            if (!gameManager.UpperUIAnimator) return;
            gameManager.UpperUIAnimator.SetBool("IsIldePlace", true);
            gameManager.UpperUIAnimator.SetBool("IsGoBack", false);
        }
    }

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
