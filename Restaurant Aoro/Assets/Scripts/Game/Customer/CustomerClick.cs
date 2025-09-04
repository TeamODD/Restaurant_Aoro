using AOT;
using Game.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



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
    private SlidingController backBtn;

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
        float moveDuration,
        SlidingController backbtn
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
        this.backBtn = backbtn;
    }

    public void setCanClickTrue() => canClick = true;
    public void setCanClickFalse() => canClick = false;

    public void setSeatedTrue() => isSeated = true;

    private void Start()
    {
        Button btn = backBtn.GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.RemoveListener(OnBackButtonClick);
            btn.onClick.AddListener(OnBackButtonClick);
        }
    }
    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (s_locked != null && s_locked != this) return;

        if (!canClick || !isSeated) return;

        if (invController != null && invController.IsAnimating) return;

        var cm = manager != null ? manager : GetComponent<CustomerManager>();
        if (cm == null) return;

        var anchor = cm.speechAnchor != null ? cm.speechAnchor : cm.transform;
        Debug.Log(anchor.transform);

        if (DialogueManager.Instance != null)
        {
            bool consumed = DialogueManager.Instance.TryPresentNext(
                cm,
                DialogueType.Order,
                anchor
            );
            if (consumed) return;
        }

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
            backBtn.SlideIn();
            Invmanager.ChangeToFoodInventory();
            zoomed = true;
        }
    }

    private void OnBackButtonClick()
    {
        if (zoomed)
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
            backBtn.SlideOut(true);
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
