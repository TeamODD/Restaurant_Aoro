using System.Collections;
using UnityEngine;

namespace Game.Cook
{
    public class TileOverlayDisable: DisableOtherObjects
    {
        private void OnMouseDown()
        {
            base.OnMouseDown();
            StartCoroutine(Disable());
        }

        private IEnumerator Disable()
        {
            yield return new WaitForSeconds(0.1f);
            CookManager.instance.ResetCookTileOnHold();
        }
    }
}