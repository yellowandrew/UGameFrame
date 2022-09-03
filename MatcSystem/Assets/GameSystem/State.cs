using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    protected UnityGame TheGame { get => game; }
    UnityGame game;
    public State(UnityGame game)=> this.game = game;

    public virtual void Enter() { Debug.Log(GetType()+ " Enter"); }
    public virtual void Update(float delta) { }
    public virtual void FixedUpdate(float delta) { }
    public virtual void Exit() {
        Debug.Log(GetType() + " Exit");
        Resources.UnloadUnusedAssets();
        GC.Collect(2);
    }
    public virtual void OnLoadLevel() { }
    public virtual void Resume() { Debug.Log(GetType() + " Resume"); }
    public virtual void Pause() { Debug.Log(GetType() + " Pause"); }
    public virtual void OnBackKey() { }
}

public class StateMachine {
    Stack<State> stack;
    Dictionary<string, State> states;
    State currentState;

    public StateMachine()
    {
        stack = new Stack<State>();
    }
    public void Update(float delta)
    {
        currentState?.Update(delta);
    }
    public virtual void FixedUpdate(float delta) {
        currentState?.FixedUpdate(delta);
    }
    internal void OnBackKey()
    {
        currentState?.OnBackKey();
    }
    public void ChangeState(State state) {
        if (stack.Count > 0)   stack.Pop().Exit();
   
        stack.Push(state);
        currentState = state;
        state.Enter();
       
    }
    public void PushState(State state) {
        if (stack.Count>0) stack.Peek().Pause();
        
        stack.Push(state);
        currentState = state;
        state.Enter();
    }

    public void PopState() {
        
        if (stack.Count > 0) { 
            stack.Pop().Exit(); 
        }
        if (stack.Count > 0) {
            stack.Peek().Resume();
        }
    }

  
}


