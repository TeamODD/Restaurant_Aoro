using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using NUnit.Framework.Constraints;
using TMPro;

public class ArrowController : MonoBehaviour
{
    public GameObject[] Arrow;
    public Transform cameraTransform;
    public InventoryController inventoryController;
    public InventoryManager inventoryManager; //후에 변경 예정

    public float stepDistance = 19.58f;

    private int currentStep = 0; // 0 (중앙), 1 (한 칸 왼쪽), 2 (두 칸 왼쪽)
    private const int maxStep = 2;
    private const int minStep = 0;

    private Coroutine moveCoroutine;

    private void Start()
    {
        var rightArrowCG = Arrow[1].GetComponent<CanvasGroup>();
        if (rightArrowCG != null) rightArrowCG.alpha = 0f;
    }
    public void MoveLeft()
    {
        if (currentStep < maxStep && !inventoryController.IsAnimating)
        {
            currentStep++;
            StartMove();
        }
    }
    public void MoveRight()
    {
        if (currentStep > minStep && !inventoryController.IsAnimating)
        {
            currentStep--;
            StartMove();
        }
    }
    private void StartMove()
    {
        float targetX = -stepDistance * currentStep;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveCameraAndInventory(targetX, 0.5f));
    }

    private IEnumerator MoveCameraAndInventory(float targetX, float duration)
    {
        // 시작 위치 설정
        Vector3 startCamPos = cameraTransform.position;
        Vector3 endCamPos = new Vector3(targetX, startCamPos.y, startCamPos.z);

        /*Vector3 camTargetPos = new Vector3(targetX, startCamPos.y, startCamPos.z);
        Vector3 panelTargetOffset = inventoryController.BaseOffset;*/

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cameraTransform.position = Vector3.Lerp(startCamPos, endCamPos, t);
            yield return null;
        }

        // 최종 위치 보정
        cameraTransform.position = endCamPos;

        if (currentStep == maxStep)
        {
            StartCoroutine(FadeOutArrow(Arrow[0], 0.3f));
        }
        else if (currentStep == minStep)
        {
            StartCoroutine(FadeOutArrow(Arrow[1], 0.3f));
        }
        else
        {
            for (int i = 0; i < Arrow.Length; i++)
            {
                var cg = Arrow[i].GetComponent<CanvasGroup>();
                if (cg.alpha != 1f)
                    StartCoroutine(FadeInArrow(Arrow[i], 0.3f));
            }
        }
    }



    private IEnumerator FadeOutArrow(GameObject arrowObj, float duration)
    {
        CanvasGroup cg = arrowObj.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        cg.alpha = 0f;
    }

    private IEnumerator FadeInArrow(GameObject arrowObj, float duration)
    {
        CanvasGroup cg = arrowObj.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float startAlpha = cg.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.Lerp(startAlpha, 1f, t);
            yield return null;
        }

        cg.alpha = 1f;
    }
}
