using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
    public Transform cameraTransform;  // Main Camera의 Transform 직접 할당
    public TabletButtonController tabletController;

    private Coroutine moveCoroutine;

    public void OnArrowClicked(int arrowIndex)
    {
        float targetX = 0f;

        switch (arrowIndex)
        {
            case 1:
                targetX = -19.58f;
                break;
            case 2:
                targetX = 0f;
                break;
            case 3:
                targetX = -39.16f;
                break;
            case 4:
                targetX = -19.58f;
                break;
            default:
                Debug.LogWarning("올바르지 않은 arrowIndex");
                return;
        }
        CustomerManager[] allCustomers = FindObjectsOfType<CustomerManager>();
        foreach (var cm in allCustomers)
        {
            if (cm != null && cm.GetHasSeated())
            {
                cm.ForceSeatImmediately(cm.GetSeatLocation());
            }
        }


        // 기존 이동 중이면 멈춤
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        // 새 이동 시작
        moveCoroutine = StartCoroutine(MoveCamera(targetX, 0.5f)); // 1초 동안 이동
    }

    private IEnumerator MoveCamera(float targetX, float duration)
    {
        Vector3 startPos = cameraTransform.position;
        Vector3 endPos = new Vector3(targetX, startPos.y, startPos.z);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cameraTransform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = endPos;
    }
}
