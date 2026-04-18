using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [Header("Skills")]
    public BaseSkills slot1;
    public BaseSkills slot2;

    [Header("Hole Groups")]
    public List<GameObject> edgeHoles;
    public List<GameObject> middleHoles;

    

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // 🎯 SPAWN HOLE
    // =========================
    public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    {
        Debug.Log("ActivateHole CALLED");

        TurnOffAllHoles();

        List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

        if (targetList == null || targetList.Count == 0)
        {
            Debug.LogError("List hole rỗng!");
            return null;
        }

        int index = Random.Range(0, targetList.Count);
        GameObject hole = targetList[index];

        hole.SetActive(true);

        // 🔥 Đặt vị trí ban đầu (ĐỨNG YÊN)
        Vector3 startPos = GetDefaultSpawnPosition(isEdge);
        hole.transform.position = startPos;

        // 🔥 Delay rồi mới cho kéo
        StartCoroutine(StartPlacingDelay(hole, isEdge));

        Debug.Log("Spawn: " + hole.name);

        HoleLogic logic = hole.GetComponent<HoleLogic>();
        if (logic != null)
        {
            logic.Init(destroyOnBallEnter);
        }

        return hole;

        //Debug.Log("ActivateHole CALLED");

        //TurnOffAllHoles();

        //List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

        //if (targetList == null || targetList.Count == 0)
        //{
        //    Debug.LogError("List hole rỗng!");
        //    return null;
        //}

        //int index = Random.Range(0, targetList.Count);
        //GameObject hole = targetList[index];

        //hole.SetActive(true);
        //HolePlacementController.Instance.StartPlacing(hole, isEdge);
        ////if (HolePlacementController.Instance != null)
        ////{
        ////    HolePlacementController.Instance.StartPlacing(hole);
        ////}
        ////else
        ////{
        ////    Debug.LogError("Thiếu HolePlacementController trong scene!");
        ////}
        //// 🔥 Đặt vị trí ban đầu trên bàn
        //Vector3 startPos = GetDefaultSpawnPosition(isEdge);
        //hole.transform.position = startPos;

        //// 🔥 delay 1s mới cho kéo
        //StartCoroutine(StartPlacingDelay(hole));

        //Debug.Log("Spawn: " + hole.name);

        //HoleLogic logic = hole.GetComponent<HoleLogic>();
        //if (logic != null)
        //{
        //    logic.Init(destroyOnBallEnter);
        //}

        //return hole; // 🔥 QUAN TRỌNG
    }

    private IEnumerator StartPlacingDelay(GameObject hole, bool isEdge)
    {
        yield return new WaitForSeconds(1f);

        if (HolePlacementController.Instance != null)
        {
            Debug.Log("Bắt đầu cho phép kéo lỗ");

            HolePlacementController.Instance.StartPlacing(hole, isEdge);
        }
        else
        {
            Debug.LogError("Thiếu HolePlacementController!");
        }
    }

    private Vector3 GetDefaultSpawnPosition(bool isEdge)
    {
        float tableY = 0.75f; // 🔥 chỉnh theo bàn bạn

        if (isEdge)
        {
            return new Vector3(0f, tableY, 1.2f); // gần băng
        }
        else
        {
            return new Vector3(0f, tableY, 0f); // giữa bàn
        }
    }

    // =========================
    // 🎯 DELAY DISABLE (FIX CHUẨN)
    // =========================
    public void DisableHoleAfterDelay(GameObject hole, float delay)
    {
        StartCoroutine(DisableCoroutine(hole, delay));
    }

    IEnumerator DisableCoroutine(GameObject hole, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hole != null && hole.activeSelf)
        {
            Debug.Log("Disable hole: " + hole.name);
            hole.SetActive(false);
        }
    }

    // =========================
    void TurnOffAllHoles()
    {
        foreach (var h in edgeHoles)
            if (h != null) h.SetActive(false);

        foreach (var h in middleHoles)
            if (h != null) h.SetActive(false);
    }

    // =========================
    public void UseSkillSlot1()
    {
        Debug.Log("Use Skill 1");

        if (slot1 != null)
            slot1.Activate(gameObject, this);
    }

    public void UseSkillSlot2()
    {
        Debug.Log("Use Skill 2");

        if (slot2 != null)
            slot2.Activate(gameObject, this);
    }

    // =========================
    public void NotifyBallStopped()
    {
        Debug.Log("=== BI ĐÃ DỪNG ===");

        if (slot1 != null) slot1.OnTurnEnd(this);
        if (slot2 != null) slot2.OnTurnEnd(this);
    }

    //public static SkillManager Instance;

    //[Header("Current Active Skills")]
    //public BaseSkills slot1;
    //public BaseSkills slot2;

    //[Header("Holes Setup Sẵn")]
    //public List<GameObject> edgeHoles;     // 6 lỗ cạnh
    //public List<GameObject> middleHoles;   // 4 lỗ giữa

    //void Awake()
    //{
    //    Instance = this;
    //}

    //private void Start()
    //{
    //    TurnOffAllHoles();
    //}

    //// =========================
    //// SKILL
    //// =========================

    //public void LoadCharacterSkills(BaseSkills s1, BaseSkills s2)
    //{
    //    slot1 = s1;
    //    slot2 = s2;
    //}

    //public void UseSkillSlot1()
    //{
    //    if (slot1) slot1.Activate(this.gameObject, this);

    //    // TEST: bật lỗ cạnh
    //    ActivateRandomHole(true);
    //}

    //public void UseSkillSlot2()
    //{
    //    if (slot2) slot2.Activate(this.gameObject, this);

    //    // TEST: bật lỗ giữa
    //    ActivateRandomHole(false);
    //}

    //public void NotifyBallStopped()
    //{
    //    if (slot1) slot1.OnTurnEnd(this);
    //    if (slot2) slot2.OnTurnEnd(this);
    //}

    //// =========================
    //// 🎯 LOGIC MỚI (KHÔNG SPAWN)
    //// =========================

    //public void ActivateRandomHole(bool isEdge)
    //{
    //    List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

    //    if (targetList == null || targetList.Count == 0)
    //    {
    //        Debug.LogError("List rỗng!");
    //        return;
    //    }

    //    // ✅ chỉ tắt nhóm này
    //    TurnOffGroup(targetList);

    //    int index = Random.Range(0, targetList.Count);
    //    GameObject selectedHole = targetList[index];

    //    if (selectedHole.transform.parent != null)
    //    {
    //        selectedHole.transform.parent.gameObject.SetActive(true);
    //    }

    //    selectedHole.SetActive(true);

    //    Debug.Log("Bật: " + selectedHole.name);

    //    //List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

    //    //if (targetList == null || targetList.Count == 0)
    //    //{
    //    //    Debug.LogWarning("Danh sách lỗ rỗng!");
    //    //    return;
    //    //}

    //    //// TẮT HẾT
    //    //foreach (GameObject hole in targetList)
    //    //{
    //    //    if (hole != null)
    //    //        hole.SetActive(false);
    //    //}

    //    //// RANDOM
    //    //int randomIndex = Random.Range(0, targetList.Count);

    //    //GameObject selectedHole = targetList[randomIndex];

    //    //// BẬT LỖ
    //    //if (selectedHole != null)
    //    //{
    //    //    selectedHole.SetActive(true);
    //    //    Debug.Log("Đã bật lỗ: " + selectedHole.name);
    //    //}

    //    //Debug.Log("ActivateRandomHole chạy");
    //}

    //// =========================
    //// 🧪 BUTTON TEST
    //// =========================

    //private void TurnOffGroup(List<GameObject> list)
    //{
    //    foreach (var hole in list)
    //    {
    //        if (hole != null)
    //            hole.SetActive(false);
    //    }
    //}

    //private void TurnOffAllHoles()
    //{
    //    foreach (var hole in edgeHoles)
    //    {
    //        if (hole != null)
    //            hole.SetActive(false);
    //    }

    //    foreach (var hole in middleHoles)
    //    {
    //        if (hole != null)
    //            hole.SetActive(false);
    //    }
    //}

    //public void ActivateHoleWithLogic(bool isEdge, bool destroyOnBallEnter, float autoDisableTime)
    //{
    //    List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

    //    if (targetList == null || targetList.Count == 0)
    //    {
    //        Debug.LogError("List rỗng!");
    //        return;
    //    }

    //    TurnOffGroup(targetList);

    //    int index = Random.Range(0, targetList.Count);
    //    GameObject selectedHole = targetList[index];

    //    selectedHole.SetActive(true);

    //    HoleLogic logic = selectedHole.GetComponent<HoleLogic>();

    //    if (logic != null)
    //    {
    //        logic.Init(destroyOnBallEnter);

    //        // 🔥 chỉ auto nếu có time
    //        if (autoDisableTime > 0)
    //        {
    //            logic.AutoDisable(autoDisableTime);
    //        }
    //    }
    //}

    //public void DisableHolesAfterDelay(bool isEdge, float delay)
    //{
    //    List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

    //    foreach (var hole in targetList)
    //    {
    //        if (hole != null && hole.activeSelf)
    //        {
    //            HoleLogic logic = hole.GetComponent<HoleLogic>();
    //            if (logic != null)
    //            {
    //                logic.AutoDisable(delay);
    //            }
    //        }
    //    }
    //}



    //public void OnClickSpawnEdgeHole()
    //{
    //    ActivateRandomHole(true);
    //}

    //public void OnClickSpawnMiddleHole()
    //{
    //    ActivateRandomHole(false);
    //}

    //public static SkillManager Instance;

    //[Header("Current Active Skills")]
    //public BaseSkills slot1;
    //public BaseSkills slot2;

    //[Header("Spawn Positions")]
    //public List<Transform> edgeHolePoints;   // 6 lỗ cạnh bàn
    //public List<Transform> middleHolePoints; // 4 vị trí trên bàn

    //[Header("Prefab (Test Spawn)")]
    //public GameObject holePrefab; // dùng để test spawn nhanh

    //void Awake()
    //{
    //    Instance = this;
    //}

    //// Nạp skill
    //public void LoadCharacterSkills(BaseSkills s1, BaseSkills s2)
    //{
    //    slot1 = s1;
    //    slot2 = s2;
    //}

    //public void UseSkillSlot1()
    //{
    //    if (slot1) slot1.Activate(this.gameObject, this);
    //}

    //public void UseSkillSlot2()
    //{
    //    if (slot2) slot2.Activate(this.gameObject, this);
    //}

    //public void NotifyBallStopped()
    //{
    //    if (slot1) slot1.OnTurnEnd(this);
    //    if (slot2) slot2.OnTurnEnd(this);
    //}

    //// =========================
    //// ✅ FIX CHÍNH Ở ĐÂY
    //// =========================

    ///// <summary>
    ///// Lấy Transform random (CHUẨN - không bị lệch)
    ///// </summary>
    //public Transform GetRandomSpawnPoint(bool isEdgeHole)
    //{
    //    List<Transform> targetList = isEdgeHole ? edgeHolePoints : middleHolePoints;

    //    if (targetList == null || targetList.Count == 0)
    //    {
    //        Debug.LogWarning("Spawn list is EMPTY!");
    //        return null;
    //    }

    //    int randomIndex = Random.Range(0, targetList.Count);
    //    Transform spawnPoint = targetList[randomIndex];

    //    Debug.Log("Spawn tại: " + spawnPoint.name + " | Pos: " + spawnPoint.position);

    //    return spawnPoint;
    //}

    ///// <summary>
    ///// Nếu bạn vẫn muốn dùng Vector3 (giữ tương thích code cũ)
    ///// </summary>
    //public Vector3 GetRandomSpawnPos(bool isEdgeHole)
    //{
    //    Transform point = GetRandomSpawnPoint(isEdgeHole);
    //    return point != null ? point.position : Vector3.zero;
    //}

    ///// <summary>
    ///// Spawn trực tiếp (TEST nhanh trong Editor)
    ///// </summary>
    //public void SpawnHole(bool isEdgeHole)
    //{
    //    Transform spawnPoint = GetRandomSpawnPoint(isEdgeHole);

    //    if (spawnPoint == null || holePrefab == null)
    //    {
    //        Debug.LogWarning("Thiếu spawnPoint hoặc prefab!");
    //        return;
    //    }

    //    GameObject hole = Instantiate(holePrefab);

    //    // Set vị trí sau khi spawn (an toàn nhất)
    //    //hole.transform.position = spawnPoint.position;
    //    hole.transform.position = spawnPoint.TransformPoint(Vector3.zero);

    //    // Reset rotation để test
    //    hole.transform.rotation = spawnPoint.rotation;

    //    // Không parent để tránh scale lỗi
    //    hole.transform.SetParent(null);

    //    // Reset scale
    //    hole.transform.localScale = Vector3.one;

    //    Debug.Log("Spawn chuẩn tại: " + spawnPoint.position);

    //    //Transform spawnPoint = GetRandomSpawnPoint(isEdgeHole);

    //    //if (spawnPoint == null || holePrefab == null)
    //    //{
    //    //    Debug.LogWarning("Thiếu spawnPoint hoặc prefab!");
    //    //    return;
    //    //}

    //    //GameObject hole = Instantiate(holePrefab, spawnPoint.position, spawnPoint.rotation);

    //    //// QUAN TRỌNG: tránh bị lệch do parent
    //    //hole.transform.SetParent(null);

    //    //// Reset scale (tránh bị méo)
    //    //hole.transform.localScale = Vector3.one;
    //}

    //// =========================
    //// 🧪 TEST NHANH
    //// =========================
    //public void OnClickSpawnEdgeHole()
    //{
    //    SpawnHole(true);
    //}

    //public void OnClickSpawnMiddleHole()
    //{
    //    SpawnHole(false);
    //}
    //public static SkillManager Instance;

    //[Header("Current Active Skills")]
    //public BaseSkills slot1;
    //public BaseSkills slot2;

    //[Header("Spawn Positions")]
    //public List<Transform> edgeHolePoints;   // 6 lỗ cạnh bàn
    //public List<Transform> middleHolePoints; // 4 vị trí trên bàn

    //void Awake() => Instance = this;

    //// Hàm này dùng để nạp skill của nhân vật vào Manager khi bắt đầu trận
    //public void LoadCharacterSkills(BaseSkills s1, BaseSkills s2)
    //{
    //    slot1 = s1;
    //    slot2 = s2;
    //}

    //public void UseSkillSlot1() { if (slot1) slot1.Activate(this.gameObject, this); }
    //public void UseSkillSlot2() { if (slot2) slot2.Activate(this.gameObject, this); }

    //public void NotifyBallStopped()
    //{
    //    if (slot1) slot1.OnTurnEnd(this);
    //    if (slot2) slot2.OnTurnEnd(this);
    //}

    //public Vector3 GetRandomSpawnPos(bool isEdgeHole)
    //{
    //    List<Transform> targetList = isEdgeHole ? edgeHolePoints : middleHolePoints;
    //    if (targetList != null && targetList.Count > 0)
    //    {
    //        return targetList[Random.Range(0, targetList.Count)].position;
    //    }
    //    return Vector3.zero;
    //}


}
