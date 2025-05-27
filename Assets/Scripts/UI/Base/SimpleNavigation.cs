using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

public class SimpleNavigation : MonoBehaviourSingleton<SimpleNavigation>
{

    public List<GameObject> screens;
    public List<UIScreensLibrary> screensLibraries;
    [Space]
    public Transform content;

    public GameObject basicLayer;

    public Transform effectsHolder;
    public Transform GetEffectParent() => effectsHolder ?? content;

    public List<IUIScreen> navigationStack = new List<IUIScreen>();
    public IUIScreen lastScreen;

    public event System.Action OnChangeLastScreen;

    public float lastScreenChangeTime { get; protected set; }

    private List<GameObject> registredScreens { get; set; } = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();

        RegisterScreens(screens);
        screensLibraries.ForEach(s => RegisterScreens(s.screens));

        if (content == null)
            content = transform;

        foreach (Transform sub in content)
            sub.gameObject.SetActive(false);

        if (basicLayer != null)
            Instantiate(basicLayer, content);
    }


    protected T Create<T>() where T : MonoBehaviour, IUIScreen
    {
        var screenPrefab = registredScreens.FirstOrDefault(cp => cp.gameObject.GetComponent<T>() != false);
        if (screenPrefab == false)
        {
            Debug.LogError($"Navigation '{name}' can't create screen '{typeof(T).Name}'!");
            return default(T);
        }


        //return ElementManager.Instance.InstantiateElement(content, screenPrefab.GetComponent<T>());
        return Instantiate(screenPrefab, content).GetComponent<T>();

    }

    public TScreen Push<TScreen, TModel>(TModel model = default(TModel)) where TScreen : UIScreen<TModel>
    {
        var screen = Create<TScreen>();
        if (screen != false)
        {
            if (lastScreen != null) lastScreen.IsLastScreen.Value = false;


            navigationStack.Add(screen);
            screen.Show(model);

            lastScreen = screen;

            lastScreenChangeTime = Time.time;
            OnChangeLastScreen?.Invoke();
        }

        return screen;
    }

    public async Task<TScreen> Replace<TScreen, TModel>(TModel model = default(TModel), IUIScreen screen = null, bool animed = true) where TScreen : UIScreen<TModel>
    {
        await (screen == null ? PopLast(animed) : Pop(screen, animed));

        return Push<TScreen, TModel>(model);
    }

    public async Task Pop(IUIScreen screen = null, bool animed = true)
    {
        if (navigationStack.Contains(screen))
        {
            navigationStack.Remove(screen);

            screen.OnClose();

            if (animed)
                await screen.CloseAnimation();

            //ElementManager.Instance.RemoveElement(screen as MonoBehaviour);
            Destroy((screen as MonoBehaviour).gameObject);

            lastScreen = navigationStack.LastOrDefault();
            if (lastScreen != null)
                lastScreen.IsLastScreen.Value = true;

            lastScreenChangeTime = Time.time;
            OnChangeLastScreen?.Invoke();

        }
    }

    public async Task PopLast(bool animed = true)
    {
        if (navigationStack.Count > 0)
        {
            await Pop(navigationStack.Last(), animed);
        }
    }

    public async Task PopScreensTo<TScreen>(bool anim = true)
    {
        while (navigationStack.Count > 0 && navigationStack.Last().GetType() != typeof(TScreen))
        {
            await PopLast(anim);
        }
    }

#if UNITY_ANDROID || UNITY_EDITOR

    void Update()
    {
        var backKey = KeyCode.Escape;
#if UNITY_EDITOR
        backKey = KeyCode.Tab;
#endif
        
        if (Input.GetKeyDown(backKey))
        {
            navigationStack.LastOrDefault()?.OnAndroidBack();
        }
    }

#endif

    public void RegisterScreens(List<GameObject> screensList)
    {
        if(screensList != null)
        {
            registredScreens.AddRange(screensList);
        }
    }

}


public static class SimleNavigationStatic
{
    public static SimpleNavigation GetNavigation(this UIViewBase view) => view.GetComponentInParent<SimpleNavigation>();

    public static void SetLastScreenListener(this IUIScreen screen, Action<bool> listener)
    {
        SimpleNavigation navigation = SimpleNavigation.Instance;

        bool lastValue = navigation.lastScreen == screen;
        listener.Invoke(lastValue);

        Action proccessing = null;

        proccessing = () =>
        {
            var value = navigation.lastScreen == screen;
            if (value != lastValue)
            {
                lastValue = value;
                listener.Invoke(lastValue);
            }

            if (!navigation.navigationStack.Contains(screen))
                navigation.OnChangeLastScreen -= proccessing;

        };

        navigation.OnChangeLastScreen += proccessing;
    }


    public static async Task WaitForFinish<TScreen>(this TScreen screen) where TScreen : IUIScreen
    {
        while (screen.IsActive())
        {
            await Task.Delay(100);
        }
    }

}
