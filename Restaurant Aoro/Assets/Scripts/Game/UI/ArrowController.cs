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

        // 목표 좌표
        Vector3 camTargetPos = new Vector3(targetX, startCamPos.y, startCamPos.z);
        Vector3 panelTargetOffset = inventoryController.BaseOffset;

        // 화살표 이동 계산
        Vector3[] arrowStartPositions = new Vector3[Arrow.Length];
        Vector3[] arrowTargetPositions = new Vector3[Arrow.Length];
        Vector3 offset = endCamPos - startCamPos;

        for (int i = 0; i < Arrow.Length; i++)
        {
            arrowStartPositions[i] = Arrow[i].transform.position;
            arrowTargetPositions[i] = arrowStartPositions[i] + offset;
        }
        Vector3 arrowinitialpos = inventoryController.GetrightArrowInitialPos();
        inventoryController.SetrightArrowInitialPos(arrowinitialpos + offset);

        //인벤토리 패널이 중앙일 경우
        if (inventoryController.GetArrowCentered())
        {
            panelTargetOffset = inventoryManager.offsetCenter;

            //Vector3 arrowTmpPos = new Vector3(arrowTargetPositions[1].x + inventoryController.BaseOffset.x, arrowTargetPositions[1].y, arrowTargetPositions[1].z);
            //Vector3 arrowTmpPos = panelTargetOffset - inventoryController.BaseOffset;
            //float offsetX = centerOffset.x - BaseOffset.x;
            //inventoryController.SetrightArrowInitialPos(arrowTmpPos);
        }
        else
        {
            //inventoryController.SetrightArrowInitialPos(arrowTargetPositions[1]);
        }

        float t = 0f;
        inventoryController.SetIsAnimating(true);

        // 인벤토리 패널 애니메이션 시작
        inventoryController.AnimateToOffset(panelTargetOffset, camTargetPos, duration);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            cameraTransform.position = Vector3.Lerp(startCamPos, endCamPos, t);

            for (int i = 0; i < Arrow.Length; i++)
            {
                Arrow[i].transform.position = Vector3.Lerp(arrowStartPositions[i], arrowTargetPositions[i], t);
            }

            yield return null;
        }
        
        // 최종 위치 보정
        cameraTransform.position = endCamPos;

        // 애니메이션 종료 후 화살표 fade out (끝에 도달한 경우)
        for (int i = 0; i < Arrow.Length; i++)
        {
            Arrow[i].transform.position = arrowTargetPositions[i];
        }

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
