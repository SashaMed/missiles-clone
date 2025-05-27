using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace SavesData
{

    [Serializable]
    public class RecoveringCurrency : CurrencyDouble
    {
        [Serializable]
        public class Options
        {
            public int maxValue = 10;
            public float recoveringTime = 100;
        }

        public long changeTimestamp;
        public long unlimEndTimestamp;

        public static long TimestampNow => DateTime.UtcNow.Ticks;

        public bool IsUnlim()
        {
            return TimestampNow < unlimEndTimestamp;
        }

        public bool CheckToRecover(Options param)
        {
            if (Value < param.maxValue)
            {
                var timeFromChange = (TimestampNow - changeTimestamp) / TimeSpan.TicksPerSecond;
                var recovered = (int)(timeFromChange / param.recoveringTime);

                if (recovered > 0)
                {
                    changeTimestamp += (long)(recovered * param.recoveringTime) * TimeSpan.TicksPerSecond;
                    Value = Math.Min(Value + recovered, param.maxValue);

                    return true;
                }
            }

            return false;
        }

        public void AddUnlimEnd(float timeSeconds)
        {
            unlimEndTimestamp = Math.Max(unlimEndTimestamp, TimestampNow) + (long)(timeSeconds * TimeSpan.TicksPerSecond);

            InvokeChange();
        }

        public void Spend(Options param, int count = 1)
        {
            if (IsUnlim())
            {
                return;
            }

            if (Value >= param.maxValue)
            {
                changeTimestamp = TimestampNow;
            }

            Value -= count;


        }

        public float LeftTimeToRecover(Options param)
        {
            if (Value >= param.maxValue)
                return 0;

            var timeFromChange = (TimestampNow - changeTimestamp) / (float)TimeSpan.TicksPerSecond;

            return (param.recoveringTime - timeFromChange);

        }

        public float LeftTimeUnlim() => (unlimEndTimestamp - TimestampNow) / (float)TimeSpan.TicksPerSecond;
    }

    [Serializable]
    public class CurrencyDouble : Currency<double>
    {
        public bool HasCount(double value) => Value >= value;

        public bool TrySpend(double value)
        {
            if (HasCount(value))
            {
                Value -= value;
                return true;
            }

            return false;
        }
    }


    [Serializable]
    public class CurrencyFloat : Currency<float>
    {
    }

    [Serializable]
    public class CurrencyInt : Currency<int>
    {
    }


    [Serializable]
    public class Currency<T> where T : struct
    {
        [SerializeField] private T _value = default(T);
        public T Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;
                    OnChange?.Invoke(_value);
                }
            }
        }

        public void InvokeChange() => OnChange?.Invoke(Value);

        [NonSerialized] public Action<T> OnChange;
    }

}