using System;
using System.Reflection;
using UnityEngine;

public class StateMachine<T> : MonoBehaviour {

	private static string separator = "_";

	private enum Function { Enter, Exit, Update, FixedUpdate, LateUpdate };
	private MonoBehaviour component;
	private MethodInfo[,] methodLookUp;
	private T currentState;

	public T CurrentState {
		get {return currentState;}
	}

	public void Initialize (MonoBehaviour comp, T initState) {
		component = comp;
		string[] stateNames = Enum.GetNames(typeof(T));
		string[] functionNames = Enum.GetNames(typeof(Function));
		methodLookUp = new MethodInfo[stateNames.Length, functionNames.Length];
		// Reflect methods
		for (int i = 0; i < stateNames.Length; i++) {
			for (int j = 0; j < functionNames.Length; j++) {
				string methodName = stateNames[i] + separator + functionNames[j];
				methodLookUp[i,j] = component.GetType().GetMethod(
					methodName,
					BindingFlags.NonPublic | BindingFlags.Public |
					BindingFlags.Instance | BindingFlags.DeclaredOnly
				);
			}
		}
		currentState = initState;
		Call(currentState, Function.Enter);
	}

	public void ChangeState (T targetState) {
		if (!targetState.Equals(currentState)) {
			Call(currentState, Function.Exit);
			currentState = targetState;
			Call(currentState, Function.Enter);
		}
	}

	void Update () {
		Call(currentState, Function.Update);
	}

	void FixedUpdate () {
		Call(currentState, Function.FixedUpdate);
	}

	void LateUpdate () {
		Call(currentState, Function.LateUpdate);
	}

	void Call (T state, Function function) {
		MethodInfo method = methodLookUp[Convert.ToInt32(state), (int)function];
		if (method != null) method.Invoke(component, null);
	}
}
