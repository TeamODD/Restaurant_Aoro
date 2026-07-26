using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerCodexDetailUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;
    [Header("Preview")]
    [SerializeField] private Transform previewRoot;
    [SerializeField] private string previewLayerName = "CustomerPreview";
    [SerializeField] private float previewScale = 1f;
    [SerializeField] private string previewSortingLayerName = "CustomerPreview";

    [Header("Preview Buttons")]
    [SerializeField] private Button mainPreviewButton;
    [SerializeField] private Button entrancePreviewButton;
    [SerializeField] private Button seatedPreviewButton;
    [SerializeField] private Button eatingPreviewButton;
    [Header("Description")]
    [SerializeField] private TMP_Text basicDescriptionText;
    [SerializeField] private TMP_Text detailDescriptionText;

    private Customer currentCustomer;
    private CustomerCodexEntry currentEntry;
    private GameObject currentPreview;

    //[SerializeField] private float previewScale = 70f; //임시 확인용
    private void Awake()
    {
        if (mainPreviewButton != null)
            mainPreviewButton.onClick.AddListener(ShowMainPreview);

        if (entrancePreviewButton != null)
            entrancePreviewButton.onClick.AddListener(ShowEntrancePreview);

        if (seatedPreviewButton != null)
            seatedPreviewButton.onClick.AddListener(ShowSeatedPreview);

        if (eatingPreviewButton != null)
            eatingPreviewButton.onClick.AddListener(ShowEatingPreview);
    }
    private void OnDestroy()
    {
        if (mainPreviewButton != null)
            mainPreviewButton.onClick.RemoveListener(ShowMainPreview);

        if (entrancePreviewButton != null)
            entrancePreviewButton.onClick.RemoveListener(ShowEntrancePreview);

        if (seatedPreviewButton != null)
            seatedPreviewButton.onClick.RemoveListener(ShowSeatedPreview);

        if (eatingPreviewButton != null)
            eatingPreviewButton.onClick.RemoveListener(ShowEatingPreview);
    }

    public void Open(Customer customer, CustomerCodexEntry entry)
    {
        if (customer == null || entry == null)
        {
            Debug.LogWarning(
                "[CustomerCodexDetailUI] Customer 또는 Entry가 null입니다."
            );
            return;
        }

        currentCustomer = customer;
        currentEntry = entry;

        if (root != null)
            root.SetActive(true);

        UpdateDescriptions();
        UpdateButtons();

        ClearPreview();

        // 상세창을 처음 열면 기본 모습을 출력
        if (entry.mainIllustrationUnlocked)
            ShowMainPreview();
    }

    public void ShowMainPreview()
    {
        if (!CanShowPreview(currentEntry?.mainIllustrationUnlocked))
            return;

        ShowPreview(
            currentCustomer.prefabStand,
            currentCustomer.standStates?.baseState
        );
    }

    public void ShowEntrancePreview()
    {
        if (!CanShowPreview(currentEntry?.entranceIllustrationUnlocked))
            return;

        ShowPreview(
            currentCustomer.prefabLeft,
            currentCustomer.leftStates?.baseState
        );
    }

    public void ShowSeatedPreview()
    {
        if (!CanShowPreview(currentEntry?.seatedIllustrationUnlocked))
            return;

        ShowPreview(
            currentCustomer.prefabSeated,
            currentCustomer.seatedStates?.baseState
        );
    }

    public void ShowEatingPreview()
    {
        if (!CanShowPreview(currentEntry?.eatingIllustrationUnlocked))
            return;

        ShowPreview(
            currentCustomer.prefabEating,
            currentCustomer.eatingStates?.baseState
        );
    }

    private bool CanShowPreview(bool? unlocked)
    {
        return currentCustomer != null
            && currentEntry != null
            && unlocked == true;
    }

    private void ShowPreview(GameObject prefab, string stateName)
    {
        ClearPreview();

        if (prefab == null)
        {
            Debug.LogWarning(
                "[CustomerCodexDetailUI] 출력할 프리팹이 없습니다."
            );
            return;
        }

        if (previewRoot == null)
        {
            Debug.LogWarning(
                "[CustomerCodexDetailUI] PreviewRoot가 연결되지 않았습니다."
            );
            return;
        }

        currentPreview = Instantiate(prefab, previewRoot);

        currentPreview.SetActive(true);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;
        currentPreview.transform.localScale =
            Vector3.one * previewScale;

        int previewLayer =
            LayerMask.NameToLayer(previewLayerName);

        SetLayerRecursively(currentPreview, previewLayer);
        SetSortingLayerRecursively(
            currentPreview,
            previewSortingLayerName
        );

        if (previewLayer < 0)
        {
            Debug.LogWarning(
                $"[CustomerCodexDetailUI] Layer를 찾을 수 없습니다: " +
                $"{previewLayerName}"
            );
        }
        else
        {
            SetLayerRecursively(currentPreview, previewLayer);
        }

        Animator animator =
            currentPreview.GetComponentInChildren<Animator>(true);

        if (animator != null)
        {
            SetParentsActive(
                animator.gameObject,
                currentPreview.transform
            );

            animator.enabled = true;

            if (!string.IsNullOrEmpty(stateName))
            {
                animator.Play(stateName, 0, 0f);
                animator.Update(0f);
            }
        }

        PreviewRootPositionLock positionLock =
            currentPreview.GetComponent<PreviewRootPositionLock>();

        if (positionLock == null)
        {
            positionLock =
                currentPreview.AddComponent<PreviewRootPositionLock>();
        }

        positionLock.Initialize(Vector3.zero);
    }

    private void UpdateButtons()
    {
        if (currentEntry == null)
            return;

        if (mainPreviewButton != null)
        {
            mainPreviewButton.interactable =
                currentEntry.mainIllustrationUnlocked;
        }

        if (entrancePreviewButton != null)
        {
            entrancePreviewButton.interactable =
                currentEntry.entranceIllustrationUnlocked;
        }

        if (seatedPreviewButton != null)
        {
            seatedPreviewButton.interactable =
                currentEntry.seatedIllustrationUnlocked;
        }

        if (eatingPreviewButton != null)
        {
            eatingPreviewButton.interactable =
                currentEntry.eatingIllustrationUnlocked;
        }
    }

    private void UpdateDescriptions()
    {
        if (currentCustomer == null || currentEntry == null)
            return;

        if (basicDescriptionText != null)
        {
            basicDescriptionText.text =
                currentEntry.basicDescriptionUnlocked
                    ? currentCustomer.codexDescription
                    : "???";
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text =
                currentEntry.detailDescriptionUnlocked
                    ? currentCustomer.codexDetailDescription
                    : "???";
        }
    }

    private void SetLayerRecursively(GameObject target, int layer)
    {
        if (target == null)
            return;

        target.layer = layer;

        foreach (Transform child in target.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void SetParentsActive(
        GameObject target,
        Transform rootTransform)
    {
        Transform current = target.transform;

        while (current != null)
        {
            current.gameObject.SetActive(true);

            if (current == rootTransform)
                break;

            current = current.parent;
        }
    }

    private void ClearPreview()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = null;
    }

    public void Close()
    {
        ClearPreview();

        currentCustomer = null;
        currentEntry = null;

        if (root != null)
            root.SetActive(false);
    }
    private void SetSortingLayerRecursively(
        GameObject target,
        string sortingLayerName)
    {
        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingLayerName = sortingLayerName;
        }
    }
}