using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HolePlacementController : MonoBehaviour
{
    public static HolePlacementController Instance;

    [Header("Layer")]
    public LayerMask tableLayer;
    public LayerMask pocketLayer; // 🔥 layer riêng cho lỗ bàn

    [Header("Distance")]
    public float minDistanceFromBall = 0.6f;
    public float minDistanceBetweenHoles = 1.0f;

    private GameObject currentHole;
    private bool isPlacing = false;
    private Renderer holeRenderer;

    public bool isEdgeMode;

    public bool IsPlacing => isPlacing;

    [Header("Table Bounds")]
    public float tableMinX = -3.5f;
    public float tableMaxX = 3.5f;
    public float tableMinZ = -1.7f;
    public float tableMaxZ = 1.7f;

    public float edgeThickness = 0.3f;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // 🎯 START PLACING
    // =========================
    public void StartPlacing(GameObject hole, bool isEdge)
    {
        currentHole = hole;
        isEdgeMode = isEdge;

        holeRenderer = hole.GetComponent<Renderer>();

        // 🔥 FIX MATERIAL INSTANCE
        holeRenderer.material = new Material(holeRenderer.material);

        StartCoroutine(DelayEnablePlacing());
    }

    IEnumerator DelayEnablePlacing()
    {
        isPlacing = false;
        yield return new WaitForSeconds(0.2f);
        isPlacing = true;
    }

    // =========================
    void Update()
    {
        if (!isPlacing || currentHole == null) return;

        FollowMouse();
        UpdateColor();

        // 🔥 CLICK ĐẶT
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsValidPosition())
            {
                Debug.Log("❌ Vị trí không hợp lệ");
                return;
            }

            Debug.Log("✅ Đặt lỗ thành công");

            isPlacing = false;

            // 🔥 KHÓA LỖ (QUAN TRỌNG)
            LockHole(currentHole);

            currentHole = null;
        }
    }

    // =========================
    void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tableLayer))
        {
            float tableY = 0.9678f;

            currentHole.transform.position = new Vector3(
                hit.point.x,
                tableY,
                hit.point.z
            );
        }
    }

    // =========================
    void UpdateColor()
    {
        if (holeRenderer == null) return;

        bool valid = IsValidPosition();
        holeRenderer.material.color = valid ? Color.green : Color.red;
    }

    // =========================
    bool IsValidPosition()
    {
        if (currentHole == null) return false;

        Vector3 pos = currentHole.transform.position;

        // =====================
        // 🎯 CHECK EDGE / MIDDLE
        // =====================
        bool isInEdgeZone =
            pos.x < tableMinX + edgeThickness ||
            pos.x > tableMaxX - edgeThickness ||
            pos.z < tableMinZ + edgeThickness ||
            pos.z > tableMaxZ - edgeThickness;

        if (isEdgeMode)
        {
            if (!isInEdgeZone) return false;
        }
        else
        {
            if (isInEdgeZone) return false;
        }

        // =====================
        // 🟤 CHECK LỖ BÀN (Layer)
        // =====================
        Collider[] hits = Physics.OverlapSphere(pos, 0.6f, pocketLayer);

        if (hits.Length > 0)
        {
            Debug.Log("❌ Đè lên lỗ bàn");
            return false;
        }

        // =====================
        // 🎱 CHECK BALL
        // =====================
        BallNo[] balls = FindObjectsOfType<BallNo>();

        foreach (var b in balls)
        {
            if (!b.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(pos, b.transform.position);

            if (dist < minDistanceFromBall)
                return false;
        }

        // =====================
        // 🕳 CHECK HOLE KHÁC
        // =====================
        HoleLogic[] holes = FindObjectsOfType<HoleLogic>();

        foreach (var h in holes)
        {
            if (h.gameObject == currentHole) continue;
            if (!h.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(pos, h.transform.position);

            if (dist < minDistanceBetweenHoles)
                return false;
        }

        return true;
    }

    // =========================
    // 🔒 KHÓA LỖ SAU KHI ĐẶT
    // =========================
    void LockHole(GameObject hole)
    {
        // ❌ tắt kéo
        Collider col = hole.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // ✅ bật logic lại (ăn bi)
        HoleLogic logic = hole.GetComponent<HoleLogic>();
        if (logic != null)
        {
            logic.EnableTrigger();
        }
    }
    //public static HolePlacementController Instance;

    //public LayerMask tableLayer;
    //public float minDistanceFromBall = 0.5f;
    //public float minDistanceBetweenHoles = 1.0f;

    //private GameObject currentHole;
    //private bool isPlacing = false;
    //private Renderer holeRenderer;

    //public bool isEdgeMode;

    //public bool IsPlacing => isPlacing;

    //public float tableMinX = -2f;
    //public float tableMaxX = 2f;
    //public float tableMinZ = -1f;
    //public float tableMaxZ = 1f;

    //public float edgeThickness = 0.3f;

    ////GameObject[] tableHoles = GameObject.FindGameObjectsWithTag("TablePocket");

    //void Awake()
    //{
    //    Instance = this;
    //}

    //public void StartPlacing(GameObject hole, bool isEdge)
    //{
    //    currentHole = hole;
    //    isEdgeMode = isEdge;
    //    isPlacing = true;

    //    holeRenderer = hole.GetComponent<Renderer>();

    //    //currentHole = hole;
    //    //isPlacing = true;

    //    //holeRenderer = hole.GetComponent<Renderer>();

    //    StartCoroutine(DelayEnablePlacing());
    //}

    //private IEnumerator DelayEnablePlacing()
    //{
    //    isPlacing = false;

    //    yield return new WaitForSeconds(0.2f); // thêm buffer

    //    isPlacing = true;
    //}

    //void Update()
    //{
    //    if (!isPlacing || currentHole == null) return;

    //    FollowMouse();
    //    UpdateColor();

    //    Debug.Log("Placing: " + isPlacing);

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log("Đặt lỗ thành công");
    //        isPlacing = false;

    //        // 🔥 KÍCH HOẠT HOLE
    //        HoleLogic logic = currentHole.GetComponent<HoleLogic>();
    //        //if (logic != null)
    //        //{
    //        //    logic.ActivateHole();
    //        //}

    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            if (!IsValidPosition()) return;

    //            Debug.Log("Đặt lỗ thành công");

    //            isPlacing = false;

    //            currentHole = null; // 🔥 khóa luôn reference
    //        }
    //    }
    //}

    //void FollowMouse()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    if (Physics.Raycast(ray, out RaycastHit hit, 100f, tableLayer))
    //    {
    //        float tableY = 0.9678f;//0.75f// // 🔥 cùng giá trị với trên

    //        currentHole.transform.position = new Vector3(
    //            hit.point.x,
    //            tableY,
    //            hit.point.z
    //        );
    //    }

    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    //if (Physics.Raycast(ray, out RaycastHit hit, 100f, tableLayer))
    //    //{
    //    //    float yOffset = 0.005f; // 🔥 chỉnh số này

    //    //    currentHole.transform.position = new Vector3(
    //    //        hit.point.x,
    //    //        hit.point.y + yOffset,
    //    //        hit.point.z
    //    //    );
    //    //}
    //}

    //void UpdateColor()
    //{
    //    if (holeRenderer == null) return;

    //    bool valid = IsValidPosition();
    //    holeRenderer.material.color = valid ? Color.green : Color.red;
    //}

    //bool IsValidPosition()
    //{
    //    if (currentHole == null) return false;

    //    Vector3 pos = currentHole.transform.position;

    //    // =====================
    //    // 🎯 CHECK VÙNG
    //    // =====================

    //    bool isInEdgeZone =
    //        pos.x < tableMinX + edgeThickness ||
    //        pos.x > tableMaxX - edgeThickness ||
    //        pos.z < tableMinZ + edgeThickness ||
    //        pos.z > tableMaxZ - edgeThickness;

    //    if (isEdgeMode)
    //    {
    //        // skill 1.x → CHỈ ở edge
    //        if (!isInEdgeZone) return false;
    //    }
    //    else
    //    {
    //        // skill 2.x → CHỈ ở giữa
    //        if (isInEdgeZone) return false;
    //    }

    //    // =====================
    //    // 🔥 CHECK LỖ BÀN (OverlapSphere)
    //    // =====================

    //    Collider[] hits = Physics.OverlapSphere(pos, 0.7f);

    //    foreach (var col in hits)
    //    {
    //        if (col.CompareTag("TablePocket"))
    //        {
    //            Debug.Log("❌ Đang đè lên lỗ bàn");
    //            return false;
    //        }
    //    }

    //    // =====================
    //    // 🎯 CHECK BALL
    //    // =====================

    //    BallNo[] balls = FindObjectsOfType<BallNo>();
    //    foreach (var b in balls)
    //    {
    //        float dist = Vector3.Distance(pos, b.transform.position);

    //        // 🔥 QUAN TRỌNG: tăng khoảng cách này
    //        if (dist < 0.6f)
    //            return false;
    //    }

    //    // =====================
    //    // 🎯 CHECK HOLE KHÁC
    //    // =====================

    //    HoleLogic[] holes = FindObjectsOfType<HoleLogic>();
    //    foreach (var h in holes)
    //    {
    //        if (h.gameObject == currentHole) continue;

    //        float dist = Vector3.Distance(pos, h.transform.position);
    //        if (dist < 1f)
    //            return false;
    //    }

    //    return true;

    //if (currentHole == null) return false;

    //// check ball
    //BallNo[] balls = FindObjectsOfType<BallNo>();
    //foreach (var b in balls)
    //{
    //    float dist = Vector3.Distance(currentHole.transform.position, b.transform.position);
    //    if (dist < minDistanceFromBall)
    //        return false;
    //}

    //// check hole khác
    //HoleLogic[] holes = FindObjectsOfType<HoleLogic>();
    //foreach (var h in holes)
    //{
    //    if (h.gameObject == currentHole) continue;

    //    float dist = Vector3.Distance(currentHole.transform.position, h.transform.position);
    //    if (dist < minDistanceBetweenHoles)
    //        return false;
    //}

    //return true;
    //}
}
