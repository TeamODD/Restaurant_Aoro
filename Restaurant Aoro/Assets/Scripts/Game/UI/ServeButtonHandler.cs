using UnityEngine;

public class ServeButtonHandler : MonoBehaviour
{
    public void OnClick_Serve()
    {
        CustomerClick.ServeLockedCustomer();
    }
}
