using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnCustomer : MonoBehaviour
{
    [Header("고객 프리팹 목록")]
    public GameObject[] customerPrefabs;

    [Header("스폰 위치 및 정지 위치")]
    public Transform spawnPoint;
    public Transform stopPoint;

    [Header("상호작용 상태 제어")]
    public TabletState tabletState;

    [Header("클릭 이벤트용")]
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

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentCustomer == null && !isSpawning && tabletState.canSeat)
            {
                float delay = Random.Range(0.5f, 3f); //2second to 10second spawn time
                yield return new WaitForSeconds(delay);
                TrySpawnCustomer();
            }
            else
            {
                yield return null;
            }
        }
    }

    public void TrySpawnCustomer()
    {
        if (currentCustomer != null || isSpawning)
            return;

        StartCoroutine(SpawnAfterDelay(0.5f));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);

        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject randomCustomer = customerPrefabs[randomIndex];

        currentCustomer = Instantiate(randomCustomer, spawnPoint.position, Quaternion.identity);
        var manager = currentCustomer.GetComponent<CustomerManager>();
        manager.Init(this, stopPoint.position, tabletState);

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
            inventoryController,    // InventoryController 참조
            mainCamera,             // Orthographic 카메라
            centerOffset,           // 중앙 anchoredPosition
            arrowGroups,            // CanvasGroup[] (좌/우 화살표)
            zoomInSize,             // 예: 3.5f
            zoomOutSize,            // 예: 5f
            zoomDuration,           // 예: 0.35f
            moveDuration,            // 예: 0.35f
            inventoryManager.backBtn
        );

        isSpawning = false;
        tabletState.canClicked = true;
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
