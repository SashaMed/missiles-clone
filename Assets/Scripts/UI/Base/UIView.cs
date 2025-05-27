using UnityEngine;

public abstract class UIViewBase : EntityBase
{
    protected SimpleNavigation Navigation => GetComponentInParent<SimpleNavigation>();
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