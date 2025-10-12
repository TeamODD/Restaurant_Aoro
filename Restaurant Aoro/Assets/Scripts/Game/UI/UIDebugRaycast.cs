using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDebugRaycast : MonoBehaviour
{
    private PointerEventData ped;
    private List<RaycastResult> results = new();

    void Update()
    {
        if (EventSystem.current == null) return;
        ped ??= new PointerEventData(EventSystem.current);

        ped.position = Input.mousePosition;
        results.Clear();
        EventSystem.current.RaycastAll(ped, results);

        if (results.Count > 0)
        {
            // 최상단 히트
            var top = results[0];
            Debug.Log($"[UI Hit] {top.gameObject.name} (SortingLayer: {top.sortingLayer}, Order: {top.sortingOrder})");
        }
    }
}
