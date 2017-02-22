# tinyfsm
A tiny state machine for Unity

## Usage
Have your components inherit the `StateMachine` class instead of `MonoBehaviour`,
using an enum containing your states as the type parameter, like this:

```csharp
public class MyComponent : StateMachine<MyComponent.State> {
  public enum State {
    State1,
    State2,
    //...
  }
  //...
}
```

Then you can initialize the state machine using the `Initialize()` method,
passing the initial state as a parameter:

```csharp
void Awake () {
  Initialize(this, State.State1);
}
```

All you need to do to change the state is call `ChangeState()`.

```csharp
ChangeState(State.State2);
```

The state machine will call different Update and FixedUpdate methods based on the
current state, as well as Enter and Exit methods when changing the state.

```csharp
void State1_Enter () {
  // Called immediately after changing to the State1 state.
}

void State1_Update () {
  // ...
}

void State1_FixedUpdate () {
  // ...
}

void State1_Exit () {
  // Called right before changing the state from State1 to any other state.
}

void State2_Enter () {
  // ...
}
```


