using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnityGame
{
    MonoBehaviour monoCoroutine;
    StateMachine stateMachine;
    float startTime;                         
    float playTime;
    public UnityGame()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        Application.targetFrameRate = 30;
        stateMachine = new StateMachine();
    }
    public virtual void Start(MonoBehaviour mono) {
        monoCoroutine = mono;
        startTime = Time.realtimeSinceStartup;
        playTime = PlayerPrefs.GetFloat("PlayTime");
        stateMachine = new StateMachine();

        stateMachine.PushState(GetStartState());
    }
    protected abstract State GetStartState();
    public void ChangeState(State state)
    {
        stateMachine.ChangeState(state);
    }
    public void PushState(State state)
    {
        stateMachine.PushState(state);
    }
    public void PopState()
    {
        stateMachine.PopState();
    }
    public virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))   {
            stateMachine.OnBackKey();
        }
        stateMachine.Update(Time.deltaTime);
    }

    public virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate(Time.deltaTime);
    }

    public virtual void OnChangeLevelFinish(int level)
    {
       
    }

    public virtual void OnApplicationPause(bool pause)
    {
        if (pause)   {
            PlayerPrefs.SetFloat("PlayTime", GetPlayTime());
        } else  {
            playTime = PlayerPrefs.GetFloat("PlayTime"); 
            startTime = Time.realtimeSinceStartup;
        }
    }

    public virtual void OnApplicationQuit()
    {
        Application.Quit();
        PlayerPrefs.SetFloat("PlayTime", GetPlayTime());
    }
    public void StartCoroutine(IEnumerator coroutine)
    {
        if (monoCoroutine == null)
        {
            Debug.LogError("Can't start coroutine because mCoroutineStarter undefine");
            return;
        }
        monoCoroutine.StartCoroutine(coroutine);
    }

    public void StopCoroutine(IEnumerator coroutine)
    {
        monoCoroutine.StartCoroutine(coroutine);
    }
    public float GetPlayTime()
    {
        float elapseTime = Time.realtimeSinceStartup - startTime;
        return playTime + elapseTime;
    }

   public State CreateState<T>() where T : State =>  Activator.CreateInstance(typeof(T), new object[] { this}) as State;
   
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GameAttribute : Attribute { }
