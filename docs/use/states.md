# States

The state machine would be pretty boring if it only contained the two default states, `Initial` and `Final`. To that end, let's add some states!


## Declaring states

To declare states, properties are added to the state machine using the `State` type.

```csharp
class OrderStateMachine :
    AutomatonymousStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
    }

    public State Submitted { get; private set; }
    public State Accepted { get; private set; }
}
```

Once added, state properties do not need to be declared explicitly, they are automatically initialized by the `AutomatonymousStateMachine` constructor.


## Declaring substates

In addition to regular states, substates are also supported. This makes it possible to go into a substate without actually leaving the superstate. Substates must be explicitly declared, as shown.

```csharp
class OrderStateMachine :
    AutomatonymousStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        SubState(() => AwaitingCreditApproval, Submitted);
    }

    public State Submitted { get; private set; }
    public State AwaitingCreditApproval { get; private set; }
    public State Accepted { get; private set; }
}
```

This would declare `AwaitingCreditApproval` as a substate of `Submitted`. If the state machine transitions to the `AwaitingCreditApproval` state, it is will within the `Submitted` state and events observed in either state would be raised.

