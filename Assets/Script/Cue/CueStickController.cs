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
    public enum ShotState
    {
        Idle,
        Shooting,
        BallsMoving,
        WaitingEvents,
        Resolving
    }

    [Header("Dependencies")]
    public PocketTowPs pocketManager;
    public Camera mainCamera;
    public Rigidbody cueBall;
    public List<Rigidbody> balls;
    public GameObject aimingLine;
    public CueBallController englishController;
    public Transform cueStickPivot, stickTransform;

    [Header("Shot Settle")]
    public float shotSettleDelay = 1.25f;

    [Header("Settings")]
    public float hitForceAmount = 30f;
    public float stickHitSpeed = 15f;
    public float stopThreshold = 0.08f;

    private Vector3 tableMinBounds = new Vector3 (-3.5f, 0f, -1.7f),
        tableMaxBounds = new Vector3(3.5f, 0f, 1.7f);

    private float topRotationSensitivity = 0.8f, camStickRotationSensitivity = 5f;

    [Header("Logic State")]
    public bool isMoving = false;
    public bool hitPeriod = false; // Trạng thái đang thực hiện cú đánh
    private bool hasProcessedShot = true;
    private bool waitingShotResult = false;
    private bool firstCollisionDetected = false;
    private bool hitTargetBallFirst = false;

    [Header("Shot State")]
    public ShotState shotState = ShotState.Idle;

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
        //targetFinder = targetFinder.GetComponent<TargetBallFinder>();

        if (targetFinder == null)
            targetFinder = GetComponent<TargetBallFinder>();

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
        //bool movingNow = !AreAllBallsStopped();
        //if (isMoving && !movingNow) OnAllBallsStoppedAction();
        //isMoving = movingNow;

        //bool movingNow = !AreAllBallsStopped();

        //if (isMoving && !movingNow)
        //{
        //    if (waitingShotResult)
        //    {
        //        waitingShotResult = false;

        //        //StartCoroutine(WaitAndProcessShot());

        //        if (waitProcessRoutine != null)
        //        {
        //            StopCoroutine(waitProcessRoutine);
        //        }

        //        waitProcessRoutine = StartCoroutine(WaitAndProcessShot());
        //    }
        //}

        //isMoving = movingNow;

        bool movingNow = !AreAllBallsStopped();

        switch (shotState)
        {
            case ShotState.Idle:

                isMoving = false;
                break;

            case ShotState.Shooting:

                if (movingNow)
                {
                    shotState = ShotState.BallsMoving;

                    Debug.Log("STATE -> BALLS MOVING");
                }

                break;

            case ShotState.BallsMoving:

                if (!movingNow)
                {
                    shotState = ShotState.WaitingEvents;

                    Debug.Log("STATE -> WAITING EVENTS");

                    StartCoroutine(WaitAfterBallsStopped());
                }

                break;
        }

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

    private IEnumerator WaitAfterBallsStopped()
    {
        Debug.Log("WAITING EVENTS STABILIZE");

        PocketTowPs pocket = pocketManager;

        if (pocket == null)
            yield break;

        float stableTimer = 0f;

        float lastEventTime = pocket.lastBallEventTime;

        while (stableTimer < 1.0f)
        {
            // Nếu bi lăn lại
            if (!AreAllBallsStopped())
            {
                Debug.Log("BALL MOVED AGAIN");

                shotState = ShotState.BallsMoving;

                yield break;
            }

            // Nếu có event mới
            if (lastEventTime != pocket.lastBallEventTime)
            {
                lastEventTime = pocket.lastBallEventTime;

                stableTimer = 0f;

                Debug.Log("NEW BALL EVENT DETECTED");
            }
            else
            {
                // stableTimer += Time.deltaTime;
                // còn event đang xử lý
                if (pocket.HasPendingEvents())
                {
                    stableTimer = 0f;
                }
                else
                {
                    stableTimer += Time.deltaTime;
                }
            }

            yield return null;
        }

        shotState = ShotState.Resolving;

        Debug.Log("STATE -> RESOLVING");

        OnAllBallsStoppedAction();

        yield return null;
        //shotState = ShotState.Idle;
        if (shotState == ShotState.Resolving)
        {
            shotState = ShotState.Idle;
        }

        Debug.Log("<color=green>SHOT COMPLETELY RESOLVED</color>");

        Debug.Log("STATE -> IDLE");
    }

    private Coroutine waitProcessRoutine;
    //private IEnumerator WaitAndProcessShot()
    //{
    //    // Đợi toàn bộ physics settle
    //    yield return new WaitForSeconds(shotSettleDelay);

    //    // Nếu có bi lăn lại -> chờ tiếp
    //    if (!AreAllBallsStopped())
    //    {
    //        waitingShotResult = true;
    //        yield break;
    //    }

    //    // Nếu còn pending destroy -> chờ tiếp
    //    if (pocketManager != null && pocketManager.HasPendingEvents())
    //    {
    //        waitingShotResult = true;
    //        yield break;
    //    }

    //    OnAllBallsStoppedAction();
    //}

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
        

        if ( shotState != ShotState.Shooting && shotState != ShotState.BallsMoving)
        {
            return;
        }

        // Đã detect bi đầu tiên rồi
        if (firstCollisionDetected) return;

        BallNo ball = hitObject.GetComponent<BallNo>();

        // Không phải bi -> bỏ qua
        if (ball == null) return;

        // Bi trắng -> bỏ qua
        if (ball.isCueBall) return;

        // ĐÁNH DẤU ĐÃ DETECT
        firstCollisionDetected = true;

        int hitBallNumber = ball.ballNumber;

        Debug.Log("FIRST HIT BALL = " + hitBallNumber);

        if (pocketManager != null)
        {
           

            bool validFirstHit;

            if (pocketManager.gameMode ==
                PocketTowPs.PoolGameMode.NineBall)
            {
                validFirstHit =
                    (hitBallNumber ==
                    pocketManager.targetBallNumber);
            }
            else
            {
                validFirstHit =
                    pocketManager.IsValidFirstHit8Ball(hitBallNumber);
            }

            hitTargetBallFirst = validFirstHit;

            pocketManager.SetHitResult(validFirstHit);

            Debug.Log("TARGET BALL = " + pocketManager.targetBallNumber);

            Debug.Log("HIT TARGET ? " + hitTargetBallFirst);
        }
    }

    // --- LOGIC ĐÁNH BÓNG ---
    public IEnumerator HitCueBall()
    {
        hitPeriod = true;
        hasProcessedShot = false;
        if (pocketManager != null)
        {
            pocketManager.shotAlreadyResolved = false;
        }
        waitingShotResult = true;
        firstCollisionDetected = false; // Reset trước khi đánh
        hitTargetBallFirst = false;
        moveCueBallAllow = false;
        initialMoveCueBall = false;

        if (pocketManager != null)
            pocketManager.RegisterStartShot();

        shotState = ShotState.Shooting;

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

        if (GlaszekManager.Instance != null)
        {
            GlaszekManager.Instance.NotifyShotStarted();
        }

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
        if (shotState != ShotState.Resolving)
        {
            Debug.Log("NOT RESOLVING STATE");

            return;
        }

        Debug.Log("ALL BALLS STOPPED");

        // 1. Kiểm tra nếu cú đánh này đã được xử lý rồi thì bỏ qua
        //if (hasProcessedShot) return;
        if (hasProcessedShot)
        {
            Debug.Log("SHOT ALREADY PROCESSED");
            return;
        }

        Debug.Log("<color=cyan>Tất cả bi đã dừng. Đang xử lý kết quả lượt đánh...</color>");

        if (GlaszekManager.Instance != null) GlaszekManager.Instance.NotifyBallStopped();

        FixedShieldSkill shield = FindFirstObjectByType<FixedShieldSkill>();
        if (shield != null)
        {
            shield.OnTurnEnd();
        }
        // --------------------------------------------------------------------------

        //if (pocketManager != null)
        //{
        //    pocketManager.SetHitResult(hitTargetBallFirst);
        //    pocketManager.HandleStrokeResult();
        //}

        if (pocketManager != null)
        {
            //pocketManager.SetHitResult(hitTargetBallFirst);
            //pocketManager.HandleStrokeResult();

            Debug.Log("<color=cyan>Send Hit Result = "+ hitTargetBallFirst + "</color>" );

            //pocketManager.SetHitResult(hitTargetBallFirst);

            pocketManager.HandleStrokeResult();
        }

        // 2. Gửi kết quả va chạm cho PocketManager xử lý luật chơi (Foul/Valid)
        //if (pocketManager != null)
        //{
        //    pocketManager.SetHitResult(hitTargetBallFirst);
        //    pocketManager.HandleStrokeResult();
        //}

        //if (targetFinder != null)
        //{
        //    targetFinder.UpdateTargetBall();
        //}

        //if (targetFinder != null && targetFinder.currentTarget != null)
        //{
        //    PointAtTarget(targetFinder.currentTarget);
        //}

        StartCoroutine(UpdateTargetDelayed());

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
        StartCoroutine(UnlockShotProcess());
        //shotState = ShotState.Idle;
        waitingShotResult = false;
        firstCollisionDetected = false;    // Reset cảm biến va chạm cho lần sau
        //hitTargetBallFirst = false;        // Reset cờ kiểm tra mục tiêu

        stopTimer = false;

        // 6. Đảm bảo gậy hiện lại đúng vị trí bi cái (do Update sẽ lo phần visual)
        transform.position = cueBall.position;
        GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
        if (gm != null) gm.PrepareNextTurn();

        Debug.Log("<color=green>Sẵn sàng cho lượt đánh tiếp theo!</color>");

        //TriggerExplodeBalls();
    }

    private IEnumerator UnlockShotProcess()
    {
        yield return null;

        hasProcessedShot = false;
    }

    private IEnumerator UpdateTargetDelayed()
    {
        yield return new WaitForSeconds(0.05f);

        if (targetFinder != null)
        {
            targetFinder.UpdateTargetBall();
        }

        yield return null;

        if (targetFinder != null && targetFinder.currentTarget != null)
        {
            PointAtTarget(targetFinder.currentTarget);
        }
    }
    private IEnumerator ProcessShotDelayed()
    {
        if (hasProcessedShot)
            yield break;

        yield return new WaitForSeconds(0.2f);

        Debug.Log("PROCESS SHOT DELAYED");

        if (pocketManager != null)
        {
            pocketManager.SetHitResult(hitTargetBallFirst);

            pocketManager.HandleStrokeResult();
        }

        hasProcessedShot = true;

        firstCollisionDetected = false;

        hitTargetBallFirst = false;

        stopTimer = false;

        transform.position = cueBall.position;

        TriggerExplodeBalls();
    }

    private void TriggerExplodeBalls()
    {
        BallHealth[] balls = FindObjectsOfType<BallHealth>();

        foreach (BallHealth b in balls)
        {
            b.Explode();
        }
    }

    public bool AreAllBallsStopped()
    {
        if (CheckAndStopBall(cueBall)) return false;
        //foreach (Rigidbody ball in balls) { if (ball != null && CheckAndStopBall(ball)) return false; }

        for (int i = balls.Count - 1; i >= 0; i--)
        {
            Rigidbody ball = balls[i];

            // 🔥 remove null / inactive
            //if (ball == null || !ball.gameObject.activeInHierarchy)
            //{
            //    balls.RemoveAt(i);
            //    continue;
            //}

            if (ball == null)
            {
                balls.RemoveAt(i);
                continue;
            }

            if (!ball.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (CheckAndStopBall(ball))
                return false;
        }

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

}




