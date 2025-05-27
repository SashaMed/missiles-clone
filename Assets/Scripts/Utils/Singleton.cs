using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Singleton<T> where T : class
{

    static private T instance = null;
    public static T Instance { get { if (instance == null) { instance = Activator.CreateInstance<T>(); ((Singleton<T>)(object)instance).InitInstance(); }; return instance; } }

    virtual public void InitInstance() { }
    public static T Create() { BreakInstance(); return Instance; }
    protected static void BreakInstance() { instance = null; }

}