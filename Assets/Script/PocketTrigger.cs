using UnityEngine;

public class PocketTrigger : MonoBehaviour
{
    private SkillSlowMotion skillManager;

    void Start()
    {
        // 1. Tìm Skill Manager
        // Chúng ta đang sử dụng tag "SkillTest"
        GameObject managerObject = GameObject.FindWithTag("SkillTest");

        if (managerObject != null)
        {
            // Cần tìm component SkillManager
            skillManager = managerObject.GetComponent<SkillSlowMotion>();
        }

        if (skillManager == null)
        {
            // 🚨 SỬA LỖI DEBUG: Ghi rõ tag và component đang tìm
            Debug.LogError("PocketTrigger: KHÔNG tìm thấy component SkillManager trên đối tượng có tag 'SkillTest'!");
        }
        else
        {
            Debug.Log("<color=lime>PocketTrigger: Đã kết nối thành công với SkillManager.</color>");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Giả định viên bi có tag là "Ball"
        if (other.CompareTag("Ball"))
        {
            // 🚨 DEBUG LOG XÁC NHẬN BI VÀO LỖ
            Debug.Log($"<color=yellow>PocketTrigger: Bi '{other.gameObject.name}' đã lọt vào lỗ '{gameObject.name}'.</color>");

            if (skillManager != null)
            {
                // Gọi hàm đếm bi trong Skill Manager
                skillManager.BallPocketed(other.gameObject.name);
            }

            // Hủy viên bi sau khi lọt lỗ
            Destroy(other.gameObject);
        }
    }

    //void Start()
    //{
    //    // Tìm Skill Manager
    //    GameObject managerObject = GameObject.FindWithTag("SkillTest");
    //    if (managerObject != null)
    //    {
    //        skillManager = managerObject.GetComponent<SkillSlowMotion>();
    //    }

    //    if (skillManager == null)
    //    {
    //        Debug.LogError("Pocket: KHÔNG tìm thấy SkillSlowMotion script trên đối tượng có tag 'GameController'!");
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    // Giả định viên bi có tag là "Ball"
    //    if (other.CompareTag("Ball"))
    //    {
    //        if (skillManager != null)
    //        {
    //            // Gọi hàm để đếm bi
    //            //skillManager.BallSunk();
    //        }

    //        // Hủy viên bi sau khi lọt lỗ
    //        Destroy(other.gameObject);
    //    }
    //}
}
