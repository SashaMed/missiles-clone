using System.Collections.Generic;
using UnityEngine;


public class ElementManager : IElementManager
{ 
    protected class EmptyElementManager : IElementManager
    {
        public T InstantiateElement<T>(Transform parent, T prefabElement) where T : MonoBehaviour
        {
            return GameObject.Instantiate(prefabElement, parent);
        }

        public void RemoveElement<T>(T element, bool immediate = false) where T : MonoBehaviour
        {
            if(immediate)
                GameObject.DestroyImmediate(element.gameObject);
            else
                GameObject.Destroy(element.gameObject);

        }
         
        public bool CanUse<T>(T element) where T : MonoBehaviour
        {
            return true;
        }
    }

    private static ElementManager instance;
    public static ElementManager Instance => instance ?? (instance = new ElementManager());

    private ElementManager() { }


    private bool inited;

    private bool hasPoling;
    private List<IElementManager> managers = new List<IElementManager>() ;
    protected void Init()
    {
        if (inited) return;

        inited = true;

        //if (SimplePooling.Instance != null)
        //{
        //    hasPoling = true;
        //    managers.Add(SimplePooling.Instance);
        //}

        managers.Add(new EmptyElementManager());

    }

    public T InstantiateElement<T>(Transform parent, T prefabElement) where T : MonoBehaviour
    {
        Init();

        foreach (var m in managers)
        {
            if (m.CanUse(prefabElement))
            {
                return m.InstantiateElement(parent, prefabElement);
            }
        }

        return null;
    }

    public void RemoveElement<T>(T element, bool immediate = false) where T : MonoBehaviour
    {
        Init();

        foreach(var m in managers)
        {
            if(m.CanUse(element))
            {
                m.RemoveElement(element, immediate);
                return;
            }
        }
         
    }

    public bool CanUse<T>(T element) where T : MonoBehaviour
    {
        return true;
    }
}



public interface IElementManager
{
    T InstantiateElement<T>(Transform parent, T prefabElement) where T : MonoBehaviour;
    void RemoveElement<T>(T element, bool immediate = false) where T : MonoBehaviour;
    bool CanUse<T>(T element) where T : MonoBehaviour;
}
