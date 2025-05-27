using System;
using System.Collections.Generic;
using UnityEngine;


public class ValueObserved<T> : IValueObserved
{

    public event Action<T> OnChange;
    public event Action<T, T> OnChangeWithPrev;
    public event Action OnChangeObserved;

    public ValueObserved(T value = default(T))
    {
        _value = value;
    }

    [SerializeField]
    private T _value;
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {

            if (value == null ? _value != null : !value.Equals(_value) )
            {
                var prev = _value;
                _value = value;


                OnChange?.Invoke(value);
                OnChangeWithPrev?.Invoke(value, prev);

                OnChangeObserved?.Invoke();
            }
        }
    }

}


public class ValueCalculated<T> : ValueObserved<T>
{
    protected Func<T> Function;

    protected List<IValueObserved> dependOnList;


    public ValueCalculated(T value = default(T), Func<T> func = null) : base(value)
    {
        Function = func;
    }
 
    protected void OnCalculate()
    {
        if (Function == null) return;

        base.Value = Function.Invoke();
    }

    public new T Value{
        get{

            OnCalculate();
            return base.Value;

        }
    }

    public ValueCalculated<T> Func(Func<T> func)
    {
        Function = func;
        OnCalculate();

        return this;
    }

    public ValueCalculated<T> Depend(params IValueObserved[] depends)
    {
        if (dependOnList == null)
            dependOnList = new List<IValueObserved>();

        depends.ForEach(depend =>
        {
            dependOnList.Add(depend);
            depend.OnChangeObserved += OnCalculate;
        });

        return this;
    }

    public void ClearDepends()
    {
        dependOnList?.ForEach(depend => {
            depend.OnChangeObserved -= OnCalculate;
        });
        dependOnList?.Clear();
    }

}


public interface IValueObserved
{
    event Action OnChangeObserved;
}



[Serializable]
public class BoolObserved : ValueObserved<bool>
{
    public BoolObserved(bool value = false) : base(value){}
}

[Serializable] public class IntObserved : ValueObserved<int>
{
    public IntObserved(int value = 0) : base(value) { }
}

[Serializable] public class FloatObserved : ValueObserved<float>
{
    public FloatObserved(float value = 0) : base(value) { }
}

[Serializable] public class DoubleObserved : ValueObserved<double>
{
    public DoubleObserved(double value = 0) : base(value) { }
}

[Serializable] public class StringObserved : ValueObserved<string>
{
    public StringObserved(string value = null) : base(value) { }
}
