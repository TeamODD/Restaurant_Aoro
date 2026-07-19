using UnityEngine;

public class PreviewRootPositionLock : MonoBehaviour
{
    private Vector3 targetLocalPosition;

    public void Initialize(Vector3 position)
    {
        targetLocalPosition = position;
    }

    private void LateUpdate()
    {
        transform.localPosition = targetLocalPosition;
    }
}