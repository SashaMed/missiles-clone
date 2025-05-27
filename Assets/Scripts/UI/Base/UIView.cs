using UnityEngine;



public abstract class UIViewBase : MonoBehaviour
{

    public object Model { get; protected set; }

    protected SimpleNavigation Navigation => GetComponentInParent<SimpleNavigation>();

    protected bool IsInitialRefresh { get; private set; } = true;

    protected UIViewBase SetModel(object model, bool activate = true)
    {
        if (IsInitialRefresh)
            InitialPrepare();

        Model = model;

        Prepare();
        Refresh();

        IsInitialRefresh = false;

        if (activate) gameObject.SetActive(true);

        return this;
    }

    public virtual void InitialPrepare() { }

    public virtual void Prepare() { }

    public abstract void Refresh();


}

public abstract class UIView<T> : UIViewBase
{
    public new T Model => (T)base.Model;

    public UIView<T> SetModel(T model, bool activate = true)
    {
        base.SetModel(model, activate);

        return this;
    }
}

public class EmptyModel { }