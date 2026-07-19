using TMPro;
using UnityEngine;

public class CustomerCodexDetailUI : MonoBehaviour
{
    [Header("Preview Roots")]
    [SerializeField] private GameObject root;
    [SerializeField] private Transform mainPreviewRoot;
    [SerializeField] private Transform entrancePreviewRoot;
    [SerializeField] private Transform seatedPreviewRoot;
    [SerializeField] private Transform eatingPreviewRoot;

    [Header("Description")]
    [SerializeField] private TMP_Text basicDescriptionText;
    [SerializeField] private TMP_Text detailDescriptionText;

    private GameObject mainPreview;
    private GameObject entrancePreview;
    private GameObject seatedPreview;
    private GameObject eatingPreview;

    [SerializeField] private float previewScale = 70f; //임시 확인용

    public void Open(Customer customer, CustomerCodexEntry entry)
    {
        if (customer == null || entry == null)
            return;

        // 먼저 상세창 활성화
        if (root != null)
            root.SetActive(true);

        ClearPreviews();

        if (entry.mainIllustrationUnlocked)
        {
            mainPreview = CreatePreview(
                customer.prefabStand,
                mainPreviewRoot,
                customer.standStates?.baseState
            );
        }

        if (entry.entranceIllustrationUnlocked)
        {
            entrancePreview = CreatePreview(
                customer.prefabLeft,
                entrancePreviewRoot,
                customer.leftStates?.baseState
            );
        }

        if (entry.seatedIllustrationUnlocked)
        {
            seatedPreview = CreatePreview(
                customer.prefabSeated,
                seatedPreviewRoot,
                customer.seatedStates?.baseState
            );
        }

        // eatingIllustrationUnlocked 필드를 추가한 경우 사용
        /*
        if (entry.eatingIllustrationUnlocked)
        {
            eatingPreview = CreatePreview(
                customer.prefabEating,
                eatingPreviewRoot,
                customer.eatingStates?.baseState
            );
        }
        */

        if (basicDescriptionText != null)
        {
            basicDescriptionText.text = entry.basicDescriptionUnlocked
                ? customer.codexDescription
                : "???";
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = entry.detailDescriptionUnlocked
                ? customer.codexDetailDescription
                : "???";
        }
    }

    private GameObject CreatePreview(
        GameObject prefab,
        Transform parent,
        string stateName)
    {
        if (prefab == null || parent == null)
            return null;

        GameObject previewContainer =
            new GameObject($"{prefab.name}_PreviewContainer");

        previewContainer.transform.SetParent(parent, false);
        previewContainer.transform.localPosition = Vector3.zero;
        previewContainer.transform.localRotation = Quaternion.identity;
        previewContainer.transform.localScale = Vector3.one * previewScale;

        GameObject preview =
            Instantiate(prefab, previewContainer.transform);

        preview.SetActive(true);
        preview.transform.localPosition = Vector3.zero;
        preview.transform.localRotation = Quaternion.identity;
        preview.transform.localScale = Vector3.one;

        Animator animator =
            preview.GetComponentInChildren<Animator>(true);

        if (animator != null)
        {
            SetParentsActive(animator.gameObject, preview.transform);
            animator.enabled = true;

            if (!string.IsNullOrEmpty(stateName))
            {
                animator.Play(stateName, 0, 0f);
                animator.Update(0f);
            }
        }

        PreviewRootPositionLock positionLock =
            preview.AddComponent<PreviewRootPositionLock>();

        positionLock.Initialize(Vector3.zero);

        return previewContainer;
    }

    private void SetParentsActive(GameObject target, Transform root)
    {
        Transform current = target.transform;

        while (current != null)
        {
            current.gameObject.SetActive(true);

            if (current == root)
                break;

            current = current.parent;
        }
    }

    private void ClearPreviews()
    {
        DestroyPreview(ref mainPreview);
        DestroyPreview(ref entrancePreview);
        DestroyPreview(ref seatedPreview);
        DestroyPreview(ref eatingPreview);
    }

    private void DestroyPreview(ref GameObject preview)
    {
        if (preview != null)
            Destroy(preview);

        preview = null;
    }
    private void SetPreviewSorting(GameObject preview)
    {
        SpriteRenderer[] renderers =
            preview.GetComponentsInChildren<SpriteRenderer>(true);

        if (renderers.Length == 0)
            return;

        const int previewBaseOrder = 100;

        int minOrder = int.MaxValue;

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sortingOrder < minOrder)
                minOrder = renderer.sortingOrder;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            int relativeOrder = renderer.sortingOrder - minOrder;

            renderer.sortingLayerName = "UI";
            renderer.sortingOrder = previewBaseOrder + relativeOrder;
        }
    }
}