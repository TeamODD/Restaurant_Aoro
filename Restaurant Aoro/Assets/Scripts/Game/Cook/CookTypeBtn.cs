using UnityEngine;

namespace Game.Cook
{
    public class CookTypeBtn: MonoBehaviour
    {
        [SerializeField] private CookType cookType;

        private void OnMouseDown()
        {
            CookManager.instance.cookType = cookType;
        }
    }
}