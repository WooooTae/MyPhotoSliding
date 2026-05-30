using System;
using UnityEngine;
using UnityEngine.EventSystems;
using WithWild.SafeArea;

namespace WithWild.SafeArea
{
    public enum SafeAreaType{
        Anchor,
        Offset
    }
    
    public class SafeAreaView : UIBehaviour
    {
        private RectTransform _panel;
        private Rect _lastSafeArea = new Rect (0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new Vector2Int (0, 0);
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;
        
        [SerializeField] bool ConformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] bool ConformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField] GameObject GoBlackout;
        [SerializeField] private SafeAreaType SafeAreaType = SafeAreaType.Offset;

        private RectTransform _rectTransform;

        private const float TARGET_RATIO = 16f / 9f;

        protected override void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            if (GoBlackout != null)
                GoBlackout.SetActive(true);
#else
            if (GoBlackout != null)
                GoBlackout.SetActive(false);
#endif
            _panel = GetComponent<RectTransform>();

            if (_panel == null)
            {
                Destroy(gameObject);
            }
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeAreaByHeightRatio();

        }

        void ApplySafeAreaByHeightRatio()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float expectedHeight = screenWidth * TARGET_RATIO;
            float excessHeight = screenHeight - expectedHeight;

            if (excessHeight <= 0f)
                return; 

            float insetRatio = excessHeight / 2f / screenHeight;

            _rectTransform.anchorMin = new Vector2(0, insetRatio);
            _rectTransform.anchorMax = new Vector2(1, 1 - insetRatio);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

    }
}
