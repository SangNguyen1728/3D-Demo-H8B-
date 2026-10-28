using UnityEngine;
using UnityEngine.EventSystems;

public class SkillJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    //[Tooltip("Image tay cầm (con của nền, anchor giữa, Raycast Target TẮT)")]
    //public RectTransform handle;

    //[Tooltip("Bán kính kéo tối đa của tay cầm (đơn vị UI). Nên nhỏ hơn nửa kích thước nền.")]
    //public float radius = 70f;

    //public Vector2 Value { get; private set; }

    //private RectTransform baseRt;

    //private void Awake()
    //{
    //    baseRt = transform as RectTransform;
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    OnDrag(eventData);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (baseRt == null) baseRt = transform as RectTransform;
    //    if (baseRt == null) return;

    //    Vector2 local;
    //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRt, eventData.position, eventData.pressEventCamera, out local))
    //        return;

    //    Vector2 clamped = Vector2.ClampMagnitude(local, radius);
    //    if (handle != null) handle.anchoredPosition = clamped;
    //    Value = radius > 0f ? clamped / radius : Vector2.zero;
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    ResetValue();
    //}

    //public void ResetValue()
    //{
    //    Value = Vector2.zero;
    //    if (handle != null) handle.anchoredPosition = Vector2.zero;
    //}

    //private void OnDisable()
    //{
    //    ResetValue();
    //}

    [Tooltip("Image tay cầm (con của nền, anchor giữa, Raycast Target TẮT)")]
    public RectTransform handle;

    [Tooltip("Bán kính kéo tối đa của tay cầm (đơn vị UI). Nên nhỏ hơn nửa kích thước nền.")]
    public float radius = 70f;

    [Tooltip("true = điểm chạm đầu tiên là TÂM (chạm lệch tâm cũng không bị trôi). false = tâm cố định ở giữa nền.")]
    public bool floatingOrigin = true;

    public Vector2 Value { get; private set; }

    private RectTransform baseRt;
    private Vector2 originLocal;

    private void Awake()
    {
        baseRt = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 local;
        if (!TryGetLocal(eventData, out local)) return;

        originLocal = floatingOrigin ? Vector2.ClampMagnitude(local, radius) : Vector2.zero;
        UpdateFrom(local);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 local;
        if (!TryGetLocal(eventData, out local)) return;

        UpdateFrom(local);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetValue();
    }

    public void ResetValue()
    {
        Value = Vector2.zero;
        originLocal = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }

    private void OnDisable()
    {
        ResetValue();
    }

    private bool TryGetLocal(PointerEventData eventData, out Vector2 local)
    {
        local = Vector2.zero;

        if (baseRt == null) baseRt = transform as RectTransform;
        if (baseRt == null) return false;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRt, eventData.position, eventData.pressEventCamera, out local);
    }

    private void UpdateFrom(Vector2 local)
    {
        Vector2 delta = Vector2.ClampMagnitude(local - originLocal, radius);

        if (handle != null) handle.anchoredPosition = originLocal + delta;
        Value = radius > 0f ? delta / radius : Vector2.zero;
    }
}
