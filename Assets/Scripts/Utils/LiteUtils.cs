using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class LiteUtils
{
    public static void Shuffle<T>(this Random rnd, T[] array)
    {
        var n = array.Length;
        while (n > 1)
        {
            var k = rnd.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }

    public static T[] Shuffle<T>(this T[] array, Random rnd = null)
    {
        rnd = rnd ?? new Random();
        var n = array.Length;
        while (n > 1)
        {
            var k = rnd.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }

        return array;
    }
    
    public static List<T> Shuffle<T>(this List<T> list, Random rnd = null)
    {
        rnd = rnd ?? new Random();
        var n = list.Count;
        while (n > 1)
        {
            var k = rnd.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }
    
    public static string Str<T>(this T[] array) => string.Join(",", array);

    public static void AppendToList<TKey, TVal>(this Dictionary<TKey, List<TVal>> dict, TKey key,  TVal obj)
    {
        if (!dict.ContainsKey(key))
        {
            dict[key] = new List<TVal> {obj};
        }
        else
        {
            dict[key].Add(obj);
        }
    }


    public static RectTransform RectTr(this MonoBehaviour co) => (RectTransform)co.transform;

    public static Color SetA(this Color co, float alpha)
        => new(co.r, co.g, co.b, alpha);
    
    
    #region Stopwatch

    private static Stopwatch _timerStopwatch = null;
    private static string _timerStopwatchOutput = null;

    public static Stopwatch TimerStart(string logWithName = null)
    {
        _timerStopwatchOutput = logWithName;
        _timerStopwatch = new Stopwatch();
        _timerStopwatch.Start();
        return _timerStopwatch;
    }

    public static long TimerFinish(Stopwatch timer = null)
    {
        if ((timer ??= _timerStopwatch) == null)
        {
            return -1;
        }

        timer.Stop();

        if (timer == _timerStopwatch && _timerStopwatchOutput != null)
        {
            Debug.Log($"-> Timer for '<b>{_timerStopwatchOutput}</b>':" +
                                  $" {timer.ElapsedMilliseconds/1000f:0.000}s");
            _timerStopwatchOutput = null;
        }
        _timerStopwatch = null;
        return timer.ElapsedMilliseconds;
    }
    
    #endregion


    #region GraphDrawer
    
    private const string _GraphLevels = "\u3000\u2581\u2582\u2583\u2584\u2585\u2586\u2587\u2588^"; // 9 gradations
    private const string _GraphLevelsShort = "\u3000\u2581\u2583\u2585\u2588^";  // 5 gradations


    public static string DrawGraphic(IEnumerable<int> values, string div10 = ",", string div100 = null, int maxVal = 4)
    {
        if (maxVal <= 0)
        {
            maxVal = values.Max();
        }

        var i = 0;
        var levels = maxVal >= 5 ? _GraphLevels : _GraphLevelsShort;
        var res = "";
        var max = levels.Length - 1;
        foreach (var val in values)
        {
            res += levels[Mathf.Clamp(val, 0, max)];
            if ((i + 1) % 10 == 0) { res += $"<size=10>{i+1}</size>"; }//div10; }
            if ((i + 1) % 100 == 0) { res += div100; }
            i++;
        }

        return res;
    }
    
    public class GetterList : IReadOnlyList<int>
    {
        private Func<int, int> _getter;
        private Range _range;

        public GetterList(Func<int, int> getter, int count): this(getter, new Range(0, count)) { }
        public GetterList(Func<int, int> getter, Range range)
        {
            _getter = getter;
            _range = range;
        }


        IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator_core();
        public IEnumerator GetEnumerator() => GetEnumerator_core();
        
        IEnumerator<int> GetEnumerator_core()
        {
            var i = _range.Start.Value;
            while (i <  _range.End.Value)
            {
                yield return this[i++];
            }
        }


        public int Count => _range.End.Value - _range.Start.Value;
        public int this[int index] => _getter(index);
    }

    #endregion

    
    #if UNITY_EDITOR
    [MenuItem( "Tools/Lite Utils/Open Persistent Storage", false)]
    public static void OpenPersistentStorage() 
        => EditorUtility.RevealInFinder(Application.persistentDataPath);
    #endif

    public static Coroutine PostponeAction(this MonoBehaviour go, Action act, float delaySec)
    {
        IEnumerator Delay(Action action, float time)
        {
            if (time <= 0f)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(time);
            }

            action?.Invoke();
        }
        
        return go.StartCoroutine(Delay(act, delaySec));
    }
}
