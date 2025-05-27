using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UIScreenSimple : UIScreen<EmptyModel> { }

public abstract class UIScreen<T> : UIView<T>, IUIScreen
{
    public const string TRIGGER_OPEN = "Open";
    public const string TRIGGER_CLOSE = "Close";

    public BoolObserved IsLastScreen { get; } = new BoolObserved(true);

    public bool ignoreAnimator = false;
    protected Animator animator;

    public virtual bool IsActive() => this != null && IsLastScreen.Value;

    public async void Show(T model)
    {
        ShowingInitialize();

        OpenAnimation(); // NO AWAIT

        SetModel(model);

        Task loading = PageLoading();
        await loading;

        OnPageLoaded(loading.Status == TaskStatus.RanToCompletion && !loading.IsFaulted);
    }

    private void ShowingInitialize()
    {
        if (!IsInitialRefresh)
            return;

        if (!ignoreAnimator)
            animator = GetComponent<Animator>();

        IsLastScreen.OnChange += val =>
        {
            if (val)
                OnResume();
            else
                OnSuspend();
        };

    }


    protected TaskCompletionSource<bool> closing;

    public async Task CloseAnimation()
    {
        if (animator)
        {
            closing = new TaskCompletionSource<bool>();

            animator.SetTrigger(TRIGGER_CLOSE);
            await closing.Task;

            await Task.Delay(1); // delay after animation callback
        }

    }

    // Called by Close Animations.
    public virtual void CloseAnimationComplete()
    {
        closing?.SetResult(true);
    }



    public void OpenAnimation()
    {
        if (animator)
        {
            animator.SetTrigger(TRIGGER_OPEN);
        }
        
#if CORE_SCREENS_ORDER
        if (gameObject.GetComponentInParent<Canvas>() is {} canvas)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder = SimpleNavigation.Instance.navigationStack.Count;
        }
#endif
    }

    public virtual void Close()
    {
        Navigation.Pop(this);
        //this.GetNavigation().Pop(this);
    }


    protected virtual Task PageLoading()
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// On loading page finished
    /// </summary>
    /// <param name="successful">result of loading</param>
    public virtual void OnPageLoaded(bool successful)
    {
        // ...
    }


    /// <summary>
    /// On closed animation finished
    /// </summary>
    public virtual void OnClose()
    {
        // ...
    }

    public virtual void OnResume()
    {
        // ...
    }

    public virtual void OnSuspend()
    {
        // ...
    }

    public virtual void OnAndroidBack()
    {
        // ...
    }

}

public interface IUIScreen
{
    BoolObserved IsLastScreen { get; }

    void Close();

    void OnClose();

    Task CloseAnimation();

    void OnAndroidBack();

    /*
void OnResume();

void OnSuspend();
*/

    bool IsActive();
}