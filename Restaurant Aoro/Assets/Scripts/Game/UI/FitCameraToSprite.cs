using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Camera))]
    public class MatchCameraToSpriteExact : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetSprite;
    
        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
            MatchCameraToBounds();
        }
        
        private void MatchCameraToBounds()
        {
            if (targetSprite == null) return;

            var spriteBounds = targetSprite.bounds;
            
            transform.position = new Vector3(spriteBounds.center.x, spriteBounds.center.y, transform.position.z);
            
            var verticalSize = spriteBounds.size.y;
            cam.orthographicSize = verticalSize / 2f;
            
            var horizontalSize = spriteBounds.size.x;
            cam.aspect = horizontalSize / verticalSize;
        }
    }

}
