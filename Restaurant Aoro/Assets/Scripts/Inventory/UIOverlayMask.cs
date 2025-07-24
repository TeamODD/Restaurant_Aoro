using UnityEngine;

public class UIOverlayMask : MonoBehaviour
{
    public static UIOverlayMask Instance;
    public GameObject overlayPanel;

    private void Awake()
    {
        Instance = this;
        overlayPanel.SetActive(false);
    }

    public void Show()
    {
        overlayPanel.SetActive(true);
    }

    public void Hide()
    {
        overlayPanel.SetActive(false);
    }
}
