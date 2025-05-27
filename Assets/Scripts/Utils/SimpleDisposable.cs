using System;

public class SimpleDisposable : IDisposable
{
    public bool IsActive { get; private set; }
    protected Action OnDispose;

    public SimpleDisposable(Action onDispose)
    {
        IsActive = true;
        OnDispose = onDispose;
    }

    public void Dispose()
    {
        if (IsActive)
        {
            IsActive = false;
            OnDispose?.Invoke();
        }
    }
     
}