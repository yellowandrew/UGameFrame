using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIViewManager
{

    private static readonly Lazy<UIViewManager> lazy =
       new Lazy<UIViewManager>(() => new UIViewManager());
    public static UIViewManager Instance { get { return lazy.Value; } }

    Dictionary<string, UIView> views;
    Stack<UIView> stack;

    Transform canvas;
     UIViewManager()
    {
        views = new Dictionary<string, UIView>();
        stack = new Stack<UIView>();
        canvas = GameObject.Find("MainCanvas").transform;
    }

    public T OpenView<T>() where T : UIView {
        while (stack.Count > 0) PopView();
       return PushView<T>();
    }
    public T PushView<T>() where T : UIView {
        if (stack.Count > 0)
        {
            UIView v = stack.Peek();
            v.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        UIView view = LoadView<T>() ;
        stack.Push(view);
        ToggleView(view);

        return (T)view;
    }

    public void CloseView()
    {
        PopView();
        if (stack.Count > 0)
        {
            UIView view = stack.Peek();
            ToggleView(view);
        }
    }
    T LoadView<T>() where T : UIView
    {
        string viewName = typeof(T).Name;
        UIView view;
        if (views.ContainsKey(viewName))
        {
            view = views[viewName];
        }
        else
        {
            view = LoadUIView(viewName);
            views.Add(viewName, view);
        }

        return (T)view;
    }
    void PopView()
    {
        UIView view = stack.Pop();
        ToggleView(view, false);
    }
    void ToggleView(UIView view, bool flag = true) {
       // Debug.Log(view.name +" show "+flag);
        view.gameObject.SetActive(flag);
        view.GetComponent<CanvasGroup>().blocksRaycasts = flag;
        if (flag) view.transform.SetAsLastSibling();
    }
    public T GetUIView<T>() where T : UIView => (T)views[typeof(T).Name];

    UIView LoadUIView(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Views/" + prefabName);
        GameObject viewGo = GameObject.Instantiate(prefab);
        viewGo.name = prefab.name;

        viewGo.transform.SetParent(canvas);
        viewGo.transform.localPosition = Vector3.zero;
        viewGo.transform.localScale = Vector3.one;

        CanvasGroup cg = viewGo.GetComponent<CanvasGroup>() ? viewGo.GetComponent<CanvasGroup>() : viewGo.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        UIView view = viewGo.GetComponent<UIView>();
        view.LoadUI();
        viewGo.SetActive(false);
        return view;
    }
}
