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

        public void ShowEditUi(Vector2 position, bool isDestroyable)
        {
            editUiPanel.gameObject.SetActive(true);

            const float padding = 20f;
            const float offsetX = 250f;
            const float offsetY = 150f;

            var screenPosition = Camera.main.WorldToScreenPoint(position);
            var panelSize = editUiPanel.rect.size;
            var pivot = editUiPanel.pivot;
            
            screenPosition.x += offsetX;
            screenPosition.y += offsetY;
            
            if (screenPosition.x + panelSize.x * (1f - pivot.x) > Screen.width - padding)
                screenPosition.x -= offsetX + panelSize.x;
            
            if (screenPosition.y + panelSize.y * (1f - pivot.y) > Screen.height - padding)
                screenPosition.y -= offsetY + panelSize.y;
            
            screenPosition.x = Mathf.Clamp(screenPosition.x, padding + panelSize.x * pivot.x, Screen.width - padding - panelSize.x * (1f - pivot.x));
            screenPosition.y = Mathf.Clamp(screenPosition.y, padding + panelSize.y * pivot.y, Screen.height - padding - panelSize.y * (1f - pivot.y));
            editUiPanel.position = screenPosition;
            
            destroyButton.gameObject.SetActive(!isDestroyable);
        }

        public void HideEditUi()
        {
            editUiPanel.gameObject.SetActive(false);
        }
    }
}
