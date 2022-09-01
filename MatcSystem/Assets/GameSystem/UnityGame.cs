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
    }
    // Update is called once per frame
    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public void OnChangeLevelFinish(int level)
    {
        throw new NotImplementedException();
    }

    public void OnApplicationPause(bool pause)
    {
        throw new NotImplementedException();
    }

    public void OnApplicationQuit()
    {
        throw new NotImplementedException();
    }
}
