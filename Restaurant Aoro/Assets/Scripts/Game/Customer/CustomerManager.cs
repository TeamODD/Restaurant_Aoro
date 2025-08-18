using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CustomerManager : MonoBehaviour
{
    [Header("손님 데이터")]
    public Customer customerData;

    private SpriteRenderer spriteRenderer;
    private Vector3 stopPosition;
    private SpawnCustomer spawner;
    private TabletState tabletState;
    private Transform customerSeat;

    private float speed = 5f;
    private bool isLeaving = false;
    private bool hasSeated = false;

    private Coroutine SeatCoroutine;

    public void Init(SpawnCustomer spawner, Vector3 stopPos, TabletState tabletState)
    {
        this.spawner = spawner;
        this.stopPosition = stopPos;
        this.tabletState = tabletState;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = customerData.leftSprite;

        StartCoroutine(MoveToStopPosition());
    }

    private IEnumerator MoveToStopPosition()
    {
        while (Vector3.Distance(transform.position, stopPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, stopPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = stopPosition;
        spriteRenderer.sprite = customerData.frontSprite;
        tabletState.canClicked = true;
    }

    public void Refuse()
    {
        if (isLeaving) return;
        isLeaving = true;

        tabletState.canClicked = false;
        spriteRenderer.sprite = customerData.rightSprite;

        spawner.ClearCurrentCustomer();
        StartCoroutine(MoveAndDestroy());
    }

    public void Accept(Transform seatLocation)
    {
        tabletState.canClicked = false;
        spriteRenderer.sprite = customerData.leftSprite;
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
        spriteRenderer.sprite = customerData.SeatedSprite;

        CustomerClick customerClick = GetComponent<CustomerClick>();
        customerClick.setCanClickTrue();
        customerClick.setSeatedTrue();
        yield return new WaitForSeconds(0.3f);
    }

    public void ForceSeatImmediately(Transform seatLocation)
    {
        if (SeatCoroutine != null)
        {
            StopCoroutine(SeatCoroutine); // 이동 중지
        }

        transform.position = seatLocation.position;
        spriteRenderer.sprite = customerData.SeatedSprite;
        CustomerClick customerClick = GetComponent<CustomerClick>();
        customerClick.setCanClickTrue();
        customerClick.setSeatedTrue();
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
