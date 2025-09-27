using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CustomerManager : MonoBehaviour
{
    [Header("손님 데이터")]
    public Customer customerData;
    public Transform speechAnchor;
    public GameObject idle_up;

    private Animator animator;
    private Animator animator_idle_up;
    private SpriteRenderer spriteRenderer;
    private Vector3 stopPosition;
    private SpawnCustomer spawner;
    private TabletState tabletState;
    private Transform customerSeat;

    private float speed = 5f;
    private bool isLeaving = false;
    private bool hasSeated = false;
    private bool greetedOnce = false;

    public event Action<CustomerManager> OnSeated;

    private Coroutine SeatCoroutine;

    public void Init(SpawnCustomer spawner, Vector3 stopPos, TabletState tabletState)
    {
        this.spawner = spawner;
        this.stopPosition = stopPos;
        this.tabletState = tabletState;

        animator_idle_up = idle_up.GetComponent<Animator>();
        idle_up.SetActive(false);
        animator = GetComponent<Animator>();
        animator.Play(customerData.leftAnim.name);

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
        //To rotation setting y=180
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = 180f;
        transform.eulerAngles = currentRotation;

        animator.Play(customerData.rightAnim.name);

        spawner.ClearCurrentCustomer();
        StartCoroutine(MoveAndDestroy());
    }

    public void Accept(Transform seatLocation)
    {
        tabletState.canClicked = false;
        animator.Play(customerData.rightAnim.name);
        hasSeated = true;
        customerSeat = seatLocation;

        spawner.ClearCurrentCustomer();
        SeatCoroutine = StartCoroutine(MoveAndSeated(seatLocation));
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
        if(hasSeated == true)
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
            //customerClick.setCanClickTrue();
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
