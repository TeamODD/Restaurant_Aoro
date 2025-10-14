using AOT;
using Game.UI;
using System.Collections;
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

    [Header("Exclamation (느낌표)")]
    [SerializeField] private GameObject exclamation;
    [SerializeField] private float exclamationDelayMin = 1f;
    [SerializeField] private float exclamationDelayMax = 3f;
    private Coroutine exclamationRoutine;
    private bool exclamationShownOnce = false;

    private bool pendingZoomIn = false;
    private Coroutine pendingRoutine;
    private float clickDebounce = 2f; // 선택: 너무 빠른 중복 클릭 억제
    private float lastClickTime = -999f;

    private Coroutine foldRoutine;

    private void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
    }
    private void OnDisable()
    {
        All.Remove(this);
        if (s_locked == this) UnlockAll();
        if (manager != null) manager.OnSeated -= HandleSeated;
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

        if (exclamation == null)
        {
            var tr = transform.Find("Exclamation");
            if (tr != null) exclamation = tr.gameObject;
            Debug.Log("Find");
        }
        if (exclamation != null)
        {
            Debug.Log("OK");
            exclamation.SetActive(false);
        }

        if (manager != null)
        {
            manager.OnSeated -= HandleSeated;  
            manager.OnSeated += HandleSeated;

            if (manager.GetHasSeated()) 
            {
                HandleSeated(manager); 
            }
        }
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
    private void HandleSeated(CustomerManager cm)
    {
        if (cm != manager) return;
        if (exclamationShownOnce) return;
        isSeated = true;

        if (exclamationRoutine != null)
        {
            StopCoroutine(exclamationRoutine);
            exclamationRoutine = null;
        }
        exclamationRoutine = StartCoroutine(ExclamationAfterDelay());
    }

    private IEnumerator ExclamationAfterDelay()
    {
        float delay = Random.Range(exclamationDelayMin, exclamationDelayMax);
        yield return new WaitForSeconds(delay);

        if (exclamation != null) exclamation.SetActive(true);
        canClick = true;

        exclamationShownOnce = true;

        exclamationRoutine = null;
    }

    private void OnMouseDown()
    {
        if (Time.unscaledTime - lastClickTime < clickDebounce) return;
        lastClickTime = Time.unscaledTime;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (s_locked != null && s_locked != this) return;
        if (!canClick || !isSeated) return;

        var cm = manager != null ? manager : GetComponent<CustomerManager>();
        if (cm == null) return;
        var anchor = cm.speechAnchor != null ? cm.speechAnchor : cm.transform;

        if (exclamation != null && exclamation.activeSelf)
            exclamation.SetActive(false);

        if (invController != null && invController.IsAnimating)
        {
            if (!pendingZoomIn) pendingRoutine = StartCoroutine(WaitAndZoomIn(cm, anchor));
            return;
        }

        if (DialogueManager.Instance != null)
        {
            bool consumed = DialogueManager.Instance.TryPresentNext(cm, DialogueType.Order, anchor);
            if (consumed) return;
        }

        TryZoomInAndOpen();
    }
    public static void ServeLockedCustomer()
    {
        if (s_locked == null) return;
        s_locked.ServeAndExit();
    }
    public void ServeAndExit()
    {
        var cm = manager != null ? manager : GetComponent<CustomerManager>();
        if (cm != null)
        {
            cm.StartEatingAndFill();
        }

        ExitFocus();
    }
    private IEnumerator WaitAndZoomIn(CustomerManager cm, Transform anchor)
    {
        pendingZoomIn = true;

        while (invController != null && invController.IsAnimating)
            yield return null;

        if (s_locked != null && s_locked != this) { pendingZoomIn = false; yield break; }
        if (!canClick || !isSeated) { pendingZoomIn = false; yield break; }
        if (zoomed) { pendingZoomIn = false; yield break; }

        if (DialogueManager.Instance != null)
        {
            bool consumed = DialogueManager.Instance.TryPresentNext(cm, DialogueType.Order, anchor);
            if (consumed) { pendingZoomIn = false; yield break; }
        }

        TryZoomInAndOpen();
        pendingZoomIn = false;
    }

    private void TryZoomInAndOpen()
    {
        if (zoomed) return;

        if (invController != null && invController.IsInventoryOpen)
        {
            if (foldRoutine != null) StopCoroutine(foldRoutine);
            foldRoutine = StartCoroutine(FoldThenZoomIn());
            return;
        }

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
        if (backBtn != null) backBtn.SlideIn();   
        if (Invmanager != null) Invmanager.ChangeToFoodInventory();

        zoomed = true;
    }

    private void ExitFocus()
    {
        if (pendingRoutine != null) { StopCoroutine(pendingRoutine); pendingRoutine = null; pendingZoomIn = false; }

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

    private void OnBackButtonClick()
    {
        if (pendingRoutine != null) { StopCoroutine(pendingRoutine); pendingRoutine = null; pendingZoomIn = false; }

        if (PlateManager.instance != null)
            PlateManager.instance.RemoveAllFromPlate();

        InventoryController.InvokeServe();

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

    private IEnumerator FoldThenZoomIn()
    {
        float half = Mathf.Max(0.01f, moveDuration * 0.5f);

        if (invController != null && invController.IsInventoryOpen)
        {
            invController.MovePanelToCenter(centerOffset, half);
            while (invController != null && invController.IsAnimating)
                yield return null;
        }

        invController.ZoomAndFrameTargetLeftCenter(
            mainCam,
            transform,
            zoomInSize,
            zoomDuration,
            centerOffset,  
            half,           
            arrowGroups,
            true,
            viewportX,
            viewportY
        );

        while (invController != null && invController.IsAnimating)
            yield return null;

        LockToThis();
        if (backBtn != null) backBtn.SlideIn();
        if (Invmanager != null) Invmanager.ChangeToFoodInventory();
        zoomed = true;

    }
}
