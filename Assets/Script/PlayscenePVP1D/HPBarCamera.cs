using UnityEngine;

public class HPBarCamera : MonoBehaviour
{
    public Camera targetCamera;

    void LateUpdate()
    {
        if (targetCamera == null) return;

        transform.forward = targetCamera.transform.forward;
    }
}
