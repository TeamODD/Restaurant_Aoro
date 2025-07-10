using UnityEngine;
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

    private   GameObject currentCustomer;
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
