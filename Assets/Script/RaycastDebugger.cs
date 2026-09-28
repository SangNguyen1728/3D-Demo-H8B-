using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // hoặc Input.touchCount > 0 trên mobile
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"--- Raycast tại {Input.mousePosition}: {results.Count} object trúng ---");
            foreach (var result in results)
            {
                Debug.Log($"  -> {result.gameObject.name} (thứ tự vẽ: sortingOrder={result.sortingOrder}, depth={result.depth})");
            }
        }
    }
}
