using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    [Header("손님 데이터")]
    public Customer customerData;
    public Transform speechAnchor;
    public GameObject idle_up;
    public GameObject Plate;
    public GameObject gaugeBG;
    public GameObject gaugeFilled;
    public Transform gaugeFilledTransform;
    public float fillSpeed = 0.1f;

    public static event Action OnAnyCustomerAccepted;

    private Animator animator;
    private Animator animator_idle_up;
    private Vector3 stopPosition;
    private SpawnCustomer spawner;
    private TabletState tabletState;
    private Transform customerSeat;

    private float speed = 5f;
    private bool isLeaving = false;
    private bool hasSeated = false;
    private bool greetedOnce = false;

    public event Action<CustomerManager> OnSeated;
    public static event Action<int> OnMoneyEarned;

    private Coroutine SeatCoroutine;
    private Coroutine fillRoutine;
    private Vector3 baseScale;

    [SerializeField] private PlateTile myPlateTile;
    private Item lastServedItem;     
    public ResultType resultTypeOnLastServe;   // 디버깅용

    private bool hasPaidOut = false;
    private Coroutine leaveRoutine;
    public bool IsLeaveScheduled => leaveRoutine != null || isLeaving;

    public void Init(SpawnCustomer spawner, Vector3 stopPos, TabletState tabletState)
    {
        this.spawner = spawner;
        this.stopPosition = stopPos;
        this.tabletState = tabletState;

        animator_idle_up = idle_up.GetComponent<Animator>();
        idle_up.SetActive(false);

        animator = GetComponent<Animator>();
        animator.Play(customerData.leftAnim.name);

        var lp = Plate.transform.localPosition;
        lp.z -= 0.4f;
        Plate.transform.localPosition = lp;

        if (gaugeFilledTransform != null)
            baseScale = gaugeFilledTransform.localScale;

        gaugeBG.SetActive(false);
        gaugeFilled.SetActive(false);

        if (myPlateTile == null)
            myPlateTile = GetComponentInChildren<PlateTile>(includeInactive: true);

        StartCoroutine(MoveToStopPosition());
    }
    public Transform GetSpeechAnchor()
    {
        return speechAnchor != null ? speechAnchor : transform;
    }

    private IEnumerator MoveToStopPosition()
    {
        while (Vector3.Distance(transform.position, stopPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, stopPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = stopPosition;

        animator.Play(customerData.frontAnim.name);

        if (!greetedOnce && DialogueManager.Instance != null && customerData.greetingLines.Count > 0)
        {
            var anchor = speechAnchor != null ? speechAnchor : transform;
            DialogueManager.Instance.RequestGreeting(this, customerData.greetingLines, anchor);
        }

        tabletState.canClicked = true;
    }

    public void MarkGreeted() => greetedOnce = true;
    public bool HasGreeted() => greetedOnce;

    public void Refuse()
    {
        if (isLeaving) return;
        isLeaving = true;

        tabletState.canClicked = false;

        animator.Play(customerData.rightAnim.name);

        spawner.ClearCurrentCustomer();
        StartCoroutine(MoveAndDestroy());
    }

    public void Accept(Transform seatLocation)
    {
        tabletState.canClicked = false;
        animator.Play(customerData.leftAnim.name);
        hasSeated = true;
        customerSeat = seatLocation;

        OnAnyCustomerAccepted?.Invoke();

        spawner.ClearCurrentCustomer();
        SeatCoroutine = StartCoroutine(MoveAndSeated(seatLocation));
    }

    public void LeaveRestaurant()
    {
        if (isLeaving) return;
        isLeaving = true;

        FreeCurrentSeat();

        hasSeated = false;
        animator.Play(customerData.rightAnim.name);

        CustomerClick customerClick = GetComponent<CustomerClick>();
        customerClick.setCanClickFalse();

        StartCoroutine(MoveAndDestroy());
    }

    private void FreeCurrentSeat()
    {
        if (tabletState == null || customerSeat == null) return;

        foreach (var seatGO in tabletState.Seats)
        {
            if (seatGO == null) continue;

            var state = seatGO.GetComponent<SeatState>();
            if (state == null || state.SeatLocation == null) continue;

            if (state.SeatLocation.transform == customerSeat)
            {
                state.isSeated = false;
                state.isClicked = false;

                var img = seatGO.GetComponent<Image>();
                if (img != null && tabletState.seatBlankSprite != null)
                    img.sprite = tabletState.seatBlankSprite;

                break;
            }
        }
        bool anyEmpty = false;
        foreach (var seatGO in tabletState.Seats)
        {
            var st = seatGO?.GetComponent<SeatState>();
            if (st != null && !st.isSeated) { anyEmpty = true; break; }
        }
        tabletState.canSeat = anyEmpty;
    }

    public void StartEatingAndFill()
    {
        if (myPlateTile != null)
            myPlateTile.SetInteractable(false);

        var customerClick = GetComponent<CustomerClick>();
        if (customerClick != null) customerClick.SuppressForEating();

        Eating();     
        FillToFull();
    }

    public void Eating()
    {
        idle_up.SetActive(false);
        animator.Play(customerData.eatingAnim.name);

        var customerClick = GetComponent<CustomerClick>();
        if (customerClick != null) customerClick.SuppressForEating();
    }

    public void FillToFull()
    {
        gaugeBG.SetActive(true);
        gaugeFilled.SetActive(true);

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);
        fillRoutine = StartCoroutine(FillRoutine());
    }

    private IEnumerator FillRoutine()
    {
        gaugeFilledTransform.localScale = new Vector3(0, baseScale.y, baseScale.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            float clamped = Mathf.Clamp01(t);

            gaugeFilledTransform.localScale = new Vector3(baseScale.x * clamped, baseScale.y, baseScale.z);

            yield return null;
        }
        gaugeFilledTransform.localScale = baseScale;
        gaugeBG.SetActive(false);
        gaugeFilled.SetActive(false);
        animator.Play(customerData.seatedAnim.name);

        var customerClick = GetComponent<CustomerClick>();
        if (customerClick != null) customerClick.setCanClickTrue();

        lastServedItem = myPlateTile != null ? myPlateTile.PeekItem() : null;
        if (myPlateTile != null) myPlateTile.ClearSpriteOnly();

        resultTypeOnLastServe = EvaluateResult(lastServedItem, customerData);

        var click = GetComponent<CustomerClick>();
        if (click != null) click.ShowResultExclamation();

    }

    public void RequestResultDialogue(bool randomPick = false)
    {
        var anchor = GetSpeechAnchor();

        if (DialogueManager.Instance == null)
            return;

        bool shown = DialogueManager.Instance.TryPresentResult(
            this,
            resultTypeOnLastServe,
            anchor,
            randomPick
        );
    }
    

    private ResultType EvaluateResult(Item served, Customer cust)
    {
        if (served == null)
            return ResultType.Fail;

        int score = 0;

        const int FAVOR_TASTE = 2;
        const int DISLIKE_TASTE = -2;
        const int FAVOR_CATEGORY = 2;
        const int DISLIKE_CATEGORY = -2;

        if (cust.favoriteTastes != null && cust.favoriteTastes.Contains(served.Foodtaste))
        {
            score += FAVOR_TASTE;
            Debug.Log($"[{cust.CustomerID}] 좋아하는 맛({served.Foodtaste}) → +{FAVOR_TASTE}");
        }
        else if (cust.dislikedTastes != null && cust.dislikedTastes.Contains(served.Foodtaste))
        {
            score += DISLIKE_TASTE;
            Debug.Log($"[{cust.CustomerID}] 싫어하는 맛({served.Foodtaste}) → {DISLIKE_TASTE}");
        }

        if (cust.favoriteFoods != null && cust.favoriteFoods.Contains(served.ItemMainCategory))
        {
            score += FAVOR_CATEGORY;
            Debug.Log($"[{cust.CustomerID}] 좋아하는 종류({served.ItemMainCategory}) → +{FAVOR_CATEGORY}");
        }
        else if (cust.dislikedFoods != null && cust.dislikedFoods.Contains(served.ItemMainCategory))
        {
            score += DISLIKE_CATEGORY;
            Debug.Log($"[{cust.CustomerID}] 싫어하는 종류({served.ItemMainCategory}) → {DISLIKE_CATEGORY}");
        }

        if (score >= 4)
        {
            Debug.Log($"[{cust.CustomerID}] 결과: Perfect ({score})");
            return ResultType.Perfect;
        }
        else if (score >= 2)
        {
            Debug.Log($"[{cust.CustomerID}] 결과: Excellent ({score})");
            return ResultType.Excellent;
        }
        else if (score >= 0)
        {
            Debug.Log($"[{cust.CustomerID}] 결과: Success ({score})");
            return ResultType.Success;
        }
        else
        {
            Debug.Log($"[{cust.CustomerID}] 결과: Fail ({score})");
            return ResultType.Fail;
        }
    }

    private int ResultToPayIndex(ResultType r)
    {
        switch (r)
        {
            case ResultType.Perfect:
            case ResultType.Excellent: return 2; 
            case ResultType.Success: return 1; 
            case ResultType.Fail:
            case ResultType.Late:
            case ResultType.WrongOrder:
            default: return 0; 
        }
    }

    private void ResolvePayment()
    {
        if (customerData == null) return;

        int idx = ResultToPayIndex(resultTypeOnLastServe);

        if (customerData.tribe == TribeType.Human)
        {
            int amount = 0;
            if (customerData.payable != null && idx >= 0 && idx < customerData.payable.Count)
                amount = customerData.payable[idx];

            if (amount > 0)
            {
                OnMoneyEarned?.Invoke(amount);
                Debug.Log($"[Pay] Human paid {amount} (idx:{idx})");
            }
            else
            {
                Debug.LogWarning($"[Pay] payable[{idx}] is empty/zero. Customer:{customerData.CustomerID}");
            }
        }
        else // Youkai
        {
            Item reward = null;
            if (customerData.payItem != null && idx >= 0 && idx < customerData.payItem.Count)
                reward = customerData.payItem[idx];

            if (reward != null)
            {
                InventoryManager.instance.AddItem(reward);
                Debug.Log($"[Pay] Youkai gave item {reward.name} (idx:{idx})");
            }
            else
            {
                Debug.LogWarning($"[Pay] payItem[{idx}] is null. Customer:{customerData.CustomerID}");
            }
        }
    }
    private Vector3 GetExitPos()
    {
        return new Vector3(30f, transform.position.y, transform.position.z);
    }

    private void InstantDespawn()
    {
        if (gaugeBG) gaugeBG.SetActive(false);
        if (gaugeFilled) gaugeFilled.SetActive(false);
        if (idle_up) idle_up.SetActive(false);

        transform.position = GetExitPos();

        Destroy(gameObject);
    }
    public void ConfirmResultAndLeave(float delay = 1.6f)
    {
        if (leaveRoutine != null) StopCoroutine(leaveRoutine);
        leaveRoutine = StartCoroutine(ConfirmResultAndLeaveRoutine(delay));
    }

    private IEnumerator ConfirmResultAndLeaveRoutine(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        ResolvePayment();
        LeaveRestaurant();

        leaveRoutine = null;
    }

    public void ForceLeaveNow()
    {
        if (isLeaving)
        {
            InstantDespawn();
            return;
        }

        if (leaveRoutine != null)
        {
            StopCoroutine(leaveRoutine);
            leaveRoutine = null;
        }

        if (!hasPaidOut)
        {
            ResolvePayment();
            hasPaidOut = true;
        }

        isLeaving = true;
        hasSeated = false;

        var customerClick = GetComponent<CustomerClick>();
        if (customerClick != null) customerClick.setCanClickFalse();

        InstantDespawn();
    }

    private IEnumerator MoveAndDestroy()
    {
        Vector3 exitPos = new Vector3(30f, transform.position.y, transform.position.z);
        while (Vector3.Distance(transform.position, exitPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitPos, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        Destroy(gameObject);
    }

    private IEnumerator MoveAndSeated(Transform seatLocation)
    {
        Vector3 moveTarget = new Vector3(seatLocation.position.x - 1f, transform.position.y, -0.1f);

        while (Vector3.Distance(transform.position, moveTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = seatLocation.position;
        animator.Play(customerData.seatedAnim.name);
        var lp = idle_up.transform.localPosition;
        lp.z -= 0.3f;
        idle_up.transform.localPosition = lp;
        idle_up.SetActive(true);
        animator_idle_up.Play(customerData.upAnim.name);

        CustomerClick customerClick = GetComponent<CustomerClick>();
        hasSeated = true;
        OnSeated?.Invoke(this);
        customerClick.setSeatedTrue();
        yield return new WaitForSeconds(0.3f);
    }

    public void ForceSeatImmediately()
    {
        if (IsLeaveScheduled)
        {
            ForceLeaveNow();
            return;
        }

        if (hasSeated == true)
        {
            if (SeatCoroutine != null)
            {
                StopCoroutine(SeatCoroutine); // 이동 중지
            }

            transform.position = customerSeat.position;
            animator.Play(customerData.seatedAnim.name);
            var lp = idle_up.transform.localPosition;
            lp.z -= 0.3f;
            idle_up.transform.localPosition = lp;
            idle_up.SetActive(true);
            animator_idle_up.Play(customerData.upAnim.name);
            CustomerClick customerClick = GetComponent<CustomerClick>();
            hasSeated = true;            
            OnSeated?.Invoke(this);
            customerClick.setSeatedTrue();
        }
    }
    public bool GetHasSeated()
    {
        return hasSeated;
    }

    public Transform GetSeatLocation()
    {
        return customerSeat;
    }
}
