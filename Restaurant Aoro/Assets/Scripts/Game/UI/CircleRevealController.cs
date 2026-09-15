using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleRevealController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image revealImage;

    [Header("Settings")]
    [SerializeField] private float revealTime = 2.0f;
    [SerializeField] private float maxRadius = 1.5f;

    private Material revealMaterial;

    private void Awake()
    {
        if (revealImage == null)
        {
            return;
        }

        if (revealImage.material == null)
        {
            return;
        }

        revealMaterial = Instantiate(revealImage.material);
        revealImage.material = revealMaterial;

        revealMaterial.SetFloat("_Radius", 0f);

        revealImage.gameObject.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(Reveal());
    }

    private IEnumerator Reveal()
    {
        float elapsedTime = 0f;

        while (elapsedTime < revealTime)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / revealTime;

            float radius = Mathf.SmoothStep(
                0f,
                maxRadius,
                t
            );

            revealMaterial.SetFloat(
                "_Radius",
                radius
            );

            yield return null;
        }

        revealMaterial.SetFloat(
            "_Radius",
            maxRadius
        );

        revealImage.gameObject.SetActive(false);
    }
}