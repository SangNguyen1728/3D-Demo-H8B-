using UnityEngine;

public class PocketDetector : MonoBehaviour
{
    private PocketTowPs masterLogic;

    void Start()
    {
        // Tự động tìm script quản lý chính trong Scene
        masterLogic = FindObjectOfType<PocketTowPs>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (masterLogic != null)
        {
            // Báo cho script chính biết có bi vừa chui vào lỗ này
            masterLogic.OnBallEnteredPocket(other);
        }
    }
}
