using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpawnCustomer : MonoBehaviour
{
    [Header("고객")]
    public GameObject[] customerPrefabs;
    private HashSet<string> activeTypes = new HashSet<string>();

    [Header("위치")]
    public Transform spawnPoint;
    public Transform stopPoint;

    [Header("상태 제어")]
    public TabletState tabletState;

    [Header("이벤트용")]
    public InventoryManager inventoryManager;
    public InventoryController inventoryController;
    public Camera mainCamera;
    private Vector2 centerOffset = new Vector2(620f, 0f); //545f
    public CanvasGroup[] arrowGroups;
    public float zoomInSize = 4f;
    public float zoomOutSize = 5f;
    public float zoomDuration = 0.35f;
    public float moveDuration = 0.35f;

    private GameObject currentCustomer;
    private bool isSpawning = false;

    private bool allowSpawning = false;
    private Coroutine spawnLoopCo = null;
    private Coroutine spawnDelayCo = null;

    public void StartCustomerFlow()
    {
        allowSpawning = true;


        if (spawnLoopCo == null)
            spawnLoopCo = StartCoroutine(SpawnLoop());
    }

    public void StopCustomerFlow()
    {
        allowSpawning = false;

        if (spawnDelayCo != null)
        {
            StopCoroutine(spawnDelayCo);
            spawnDelayCo = null;
        }

        if (spawnLoopCo != null)
        {
            StopCoroutine(spawnLoopCo);
            spawnLoopCo = null;
        }

        isSpawning = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (allowSpawning)
        {
            if (allowSpawning && currentCustomer == null && !isSpawning && tabletState.canSeat)
            {
                float delay = Random.Range(0.5f, 3f);
                if (spawnDelayCo == null)
                    spawnDelayCo = StartCoroutine(SpawnAfterDelay(delay));
            }
            yield return null;
        }
    }

    public void TrySpawnCustomer()
    {
        if (currentCustomer != null || isSpawning || spawnDelayCo != null)
            return;

        StartCoroutine(SpawnAfterDelay(0.5f));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);

        float t = 0f;
        while (t < delay)
        {
            if (!allowSpawning)
            {
                isSpawning = false;
                spawnDelayCo = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        /*int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject randomCustomer = customerPrefabs[randomIndex];*/
        List<GameObject> candidates = new List<GameObject>();

        foreach (var prefab in customerPrefabs)
        {
            if (!prefab) continue;

            var cm = prefab.GetComponent<CustomerManager>();
            if (cm == null || cm.customerData == null) continue;

            string typeKey = cm.customerData.name;

            if (activeTypes.Contains(typeKey)) continue;

            candidates.Add(prefab);
        }
        if (candidates.Count == 0)
        {
            isSpawning = false;
            spawnDelayCo = null;
            yield break;
        }

        GameObject randomCustomer = candidates[Random.Range(0, candidates.Count)];

        currentCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);
        var manager = currentCustomer.GetComponent<CustomerManager>();
        manager.Init(this, stopPoint.position, tabletState);

        if (manager != null && manager.customerData != null)
        {
            string typeKey = manager.customerData.name;
            activeTypes.Add(typeKey);
        }

        //tmp
        if (manager != null && manager.customerData != null)
        {
            DialogueManager.Instance.Register(manager, manager.customerData);
        }

        var clickProxy = currentCustomer.GetComponent<CustomerClick>();
        if (clickProxy == null) clickProxy = currentCustomer.AddComponent<CustomerClick>();

        clickProxy.Setup(
            manager,
            inventoryManager,
            tabletState,
            inventoryController,    // InventoryController ����
            mainCamera,             // Orthographic ī�޶�
            centerOffset,           // �߾� anchoredPosition
            arrowGroups,            // CanvasGroup[] (��/�� ȭ��ǥ)
            zoomInSize,             // ��: 3.5f
            zoomOutSize,            // ��: 5f
            zoomDuration,           // ��: 0.35f
            moveDuration,            // ��: 0.35f
            inventoryManager.backBtn
        );

        isSpawning = false;
        //tabletState.canClicked = true;

        spawnDelayCo = null;
    }
    public void UnregisterCustomerType(CustomerManager manager)
    {
        if (manager == null || manager.customerData == null) return;
        string typeKey = manager.customerData.name;
        activeTypes.Remove(typeKey);
    }

    public void ClearCurrentCustomer()
    {
        currentCustomer = null;
    }

    public GameObject GetCurrentCustomer()
    {
        return currentCustomer;
    }
}
