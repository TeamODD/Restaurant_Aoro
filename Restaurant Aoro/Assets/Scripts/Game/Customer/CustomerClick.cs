using AOT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerClick : MonoBehaviour
{
    private static readonly List<CustomerClick> All = new List<CustomerClick>();
    private static CustomerClick s_locked;

    private CustomerManager manager;
    private InventoryManager Invmanager;
    private TabletState tablet;
    private InventoryController invController;
    private Camera mainCam;
    private Vector2 centerOffset;
    private CanvasGroup[] arrowGroups;

    private float zoomInSize;
    private float zoomOutSize;
    private float zoomDuration;
    private float moveDuration;

    private bool zoomed = false;
    private bool canClick = false;
    private bool isSeated = false;

    [SerializeField] private float viewportX = 0.4f;
    [SerializeField] private float viewportY = 0.5f;

    // ====== 라이프사이클: 전역 목록 관리 ======
    private void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
    }
    private void OnDisable()
    {
        All.Remove(this);
        if (s_locked == this) UnlockAll();
    }

    public void Setup(
        CustomerManager m,
        InventoryManager minv,
        TabletState t,
        InventoryController controller,
        Camera cam,
        Vector2 centerOffset,
        CanvasGroup[] arrowGroups,
        float zoomInSize,
        float zoomOutSize,
        float zoomDuration,
        float moveDuration
    )
    {
        manager = m;
        Invmanager = minv;
        tablet = t;
        invController = controller;
        mainCam = cam;
        this.centerOffset = centerOffset;
        this.arrowGroups = arrowGroups;
        this.zoomInSize = zoomInSize;
        this.zoomOutSize = zoomOutSize;
        this.zoomDuration = zoomDuration;
        this.moveDuration = moveDuration;
    }

    public void setCanClickTrue() => canClick = true;
    public void setCanClickFalse() => canClick = false;

    public void setSeatedTrue() => isSeated = true;

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (s_locked != null && s_locked != this) return;

        if (!canClick || !isSeated) return;

        if (invController != null && invController.IsAnimating) return;

        if (!zoomed)
        {
            invController.ZoomAndFrameTargetLeftCenter(
                mainCam,
                transform,
                zoomInSize,
                zoomDuration,
                centerOffset,
                moveDuration,
                arrowGroups,
                true,
                viewportX, viewportY
            );

            LockToThis();
            Invmanager.ChangeToFoodInventory();
            zoomed = true;
        }
        else
        {
            invController.ResetFromCenterWithCamera(
                mainCam,
                zoomOutSize,
                zoomDuration,
                moveDuration,
                arrowGroups,
                true
            );
            UnlockAll();
            zoomed = false;
        }
    }

    private void LockToThis()
    {
        s_locked = this;
        foreach (var c in All)
        {
            if (c == null) continue;
            if (c == this)
            {
                continue;
            }
            c.canClick = false;
            c.SetColliderEnabled(false);
        }
    }

    private static void UnlockAll()
    {
        s_locked = null;
        foreach (var c in All)
        {
            if (c == null) continue;
            c.canClick = true;        
            c.SetColliderEnabled(true);
        }
    }

    private void SetColliderEnabled(bool enabled)
    {
        var col2d = GetComponent<Collider2D>();
        if (col2d != null) { col2d.enabled = enabled; return; }
        var col3d = GetComponent<Collider>();
        if (col3d != null) { col3d.enabled = enabled; }
    }
}
