using UnityEngine;

namespace BlockShift
{
    public class SafeAreaController : MonoBehaviour
    {
        private RectTransform panelRectTransform;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            panelRectTransform = GetComponent<RectTransform>();
            RefreshPanelPosition();
        }

        void Update()
        {
            RefreshPanelPosition();
        }

        void RefreshPanelPosition()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;

                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                panelRectTransform.anchorMin = anchorMin;
                panelRectTransform.anchorMax = anchorMax;
            }
        }
    }
}
