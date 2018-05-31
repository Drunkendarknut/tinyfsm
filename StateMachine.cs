using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

public class StateMachine<T> : MonoBehaviour where T : IConvertible
{
    const string separator = "_";

    const int ENTER        = 0;
    const int EXIT         = 1;
    const int UPDATE       = 2;
    const int FIXED_UPDATE = 3;
    const int LATE_UPDATE  = 4;

    static readonly string[] callbackNames = new string[]
    {
        "Enter",
        "Exit",
        "Update",
        "FixedUpdate",
        "LateUpdate"
    };

    public delegate void CallbackDel();

    CallbackDel[,] callbacks;
    int currentState;

    public void Initialize(T initState)
    {
        var stateNames = Enum.GetNames(typeof(T));

        callbacks = new CallbackDel[stateNames.Length, callbackNames.Length];

        for (int i = 0; i < stateNames.Length; i++)
        {
            for (int j = 0; j < callbackNames.Length; j++)
            {
                var method = this.GetType().GetMethod(
                    stateNames[i] + separator + callbackNames[j],
                    BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.DeclaredOnly);
                callbacks[i,j] = (method == null)? null : (CallbackDel)Delegate.CreateDelegate(
                    typeof(CallbackDel), this, method);
            }
        }

        currentState = initState.ToInt32(null);
        var func = callbacks[currentState, ENTER];
        if (func != null) func();
    }

    public T GetState()
    {
        return (T)Enum.ToObject(typeof(T), currentState);
    }

    public void SetState(T targetState)
    {
        var targetStateInt = targetState.ToInt32(null);
        if (targetStateInt == currentState) return;

        var exit = callbacks[currentState, EXIT];
        if (exit != null) exit();

        currentState = targetStateInt;

        var enter = callbacks[currentState, ENTER];
        if (enter != null) enter();
    }

    protected virtual void Update()
    {
        var func = callbacks[currentState, UPDATE];
        if (func != null) func();
    }

    protected virtual void FixedUpdate()
    {
        var func = callbacks[currentState, FIXED_UPDATE];
        if (func != null) func();
    }

    protected virtual void LateUpdate()
    {
        var func = callbacks[currentState, LATE_UPDATE];
        if (func != null) func();
    }
}
