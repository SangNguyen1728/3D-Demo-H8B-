using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Demo Targeting Skill", menuName = "Skills/Demo Targeting (test)")]
public class DemoTargetingSkill : BaseSkills
{
    [Header("Demo")]
    public float effectSeconds = 2f;
    [Tooltip("Bán kính vòng đánh dấu (đơn vị thế giới), chỉnh cho to hơn viên bi một chút")]
    public float markerRadius = 0.25f;

    // Bản xem trước theo từng người chơi (KHÔNG lưu trong asset vì asset dùng chung)
    private static readonly Dictionary<int, GameObject> previews = new Dictionary<int, GameObject>();

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        Debug.Log($"[DemoSkill:{skillName}] ACTIVATE ({targetingType}) bởi P{manager.playerNumber}");
    }

    public override void ActivateWithTarget(GameObject player, GlaszekManager manager, SkillTargetData data)
    {
        Debug.Log($"[DemoSkill:{skillName}] TARGET bi {(data.ball != null ? data.ball.name : "none")}");
        if (data.ball != null) manager.StartCoroutine(FlashBall(data.ball));
    }

    public override void OnTurnEnd(GlaszekManager manager) { }

    // ===== XEM TRƯỚC TRỰC TIẾP =====
    public override void BeginPreview(GameObject player, GlaszekManager manager, SkillTargetData data)
    {
        ClearPreview(manager.playerNumber);
        previews[manager.playerNumber] = CreateMarker(markerRadius, Color.yellow);

        Debug.Log($"[DemoSkill:{skillName}] BeginPreview");
        UpdatePreview(manager, data);
    }

    public override void UpdatePreview(GlaszekManager manager, SkillTargetData data)
    {
        GameObject marker = GetPreview(manager.playerNumber);
        if (marker == null || data.ball == null) return;

        marker.transform.position = data.ball.transform.position;
    }

    // Duyệt: GIỮ chính vòng đang xem trước làm kết quả (không tạo cái mới)
    public override void CommitPreview(GameObject player, GlaszekManager manager, SkillTargetData data)
    {
        GameObject marker = GetPreview(manager.playerNumber);
        previews.Remove(manager.playerNumber);

        if (marker == null)
        {
            ActivateWithTarget(player, manager, data);
            return;
        }

        Debug.Log($"[DemoSkill:{skillName}] CommitPreview: giữ vòng tại {marker.transform.position}");
        manager.StartCoroutine(DestroyAfter(marker, effectSeconds));

        if (data.ball != null) manager.StartCoroutine(FlashBall(data.ball));
    }

    public override void CancelPreview(GlaszekManager manager)
    {
        if (manager == null) return;
        Debug.Log($"[DemoSkill:{skillName}] CancelPreview");
        ClearPreview(manager.playerNumber);
    }

    // ===== helpers =====
    private static GameObject GetPreview(int playerNumber)
    {
        GameObject go;
        return previews.TryGetValue(playerNumber, out go) ? go : null; // go có thể là "null giả" nếu đã bị hủy
    }

    private static void ClearPreview(int playerNumber)
    {
        GameObject go;
        if (previews.TryGetValue(playerNumber, out go))
        {
            if (go != null) Destroy(go);
            previews.Remove(playerNumber);
        }
    }

    private GameObject CreateMarker(float radius, Color color)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        marker.name = "DemoMarker";

        Collider col = marker.GetComponent<Collider>();
        if (col != null) Destroy(col);

        marker.transform.localScale = new Vector3(radius * 2f, 0.005f, radius * 2f);

        Renderer r = marker.GetComponent<Renderer>();
        if (r != null) r.material.color = color;

        return marker;
    }

    private IEnumerator DestroyAfter(GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (go != null) Destroy(go);
    }

    private IEnumerator FlashBall(BallNo ball)
    {
        Renderer r = ball != null ? ball.GetComponentInChildren<Renderer>() : null;
        if (r == null) yield break;

        for (int i = 0; i < 6; i++)
        {
            if (r == null) yield break;
            r.enabled = false;
            yield return new WaitForSeconds(0.12f);
            if (r == null) yield break;
            r.enabled = true;
            yield return new WaitForSeconds(0.12f);
        }
    }
}
