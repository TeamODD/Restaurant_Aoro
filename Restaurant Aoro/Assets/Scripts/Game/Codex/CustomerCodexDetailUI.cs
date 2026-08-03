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
    [SerializeField] private Button rightPreviewButton;
    [SerializeField] private Button perfectPreviewButton;
    [SerializeField] private Button excellentPreviewButton;
    [SerializeField] private Button successPreviewButton;
    [SerializeField] private Button failPreviewButton;
    [Header("Preview Control")]
    [SerializeField] private Button playButton;
    [Header("Description")]
    //[SerializeField] private TMP_Text basicDescriptionText;
    //[SerializeField] private TMP_Text detailDescriptionText;

    [Header("Info Panel")]
    [SerializeField] private CustomerCodexInfoUI infoUI;

    private Customer currentCustomer;
    private CustomerCodexEntry currentEntry;
    private GameObject currentPreview;
    private Animator currentAnimator;
    private bool isPlaying;

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

        if (rightPreviewButton != null)
            rightPreviewButton.onClick.AddListener(ShowRightPreview);

        if (perfectPreviewButton != null)
            perfectPreviewButton.onClick.AddListener(ShowPerfectPreview);

        if (excellentPreviewButton != null)
            excellentPreviewButton.onClick.AddListener(ShowExcellentPreview);

        if (successPreviewButton != null)
            successPreviewButton.onClick.AddListener(ShowSuccessPreview);

        if (failPreviewButton != null)
            failPreviewButton.onClick.AddListener(ShowFailPreview);

        if (playButton != null)
            playButton.onClick.AddListener(TogglePreviewPlayback);
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

        if (rightPreviewButton != null)
            rightPreviewButton.onClick.RemoveListener(ShowRightPreview);

        if (perfectPreviewButton != null)
            perfectPreviewButton.onClick.RemoveListener(ShowPerfectPreview);

        if (excellentPreviewButton != null)
            excellentPreviewButton.onClick.RemoveListener(ShowExcellentPreview);

        if (successPreviewButton != null)
            successPreviewButton.onClick.RemoveListener(ShowSuccessPreview);

        if (failPreviewButton != null)
            failPreviewButton.onClick.RemoveListener(ShowFailPreview);

        if (playButton != null)
            playButton.onClick.RemoveListener(TogglePreviewPlayback);
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

        //UpdateDescriptions();
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
    public void ShowRightPreview()
    {
        if (!CanShowPreview(currentEntry?.rightIllustrationUnlocked))
            return;

        ShowPreview(
            currentCustomer.prefabLeft,
            currentCustomer.rightStates?.baseState
        );
    }

    public void ShowPerfectPreview()
    {
        if (!CanShowPreview(currentEntry?.perfectIllustrationUnlocked))
            return;

        ShowSeatedVariant(2);
    }

    public void ShowExcellentPreview()
    {
        if (!CanShowPreview(currentEntry?.excellentIllustrationUnlocked))
            return;

        ShowSeatedVariant(1);
    }

    public void ShowSuccessPreview()
    {
        if (!CanShowPreview(currentEntry?.successIllustrationUnlocked))
            return;

        ShowSeatedVariant(0);
    }

    public void ShowFailPreview()
    {
        if (!CanShowPreview(currentEntry?.failIllustrationUnlocked))
            return;

        ShowSeatedVariant(3);
    }

    private void ShowSeatedVariant(int variantIndex)
    {
        if (currentCustomer == null || currentEntry == null)
            return;

        if (!currentEntry.seatedIllustrationUnlocked)
            return;

        VariantStates states = currentCustomer.seatedStates;

        if (states == null)
            return;

        string stateName = states.baseState;

        if (states.variants != null &&
            variantIndex >= 0 &&
            variantIndex < states.variants.Count &&
            !string.IsNullOrEmpty(states.variants[variantIndex]))
        {
            stateName = states.variants[variantIndex];
        }

        ShowPreview(
            currentCustomer.prefabSeated,
            stateName
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

        int previewLayer = LayerMask.NameToLayer(previewLayerName);

        if (previewLayer < 0)
        {
            Debug.LogWarning(
                $"[CustomerCodexDetailUI] Layer를 찾을 수 없습니다: {previewLayerName}"
            );
        }
        else
        {
            SetLayerRecursively(currentPreview, previewLayer);
        }

        SetSortingLayerRecursively(
            currentPreview,
            previewSortingLayerName
        );

        currentAnimator =
            currentPreview.GetComponentInChildren<Animator>(true);

        if (currentAnimator != null)
        {
            SetParentsActive(
                currentAnimator.gameObject,
                currentPreview.transform
            );

            currentAnimator.enabled = true;

            if (!string.IsNullOrEmpty(stateName))
            {
                currentAnimator.Play(stateName, 0, 0f);
                currentAnimator.Update(0f);
            }

            // 기본 상태는 정지
            currentAnimator.speed = 0f;
            isPlaying = false;
        }

        UpdatePlayButton();

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
        if (rightPreviewButton != null)
        {
            rightPreviewButton.interactable =
                currentEntry.rightIllustrationUnlocked;
        }

        if (perfectPreviewButton != null)
        {
            perfectPreviewButton.interactable =
                currentEntry.perfectIllustrationUnlocked;
        }

        if (excellentPreviewButton != null)
        {
            excellentPreviewButton.interactable =
                currentEntry.excellentIllustrationUnlocked;
        }

        if (successPreviewButton != null)
        {
            successPreviewButton.interactable =
                currentEntry.successIllustrationUnlocked;
        }

        if (failPreviewButton != null)
        {
            failPreviewButton.interactable =
                currentEntry.failIllustrationUnlocked;
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
        currentAnimator = null;
        isPlaying = false;

        UpdatePlayButton();
    }

    public void Close()
    {
        ClearPreview();

        currentCustomer = null;
        currentEntry = null;

        if (root != null)
            root.SetActive(false);
    }
    public void BackToInfo()
    {
        ClearPreview();

        Customer customer = currentCustomer;
        CustomerCodexEntry entry = currentEntry;

        currentCustomer = null;
        currentEntry = null;

        if (root != null)
            root.SetActive(false);

        if (infoUI != null)
            infoUI.Open(customer, entry);
    }
    private void SetSortingLayerRecursively(
        GameObject target,
        string sortingLayerName)
    {
        if (target == null)
            return;

        if (!SortingLayer.IsValid(SortingLayer.NameToID(sortingLayerName)))
        {
            Debug.LogWarning(
                $"[CustomerCodexDetailUI] Sorting Layer를 찾을 수 없습니다: {sortingLayerName}"
            );
            return;
        }

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingLayerName = sortingLayerName;
        }
    }
    public void TogglePreviewPlayback()
    {
        if (currentAnimator == null)
            return;

        isPlaying = !isPlaying;
        currentAnimator.speed = isPlaying ? 1f : 0f;

        UpdatePlayButton();
    }
    private void UpdatePlayButton()
    {
        if (playButton != null)
            playButton.interactable = currentAnimator != null;
    }
}