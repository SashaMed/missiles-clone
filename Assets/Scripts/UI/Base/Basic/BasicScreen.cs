using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CoreKit.Runtime.Platform.UI.Basic
{
    public abstract class UIBasicSimple : BasicScreen<EmptyModel> { }
    public abstract class BasicScreen<T> : UIScreen<T>, IBasicScr, ISerializationCallbackReceiver
    {
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private Canvas           _canvas;
        
        [SerializeField] private bool             _coverScreenUnder = false;
        [SerializeField] private SafeAreaUtils    _safeArea;
        
        private bool _appearing;
        private int _lockingLevel;



        public static void InitFeatures()
        {
            SimpleNavigation.Instance.OnChangeLastScreen += () => RefreshCoveredScreensVisibility();
        }

        protected static void OnScreenAppearedCallback(IBasicScr scr)
        {
            IBasicScr.Internal_OnPageAppearedInvoke(scr);
            RefreshCoveredScreensVisibility(false);
        }

        protected static void RefreshCoveredScreensVisibility(bool showTop = true)
        {
            var nav = SimpleNavigation.Instance;
            var lastScr = nav.navigationStack.Count > 0 ? nav.navigationStack[^1] as IBasicScr : null;
            if (showTop && lastScr != null)
            {
                lastScr.Visible = true;
            }
            var lastScreenCover = lastScr != null &&
                                  !lastScr.IsAppearing && lastScr.CoverScreenUnder;

            var scr = nav.navigationStack.Count - 2;
            while (scr >= 0)
            {
                if (nav.navigationStack[scr] is IBasicScr screen &&
                    screen.Visible != !lastScreenCover)
                {
                    screen.Visible = !lastScreenCover;
                    lastScreenCover |= screen.CoverScreenUnder;
                    scr--;
                    continue;
                }
                break;
            }
        }


        #region UISCREEN

        
        // public virtual void InitialPrepare() { }
        // public override void Prepare() { }      // On Set Data / Configure()
        public override void Refresh() { }      // Check/Apply changes
        
        public override void OnAndroidBack()
        {
            if (Lock == false)
            {
                Close();
            }
        }

        #region IMPORTANT - Dont skip on override
        ///
        /// !!! CALL base.FUNC() on override !!! 
        ///
        
        public override void OnResume() // On UnHide    / OnUnblock
        {
            Visible = true;
            Lock = false;
            Refresh();
        }

        public override void OnSuspend() // On Hide       / OnBlock
        {
            Lock = true;
        }

        public override void OnClose()
        {
            RefreshCoveredScreensVisibility(true);
        }
        
        
        protected override Task PageLoading()   // Opening Animation
        {
#if !CORE_SCREENS_ORDER
            _canvas.sortingOrder = SimpleNavigation.Instance.navigationStack.Count;
#endif
            var animDuration = AppearAnimationLen();
            if (animDuration > 0)
            {
                Lock =  true;
                _appearing = true;
                return Task.Delay((int)(animDuration * 1000));
            }

            return Task.FromResult(true);
        }
        
        public override void OnPageLoaded(bool successful)      // Appear Animation Done
        {
            if (!_raycaster || !_canvas)    // Check is not destroyed
            {
                Debug.LogError(
                    $"Mandatory component is Null: raycast({_raycaster != false}), canvas({_canvas != false}), Screen({name})");
                return;
            }
                
            _appearing = false;
            OnScreenAppearedCallback(this);

            if (AppearAnimationLen() > 0)
            {
                Lock = false;
            }
        }
        
        #endregion  // IMPORTANT - Dont skip on override
        #endregion  // UISCREEN

        #region IBasicScr

        

        public bool IsAppearing => _appearing;
        public bool CoverScreenUnder => _coverScreenUnder;

        protected virtual float AppearAnimationLen() => 0.3f;
        public bool Visible
        {
            get => !_canvas || _canvas.enabled;
            set {
                if (_canvas)
                {
                    _canvas.enabled = value;
                }
            }
        }

        public bool Lock
        {
            get => _lockingLevel > 0;
            set
            {
                _lockingLevel = Mathf.Max(0, _lockingLevel + (value ? 1 : -1));
                _raycaster.enabled = _lockingLevel <= 0;
                // Debug.Log($"Lock Debug: {name} " + (value ? "+1" : "-1") + $"  \tlvl {_lockingLevel} --> {_raycaster.enabled}");
            }
        }

        
        #endregion
        
        
        void ISerializationCallbackReceiver.OnAfterDeserialize(){}

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (Application.isPlaying)
            {
                if (_raycaster == null)
                {
                    _raycaster = gameObject.GetComponent<GraphicRaycaster>();
                }
                if (_canvas == null)
                {
                    _canvas = gameObject.GetComponentInParent<Canvas>();
                }
            }
        }


        #region DEVICE SCREEN Utils

        protected virtual void OnRectTransformDimensionsChange() => _safeArea.RefreshSafeArea();
        
        #endregion
    }

    public interface IBasicScr
    {
        bool Lock { get; set; }
        bool Visible { get; set; }
        bool IsAppearing { get; }
        bool CoverScreenUnder { get; }
        
        
        
        public static event Action<IBasicScr> OnPageAppeared;
        public static void Internal_OnPageAppearedInvoke(IBasicScr scr)
        {
            OnPageAppeared?.Invoke(scr);
        }
    }
}