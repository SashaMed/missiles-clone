using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public object Model { get; protected set; }

    protected bool IsInitialRefresh { get; private set; } = true;

    protected EntityBase SetModel(object model, bool activate = true)
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


public abstract class Entity<T> : EntityBase
{
    public new T Model => (T)base.Model;

    public Entity<T> SetModel(T model, bool activate = true)
    {
        base.SetModel(model, activate);

        return this;
    }
}