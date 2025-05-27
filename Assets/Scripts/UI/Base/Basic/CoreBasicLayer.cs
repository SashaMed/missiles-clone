using UnityEngine;
using UnityEngine.UI;

namespace CoreKit.Runtime.Platform.UI.Basic
{
    public abstract class CoreBasicLayer : MonoBehaviourSingleton<CoreBasicLayer>
    {
        [SerializeField] protected Canvas _topPanel;
        [SerializeField] protected SafeAreaUtils _safeArea;

        //[SerializeField] protected AudioHitInfo _buttonClick;
        //[SerializeField] protected AudioHitInfo _buttonTouch;

        private Canvas _rootCanvas;
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera ??= Camera.main;

        void Start()
        {
            _rootCanvas = _topPanel.rootCanvas;
            if (_rootCanvas.isRootCanvas && _rootCanvas.worldCamera == null)
            {
                _rootCanvas.worldCamera = MainCamera;
                SetupCanvas();
            }

            UIBasicSimple.InitFeatures();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            SetupCanvas();
            _safeArea.RefreshSafeArea();
        }

        #region DeviceScreenUtils

        private CanvasScaler _scaler;

        private static Vector2 GetReferenceResolution()
        {
            var isIpad = false; // ScreenUtils.IsTablet;
            var isLs = Screen.width > Screen.height;

            return new Vector2(1125f, 2200); // isLs ? new Vector2(2436, 1125f) : new Vector2(1125f, 2436);
        }

        private void SetupCanvas()
        {
            _scaler = _scaler ??= _rootCanvas?.GetComponent<CanvasScaler>();
            if (!_scaler)
            {
                return;
            }

            _scaler.referenceResolution = GetReferenceResolution();
        }

        #endregion

        #region TouchSounds     // Replace to pushing event

        //public static void PlayClick() => Instance._buttonClick?.Play();
        //public static void PlayTouch() => Instance._buttonTouch?.Play();

        #endregion

    }
}