using TMPro;
using UnityEngine;

public class CustomerCodexDetailUI : MonoBehaviour
{
    [Header("Preview Roots")]
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

    public void Open(Customer customer, CustomerCodexEntry entry)
    {
        ClearPreviews();

        if (customer == null || entry == null)
            return;

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

        GameObject preview = Instantiate(prefab, parent);

        preview.transform.localPosition = Vector3.zero;
        preview.transform.localRotation = Quaternion.identity;
        preview.transform.localScale = Vector3.one;

        Animator animator = preview.GetComponentInChildren<Animator>(true);

        if (animator != null && !string.IsNullOrEmpty(stateName))
            animator.Play(stateName, 0, 0f);

        return preview;
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
}