using System;
using System.Globalization;
using UnityEngine;

public class PrefsValue<T>
{

    public string PrefsKey { get; protected set; }
    protected T DefaultValue;

    public T Value
    {
        get => (T)CastGet();
        set => CastSet(value);
    }


    public PrefsValue(string key, T defaultValue = default(T))
    {
        PrefsKey = key;
        DefaultValue = defaultValue;
    }

    public bool HasKey() => PlayerPrefs.HasKey(PrefsKey);

    private object CastGet()
    {
        if (!PlayerPrefs.HasKey(PrefsKey))
            Value = DefaultValue;


        object val = default(T);
        if (typeof(T) == typeof(bool))
            val = (PlayerPrefs.GetInt(PrefsKey) == 1);
        else if (typeof(T) == typeof(int))
            val = PlayerPrefs.GetInt(PrefsKey);
        else if (typeof(T) == typeof(string))
            val = PlayerPrefs.GetString(PrefsKey);
        else if (typeof(T) == typeof(float))
            val = PlayerPrefs.GetFloat(PrefsKey);
        else if (typeof(T) == typeof(DateTime))
            val = PlayerPrefsEx.GetDateTime(PrefsKey);
        else
            throw new InvalidCastException();

        return val;
    }


    private void CastSet(object value)
    {
        if (typeof(T) == typeof(bool))
            PlayerPrefs.SetInt(PrefsKey, ((bool)value) ? 1 : 0);
        else if (typeof(T) == typeof(int))
            PlayerPrefs.SetInt(PrefsKey, ((int)value));
        else if (typeof(T) == typeof(string))
            PlayerPrefs.SetString(PrefsKey, ((string)value));
        else if (typeof(T) == typeof(float))
            PlayerPrefs.SetFloat(PrefsKey, ((float)value));
        else if (typeof(T) == typeof(DateTime))
            PlayerPrefsEx.SetDateTime(PrefsKey, ((DateTime)value));
        else
            throw new InvalidCastException();


        PlayerPrefs.Save();
    }


}

public static class PlayerPrefsEx
{
    public static void SetDateTime(string key, DateTime dateTime)
    {
        PlayerPrefs.SetString(key, dateTime.ToString("O", CultureInfo.InvariantCulture));
    }

    public static DateTime GetDateTime(string key, DateTime defaultValue = default(DateTime))
    {
        if (PlayerPrefs.HasKey(key))
        {
            var dateTimeString = PlayerPrefs.GetString(key);
            return Convert.ToDateTime(dateTimeString).ToUniversalTime();
        }
        else
        {
            return defaultValue;
        }
    }
}
