using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachine<T> : MonoBehaviour {

	private static string separator = "_";
	private static string[] methodTypes = new string[] {
		"Enter", "Exit", "Update", "FixedUpdate"
	};

	private MonoBehaviour component;
	private Dictionary<string, MethodInfo> methodLookUp = new Dictionary<string, MethodInfo> {};
	private T currentState;

	public T CurrentState {
		get {return currentState;}
	}

	public void Initialize (MonoBehaviour comp, T initState) {
		component = comp;
		// Reflect methods
		string[] stateNames = Enum.GetNames(typeof(T));
		for (int i = 0; i < stateNames.Length; i++) {
			for (int j = 0; j < methodTypes.Length; j++) {
				string methodName = stateNames[i] + separator + methodTypes[j];
				MethodInfo method = component.GetType().GetMethod(
					methodName,
					BindingFlags.NonPublic | BindingFlags.Public |
					BindingFlags.Instance | BindingFlags.DeclaredOnly
				);
				if (method != null) {
					methodLookUp.Add(methodName, method);
				}
			}
		}
		currentState = initState;
		Call(currentState, "Enter");
	}

	public void ChangeState (T targetState) {
		if (!targetState.Equals(currentState)) {
			Call(currentState, "Exit");
			currentState = targetState;
			Call(currentState, "Enter");
		}
	}

	void Update () {
		Call(currentState, "Update");
	}

	void FixedUpdate () {
		Call(currentState, "FixedUpdate");
	}

	void Call (T state, string function) {
		string methodName = Enum.GetName(typeof(T), state) + separator + function;
		MethodInfo method;
		if (methodLookUp.TryGetValue(methodName, out method))
			method.Invoke(component, null);
	}
}
