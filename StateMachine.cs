﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachine : MonoBehaviour {

	public string separator = "_";
	public int initialStateIndex = 0;
	public string[] states;
	public MonoBehaviour[] components;
	public string CurrentState {
		get {return states[currentState];}
	}

	private enum Function { Enter, Exit, Update, FixedUpdate, LateUpdate };
	private string[] functionNames;
	private Dictionary<string,int> stateIndex = new Dictionary<string,int> () {};
	private MethodInfo[,,] methodLookUp;
	private int currentState;

	private void Awake () {
		functionNames = Enum.GetNames(typeof(Function));
		// Store indices for states
		for (int i = 0; i < states.Length; i++)
			stateIndex.Add(states[i], i);
		// Reflect methods
		methodLookUp = new MethodInfo[components.Length, states.Length, functionNames.Length];
		for (int j = 0; j < states.Length; j++) {
			for (int k = 0; k < functionNames.Length; k++) {
				string methodName = states[j] + separator + functionNames[k];
				for (int i = 0; i < components.Length; i++) {
					methodLookUp[i,j,k] = components[i].GetType().GetMethod(
						methodName,
						BindingFlags.NonPublic | BindingFlags.Public |
						BindingFlags.Instance | BindingFlags.DeclaredOnly
					);
				}
			}
		}
		currentState = initialStateIndex;
		Call(currentState, Function.Enter);
	}

	public void ChangeState (string targetState) {
		if (!targetState.Equals(currentState)) {
			Call(currentState, Function.Exit);
			currentState = stateIndex[targetState];
			Call(currentState, Function.Enter);
		}
	}

	private void Update () {
		Call(currentState, Function.Update);
	}

	private void FixedUpdate () {
		Call(currentState, Function.FixedUpdate);
	}

	private void LateUpdate () {
		Call(currentState, Function.LateUpdate);
	}

	private void Call (int state, Function function) {
		for (int i = 0; i < components.Length; i++) {
			MethodInfo method = methodLookUp[i, state, (int)function];
			if (method != null) method.Invoke(components[i], null);
		}
	}
}
