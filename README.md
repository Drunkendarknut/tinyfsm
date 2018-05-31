# tinyfsm
A tiny state machine for Unity

## Usage
Have your component inherit the `StateMachine` class instead of `MonoBehaviour`, using an enum containing your states as the type parameter.

```csharp
public class MyComponent : StateMachine<MyComponent.State> {
    public enum State {
        State1,
        State2,
        // ...
    }
    // ...
}
```

Initialize the state machine using the `Initialize` method, passing the initial state as a parameter.

```csharp
void Awake() {
    Initialize(State.State1);
}
```

All you need to do to change the state is call `SetState`.

```csharp
SetState(State.State2);
```

The state machine will call different `Update`, `FixedUpdate` and `LateUpdate` methods based on the current state, as well as `Exit` and `Enter` methods when changing the state. The `Enter` method for the initial state is also called on `Initialize`.

```csharp
void State1_Enter() {
    // Called immediately after changing to the State1 state.
}

void State1_Update() {
    // ...
}

void State1_FixedUpdate() {
    // ...
}

void State1_LateUpdate() {
    // ...
}

void State1_Exit() {
    // Called right before changing the state from State1 to any other state.
}

void State2_Enter() {
    // ...
}
```

Use `GetState` to get the current state.

```csharp
if (GetState() == State.State1)
{
    // ...
}
```

If you want to use `Update`, `FixedUpdate` etc. regardless of the current state, you can override those methods.

```csharp
protected override void Update() {
    base.Update();
    // ...
}
```
