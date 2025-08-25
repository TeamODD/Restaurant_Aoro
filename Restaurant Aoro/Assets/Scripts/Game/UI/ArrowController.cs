using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
    private enum CameraStep
    {
        Center = 0, // Right
        One = 1, // 1 * Left
        Two = 2, // 2 * Left
    }

    private enum ArrowDirection
    {
        Left = 0,
        Right,
        Down,
        Up
    }

    private struct Axis
    {
        public CameraStep currentStep;
        public CameraStep lastStep;
        public CameraStep minStep { get; }

        public CameraStep maxStep { get; }

        public Axis(CameraStep currentStep, CameraStep minStep, CameraStep maxStep)
        {
            this.currentStep = currentStep;
            this.lastStep = currentStep;
            this.minStep = minStep;
            this.maxStep = maxStep;
        }
    }

    public GameObject[] Arrow;
    public Transform cameraTransform;
    public InventoryController inventoryController;
    public InventoryManager inventoryManager; //후에 변경 예정
    public Transform cookSceneTransform;
    public Transform connectionSceneTransform;
    public Transform customerScene2Transform;
    public Transform counterSceneTransform;

    public float horStepDistance = 19.58f;
    public float verStepDistance = 10.5f;
    public float arrowRemovalDuration = 0.3f;

    private Axis hor = new(CameraStep.Center, CameraStep.Center, CameraStep.Two);
    private Axis ver = new(CameraStep.Center, CameraStep.Center, CameraStep.One);

    private Coroutine moveCoroutine;

    private void Start()
    {
        var rightArrowCG = Arrow[(int)ArrowDirection.Right].GetComponent<CanvasGroup>();
        if (rightArrowCG != null) rightArrowCG.alpha = 0f;

        var upArrowCG = Arrow[(int)ArrowDirection.Up].GetComponent<CanvasGroup>();
        if (upArrowCG != null) upArrowCG.alpha = 0f;
    }

    public void CustomerSeat()
    {
        CustomerManager[] customers = FindObjectsOfType<CustomerManager>();

        foreach (var cm in customers)
        {
             cm.ForceSeatImmediately();
        }
    }
    public void MoveLeft()
    {
        if (hor.currentStep < hor.maxStep && !inventoryController.IsAnimating)
        {
            hor.currentStep++;
            CustomerSeat();
            StartMove();
        }
    }

    public void MoveRight()
    {
        if (hor.currentStep > hor.minStep && !inventoryController.IsAnimating)
        {
            hor.currentStep--;
            CustomerSeat();
            StartMove();
        }
    }

    public void MoveDown()
    {
        if (ver.currentStep < ver.maxStep && !inventoryController.IsAnimating)
        {
            ver.currentStep++;
            CustomerSeat();
            StartMove();
        }
    }

    public void MoveUp()
    {
        if (ver.currentStep > ver.minStep && !inventoryController.IsAnimating)
        {
            ver.currentStep--;
            CustomerSeat();
            StartMove();
        }
    }

    private void StartMove()
    {
        float targetX = -horStepDistance * (int)hor.currentStep;
        float targetY = -verStepDistance * (int)ver.currentStep;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveCameraAndInventory(targetX, targetY, 0.5f));
    }

    private IEnumerator MoveCameraAndInventory(float targetX, float targetY, float duration)
    {
        if (ver.currentStep == ver.minStep && ver.lastStep != ver.currentStep)
        {
            cameraTransform.position = new Vector3(connectionSceneTransform.position.x,
                connectionSceneTransform.position.y, cameraTransform.position.z);
        }

        // 시작 위치 설정
        Vector3 startCamPos = cameraTransform.position;
        Vector3 endCamPos = new Vector3(hor.currentStep != hor.lastStep ? targetX : cameraTransform.position.x,
            ver.currentStep != ver.lastStep ? targetY : cameraTransform.position.y, startCamPos.z);

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

        if (hor.currentStep == hor.maxStep)
        {
            if (ver.lastStep == ver.maxStep)
            {
                cameraTransform.position = new Vector3(customerScene2Transform.position.x,
                    customerScene2Transform.position.y, cameraTransform.position.z);

                ver.currentStep = CameraStep.Center;
            }

            StartCoroutine(FadeOutArrow(Arrow[(int)ArrowDirection.Left], 0.3f));
        }
        else if (hor.currentStep == hor.minStep)
        {
            if (ver.lastStep == ver.maxStep)
            {
                cameraTransform.position = new Vector3(counterSceneTransform.position.x,
                    counterSceneTransform.position.y, cameraTransform.position.z);

                ver.currentStep = CameraStep.Center;
            }

            StartCoroutine(FadeOutArrow(Arrow[(int)ArrowDirection.Right], 0.3f));
        }
        else
            FadeInSelector(new[] { ArrowDirection.Left, ArrowDirection.Right });

        if (ver.currentStep == ver.maxStep)
        {
            cameraTransform.position = new Vector3(cookSceneTransform.position.x, cookSceneTransform.position.y,
                cameraTransform.position.z);
            hor.currentStep = CameraStep.One;

            StartCoroutine(FadeOutArrow(Arrow[(int)ArrowDirection.Down], 0.3f));
            FadeInSelector(new[] { ArrowDirection.Left, ArrowDirection.Right, ArrowDirection.Up });
        }
        else if (ver.currentStep == ver.minStep)
        {
            StartCoroutine(FadeOutArrow(Arrow[(int)ArrowDirection.Up], 0.3f));
            FadeInSelector(new[] { ArrowDirection.Down });
        }

        hor.lastStep = hor.currentStep;
        ver.lastStep = ver.currentStep;
    }

    private void FadeOutSelector(ArrowDirection[] dirs)
    {
        foreach (ArrowDirection dir in dirs)
        {
            var arrow = Arrow[(int)dir];

            if (arrow.GetComponent<CanvasGroup>().alpha != 0f)
                StartCoroutine(FadeOutArrow(arrow, 0.3f));
        }
    }

    
    private void FadeInSelector(ArrowDirection[] dirs)
    {
        foreach (ArrowDirection dir in dirs)
        {
            var arrow = Arrow[(int)dir];

            if (arrow.GetComponent<CanvasGroup>().alpha != 1f)
                StartCoroutine(FadeInArrow(arrow, 0.3f));
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

    public void MoveArrowsInToScreen()
    {
        FadeInSelector(new [] { ArrowDirection.Left , ArrowDirection.Right, ArrowDirection.Down, ArrowDirection.Up});
        StartCoroutine(MoveArrows(true));
    }

    public void MoveArrowsOutOfScreen()
    {
        FadeOutSelector(new [] { ArrowDirection.Left , ArrowDirection.Right, ArrowDirection.Down, ArrowDirection.Up});
        StartCoroutine(MoveArrows());
    }

    private IEnumerator MoveArrows(bool toIn = false)
    {
        float t = 0f;
        int direction = toIn ? 1 : -1;

        Vector3 leftPos = Arrow[(int)ArrowDirection.Left].transform.position;
        Vector3 rightPos = Arrow[(int)ArrowDirection.Right].transform.position;
        Vector3 downPos = Arrow[(int)ArrowDirection.Down].transform.position;
        Vector3 upPos = Arrow[(int)ArrowDirection.Up].transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / arrowRemovalDuration;

            Arrow[(int)ArrowDirection.Left].transform.position = Vector3.Lerp(leftPos,
                new Vector3(leftPos.x + 200 * direction, leftPos.y, leftPos.z), t);
            Arrow[(int)ArrowDirection.Right].transform.position = Vector3.Lerp(rightPos,
                new Vector3(rightPos.x - 200 * direction, rightPos.y, rightPos.z), t);
            Arrow[(int)ArrowDirection.Down].transform.position = Vector3.Lerp(downPos,
                new Vector3(downPos.x, downPos.y + 100 * direction, downPos.z), t);
            Arrow[(int)ArrowDirection.Up].transform.position = Vector3.Lerp(upPos,
                new Vector3(upPos.x, upPos.y - 100 * direction, upPos.z), t);
            
            yield return null;
        }
    }
}