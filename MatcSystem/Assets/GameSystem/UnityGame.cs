using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityGame
{
    MonoBehaviour monoCoroutine;
    public UnityGame()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
    }
    public virtual void Start(MonoBehaviour mono) {
        monoCoroutine = mono;
    }
    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
       
    }

    public virtual void OnChangeLevelFinish(int level)
    {
       
    }

    public virtual void OnApplicationPause(bool pause)
    {
       
    }

    public virtual void OnApplicationQuit()
    {
       
    }
    public void StartCoroutine(IEnumerator routine)
    {
        monoCoroutine.StartCoroutine(routine);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GameAttribute : Attribute { }
