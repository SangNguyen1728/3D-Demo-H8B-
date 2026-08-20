using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHoleManager : MonoBehaviour
{
    //public static TableHoleManager Instance;

    //[Header("Hole Groups (di chuyển từ GlaszekManager sang đây)")]
    //public List<GameObject> edgeHoles;
    //public List<GameObject> middleHoles;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[TableHoleManager] Đã có 1 instance khác, hủy bản trùng.");
    //        Destroy(gameObject);
    //    }
    //}

    //public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    //{
    //    TurnOffAllHoles();

    //    List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

    //    int index = Random.Range(0, targetList.Count);
    //    GameObject hole = targetList[index];

    //    hole.SetActive(true);

    //    HoleLogic logic = hole.GetComponent<HoleLogic>();
    //    if (logic != null)
    //    {
    //        logic.Init(destroyOnBallEnter, false, 0, 1, 1);
    //    }

    //    HolePlacementController.Instance.StartPlacing(hole, isEdge);

    //    return hole;
    //}

    //public void DisableHoleAfterDelay(GameObject hole, float delay)
    //{
    //    StartCoroutine(DisableCoroutine(hole, delay));
    //}

    //private IEnumerator DisableCoroutine(GameObject hole, float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    if (hole != null && hole.activeSelf)
    //    {
    //        Debug.Log("Disable hole: " + hole.name);
    //        hole.SetActive(false);
    //    }
    //}

    //public void TurnOffAllHoles()
    //{
    //    foreach (var h in edgeHoles)
    //        if (h != null) h.SetActive(false);

    //    foreach (var h in middleHoles)
    //        if (h != null) h.SetActive(false);
    //}

    public static TableHoleManager Instance;

    [Header("Hole Groups (di chuyển từ GlaszekManager sang đây)")]
    public List<GameObject> edgeHoles;
    public List<GameObject> middleHoles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[TableHoleManager] Đã có 1 instance khác, hủy bản trùng.");
            Destroy(gameObject);
        }
    }

    public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    {
        List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

        // Ưu tiên chọn lỗ chưa active, tránh đè lên skill khác đang dùng
        List<GameObject> availableHoles = targetList.FindAll(h => h != null && !h.activeSelf);
        List<GameObject> poolToUse = availableHoles.Count > 0 ? availableHoles : targetList;

        int index = Random.Range(0, poolToUse.Count);
        GameObject hole = poolToUse[index];

        hole.SetActive(true);

        HoleLogic logic = hole.GetComponent<HoleLogic>();
        if (logic != null)
        {
            logic.Init(destroyOnBallEnter, false, 0, 1, 1);
        }

        HolePlacementController.Instance.StartPlacing(hole, isEdge);

        return hole;
    }

    public void DisableHoleAfterDelay(GameObject hole, float delay)
    {
        StartCoroutine(DisableCoroutine(hole, delay));
    }

    private IEnumerator DisableCoroutine(GameObject hole, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hole != null && hole.activeSelf)
        {
            Debug.Log("Disable hole: " + hole.name);
            hole.SetActive(false);
        }
    }

    public void TurnOffAllHoles()
    {
        foreach (var h in edgeHoles)
            if (h != null) h.SetActive(false);

        foreach (var h in middleHoles)
            if (h != null) h.SetActive(false);
    }
}
