using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DisposeFlag : IDisposable
{
    public bool IsDisposed { get; private set; } = false;

    public void Dispose()
    {
        IsDisposed = true;
    }
}

public static class DisposableHelpers
{

    /// <summary>
    /// Добавить отменяемый объект к текущему
    /// </summary>
    /// <returns></returns>
    public static IDisposable Append(this IDisposable first, IDisposable second)
    {
        return Append(first, new[] { second });
    }

    /// <summary>
    /// Добавить набор отменяемых объектов к текущему
    /// </summary>
    /// <returns></returns>
    public static IDisposable Append(this IDisposable first, IDisposable[] list)
    {
        return new SimpleDisposable(() =>
        {
            first?.Dispose();
            foreach (var d in list)
            {
                d?.Dispose();
            }
        });
    }


}



public static class UtilsHelper
{

    /// <summary>
    /// Проекция вектора на горизонтальной плоскости
    /// </summary>
    public static Vector3 Plane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// Проекция 2D вектора в 3D на горизонтальной плоскости
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 Plane(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }
    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static bool IsZero(this Vector3 vec)
    {
        return vec == Vector3.zero;
    }


    public static void Iterate(this int count, Action<int> action)
    {
        for (int i = 0; i < count; i++)
        {
            action?.Invoke(i);
        }
    }

    public static void Iterate(this int count, Action action)
    {
        for (int i = 0; i < count; i++)
        {
            action?.Invoke();
        }
    }

    public static T Random<T>(this IEnumerable<T> list)
    {
        return list.ElementAtOrDefault(UnityEngine.Random.Range(0, list.Count()));
    }

    public static T PopRandom<T>(this List<T> list)
    {
        T element = list.Random();
        list.Remove(element);

        return element;
    }


    public static float PrettyRound(this float value, float factor = 0.05f)
    {
        return factor * Mathf.Round(value / factor);
    }

    public static float PrettyRoundFront(this float value, int count = 2, float factor = 1f)
    {
        var l = value.CountDigitsByLog10() - count;

        return value.PrettyRound(l >= 1 ? (factor * Mathf.Pow(10, l)) : 1f);


    }

    public static string MultRercentageIncreese(this float value)
    {
        return $"{(value - 1f) * 100:n0}%";
    }

    public static string MultRercentageDecreese(this float value)
    {
        return $"{(1 - value) * 100:n0}%";
    }

    public static int CountDigitsByLog10(this float n)
    {
        return (n == 0) ? 1 : (int)Math.Log10(Math.Abs(n)) + 1;
    }


    public static bool EqualsSerializedValues<T>(this T obj1, T obj2)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj1) == Newtonsoft.Json.JsonConvert.SerializeObject(obj2);
    }

    public static string SerializedValue<T>(this T obj1)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj1);
    }

    public static T CloneSerializedValues<T>(this T obj1)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(obj1));
    }


    public static string ToText(this string key, string defaultValue = null)
    {
        if (string.IsNullOrEmpty(key))
            return key;

#if USE_I2_LOCALIZE
        if (I2.Loc.LocalizationManager.TryGetTranslation(key, out string result))
        {
            return result;
        }
#endif

        return defaultValue ?? key;

    }



    #region Screen
    

    public static bool IsLandscape => Screen.width > Screen.height;
    public static bool IsTablet => DeviceType == MobileDeviceType.Tablet;

    
    private enum MobileDeviceType { Undefined, Phone, Tablet, }

    private static float _ScreenAspect = -1;
    private static MobileDeviceType _DeviceType = MobileDeviceType.Undefined;
    private static MobileDeviceType DeviceType
    {
        get
        {
            Init();
            return _DeviceType;
        }
    }
    private static void Init()
    {
        if (_ScreenAspect > 0)
        {
            return;
        }

        _ScreenAspect = Screen.height / (float)Screen.width;
        if (_ScreenAspect < 1)
        {
            _ScreenAspect = 1 / _ScreenAspect;
        }
        
        _DeviceType = _ScreenAspect > 1.5f ? MobileDeviceType.Phone : MobileDeviceType.Tablet;
    }
    
    #endregion

}


