using UnityEngine;

namespace Game.Cook
{
    public class CookTypeBtn: MonoBehaviour
    {
        [SerializeField] private CookType cookType;

        private void OnMouseDown()
        {
            Debug.Log($"[CookTypeBtn] Changing Cook Type to {cookType}.");
            CookManager.instance.cookType = cookType;
        }
    }
}