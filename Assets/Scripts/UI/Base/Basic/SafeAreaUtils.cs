using System;
using UnityEngine;

namespace CoreKit.Runtime.Platform.UI.Basic
{
    [Serializable]
    public class SafeAreaUtils
    {
        // HOW TO INTEGRATE:   /!\
        //
        // [SerializeField] private SafeAreaUtils    _safeArea;
        // protected virtual void OnRectTransformDimensionsChange() => _safeArea.RefreshSafeArea();
        //
        
        [SerializeField] private RectTransform    _panel;
        

        private static void ApplySafeArea(RectTransform panel, Rect area)
        {
            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }

        private static RectOffset GetDeviceSafeArea() =>
#if UNITY_IOS
            new(0, 0, 34 * 3, 44 * 3);     // Iphone X  (top 44, bott 34)
#else         
            new(0, 0, 0, 24 * 3);     
#endif

        public void RefreshSafeArea()
        {
            if (_panel != false)
            {
                var bounds = GetDeviceSafeArea();
                ApplySafeArea(_panel, 
                    bounds.Remove(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height))));
            }
        }
    }
}