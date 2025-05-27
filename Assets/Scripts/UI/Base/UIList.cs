using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIList<TItem, TModel> where TItem : UIView<TModel>
{
    [SerializeField] private RectTransform contentHolder;
    [SerializeField] private TItem itemPattern;


    public List<TModel> Models { get; } = new List<TModel>();
    public IEnumerable<TItem> Views => _views.AsEnumerable().Cast<TItem>(); // Readonly

    public RectTransform GetRect() => contentHolder;

    private List<UIViewBase> _views = new List<UIViewBase>();
    private bool _init = false;

    protected virtual void Init()
    {
        // Hide all inner items, for maximalization of comfort 
        foreach (Transform sub in contentHolder)
            if ((sub.gameObject.GetComponent<LayoutElement>()?.ignoreLayout ?? false) != true)
                sub.gameObject.SetActive(false);

    }

    public void Prepare()
    {
        if (!_init) { _init = true; Init(); }
    }

    public void SetModeles(IEnumerable<TModel> models)
    {
        Models.Clear();
        Models.AddRange(models);
        RefreshModels();
    }

    /// <summary>
    /// Generate <see cref="Views"/> based on modeles in <see cref="Models"/>
    /// </summary>
    public void RefreshModels()
    {
        Prepare();

        _views.ForEach(obj => ElementManager.Instance.RemoveElement(obj, immediate: true));
        _views.Clear();

        Models.ForEach(o => _views.Add(ElementManager.Instance.InstantiateElement(contentHolder, GetPrefabByModel(o)).SetModel(o)));

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);

        // REMOVED: no required now
        // Canvas.ForceUpdateCanvases(); 
    }

    /// <summary>
    /// Just update current <see cref="Views"/>
    /// </summary>
    public void Refresh()
    {
        Views.ForEach(view => view.Refresh());
    }

    protected virtual TItem GetPrefabByModel(TModel model)
    {
        return itemPattern;
    }

    public bool IsConfigured() => contentHolder != false;

}