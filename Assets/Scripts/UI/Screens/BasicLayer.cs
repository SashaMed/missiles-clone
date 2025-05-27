using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreKit.Runtime.Platform.UI.Basic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif


public class BasicLayer : CoreBasicLayer    
{
    const float _Dur = 0.3f;

    public new static BasicLayer Instance => CoreBasicLayer.Instance as BasicLayer;

    [SerializeField] private CanvasGroup _fader;


    public void BackToStartScreen()
    {
        //var nav = SimpleNavigation.Instance;

        //if (nav.navigationStack
        //    .Any(s => s.GetType() == typeof(GameScreen)))
        //{
        //    if (nav.lastScreen is IBasicScr currScreen)
        //    {
        //        currScreen.Lock = true;
        //    }

        //    FadeWithAction(async () =>
        //    {

        //        if (GameSaves.Data.Progress.GroupedLevel < GameManager.Instance.LevelProvider.startScreenEnableLevel)
        //        {
        //            GoToGameScreen(StartScreen.GetNextGameLevelData());
        //        }
        //        else
        //        {
        //            await SimpleNavigation.Instance.PopScreensTo<StartScreen>(false);

        //            if (nav.navigationStack.Count == 0)
        //            {
        //                SimpleNavigation.Instance.Push<StartScreen, EmptyModel>();
        //            }
        //        }


        //    });
        //}
        //else
        //{
        //    SimpleNavigation.Instance.PopScreensTo<StartScreen>();
        //}
    }

    public void GoToGameScreen(/*StartLevelData gameCfg, bool noFade = false*/)
    {
        //var nav = SimpleNavigation.Instance;

        //if (nav.navigationStack
        //    .Any(s => s.GetType() == typeof(GameScreen)))
        //{
        //    SimpleNavigation.Instance.PopScreensTo<GameScreen>();
        //    (SimpleNavigation.Instance.navigationStack.Last() as GameScreen)?.SetModel(gameCfg);
        //}
        //else
        //{
        //    Instance.FadeWithAction(
        //        () =>
        //        {
        //            SimpleNavigation.Instance.Push<GameScreen, StartLevelData>(gameCfg);
        //        }, noFade ? false : true);
        //}
    }

    private void FadeWithAction(Action doThis, bool waitScreenLoaded = false)
    {
        if (_fader && doThis != null)
        {

            _fader.DOKill(false);
            _fader.gameObject.SetActive(true);
            var seq = DOTween.Sequence(_fader).SetUpdate(true)
                .Append(_fader.DOFade(1, _Dur).ChangeStartValue(0))
                .AppendCallback(doThis.Invoke);

            if (waitScreenLoaded)
            {
                IBasicScr.OnPageAppeared += UnfadeWhenLoaded;
            }
            else
            {
                seq.Append(_fader.DOFade(0, _Dur))
                    .OnComplete(() => _fader.gameObject.SetActive(false));
            }
        }
        else
        {
            doThis?.Invoke();
        }
    }

    private void UnfadeWhenLoaded(IBasicScr scr)
    {
        if (scr.IsAppearing == false)
        {
            IBasicScr.OnPageAppeared -= UnfadeWhenLoaded;

            _fader.DOKill(false);
            _fader.gameObject.SetActive(true);
            DOTween.Sequence(_fader).SetUpdate(true)
                .Append(_fader.DOFade(0, _Dur))
                .OnComplete(() => _fader.gameObject.SetActive(false));
        }
    }

    //public bool TryToHotBuyItem(int coinsPrice, bool openShopOnUnsuccess = true)
    //{
    //    var result = GameSaves.Data.Coins.TrySpend(coinsPrice);
    //    if (!result && openShopOnUnsuccess)
    //    {
    //        SimpleNavigation.Instance.Push<ShopScreen, EmptyModel>();
    //    }

    //    return result;
    //}


#if UNITY_EDITOR

    private async void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            Time.timeScale = Time.timeScale > 1f ? 0.1f : Time.timeScale < 1f ? 1f : 20f;
            Debug.Log($"<b><color=white>Time scale: {Time.timeScale} </color></b>");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            var paused = EditorApplication.isPaused = !EditorApplication.isPaused;
            Debug.Log($"<b><color=white>PAUSED</color></b> (To Unpause:  <b>⇧ ⌘ P</b>)");
        }

    }


#endif

}
