using UnityEngine;

public abstract class EquipmentConfig : ScriptableObject
{
    [Tooltip("ID duy nhất dùng để lưu/tải lựa chọn qua file save — đặt số không trùng nhau giữa các asset")]
    public int equipmentId;

    [Tooltip("Tên hiển thị trên nút (VD: Phóng To, Thu Nhỏ...)")]
    public string displayName;
}
