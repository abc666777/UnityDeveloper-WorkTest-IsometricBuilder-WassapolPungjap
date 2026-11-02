using UnityEngine;
using UnityEngine.UI;

namespace ObjectSystem
{
    public class PlacementControllerUi : MonoBehaviour
    {
        public Button placeStationButton;
        public RectTransform editUiPanel;
        public Button placeButton;
        public Button flipButton;
        public Button cancelButton;
        public Button destroyButton;
        
        private float _padding = 5f;
        private float _offsetX = 150f;
        private float _offsetY = 50f;

        public void ShowEditUi(Vector2 position, bool isDestroyable)
        {
            editUiPanel.gameObject.SetActive(true);
            
            var screenPosition = Camera.main.WorldToScreenPoint(position);
            var panelSize = editUiPanel.rect.size;
            var pivot = editUiPanel.pivot;
            
            screenPosition.x += _offsetX;
            screenPosition.y += _offsetY;
            
            if (screenPosition.x + panelSize.x * (1f - pivot.x) > Screen.width - _padding)
                screenPosition.x -= _offsetX + panelSize.x;
            
            if (screenPosition.y + panelSize.y * (1f - pivot.y) > Screen.height - _padding)
                screenPosition.y -= (_offsetY /2f) + panelSize.y;
            
            screenPosition.x = Mathf.Clamp(screenPosition.x, _padding + panelSize.x * pivot.x, Screen.width - _padding - panelSize.x * (1f - pivot.x));
            screenPosition.y = Mathf.Clamp(screenPosition.y, _padding + panelSize.y * pivot.y, Screen.height - _padding - panelSize.y * (1f - pivot.y));
            editUiPanel.position = screenPosition;
            
            destroyButton.gameObject.SetActive(!isDestroyable);
        }

        public void HideEditUi()
        {
            editUiPanel.gameObject.SetActive(false);
        }
    }
}
