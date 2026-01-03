namespace Game.Cook
{
    public class TileOverlayDisable: DisableOtherObjects
    {
        private void OnMouseDown()
        {
            base.OnMouseDown();
            CookManager.instance.ResetCookTileOnHold();
        }
    }
}