# Creating a state machine

To create a state machine, you must create two classes. The first class defines the content of the _state instance_, which is the data retained by the state machine. The second class defines the behavior of the state machine itself, including the _states_, the _events_, and the _behaviors_.


## Declaring the state instance

The _state instance_ is a class with only properties - no behavior or methods should be declared. Think of it as a strongly-typed data container used by the state machine to keep track of things -- including the current state.

```csharp
class OrderState
{
    public string CurrentState { get; set; }
}
```

The `OrderState` class above is the simplest possible state instance, it only contains the current state.

> As the extensive capabilities of Automatonymous are explored, this class will be expanded.


## Declaring the state machine

The state machine is a class that defines the state machine behavior, and is declared by inheriting from `AutomatonymousStateMachine<T>`.

> Note, if you are using MassTransit you should inherit from the `MassTransitStateMachine<T>` class instead.


```csharp
class OrderStateMachine :
    AutomatonymousStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
    }
}
```

The state machine declared above is simple, but it does nothing beyond defining the relationship between the _state instance_ and the state machine -- via the `InstanceState` method.


### Specifying the current state property

There are three ways to save the current state in a state instance.

#### Using State

The original approach defined a property of type `State`. This property aligned nicely with the state machine, which uses the same type to define the state machine's states. And it's perfect for state machines that are only used in-memory. However, when persisting the state instance to storage -- which is a key aspect of sagas in MassTransit, it is best to avoid this type and use one of the others.

The state property is defined as shown below.

```csharp
class OrderState
{
    public State CurrentState { get; set; }
}
```


#### Using string

Storing the current state as a string, which is shown in the original class above when the state instance was declared, is easy and provides great visibility in storage to know the current state of a state instance. It's highly recommended, and is always a good choice.


#### Using int

Storing the current state as an _int_ is friendly on storage space, particularly when using tables in a SQL database. It also has the benefit of index performance. It is, however, opaque and the knowledge of which integer value maps to which state must be understood.

When using an integer, the default states are included at predefined values:

    0 - No State (undefined, defaults to Initial upon use)
    1 - Initial (the state of a newly created state instance)
    2 - Final (the state of a finalized state instance)

To use an integer to store the current, state, the state instance is modified as shown.

```csharp
class OrderState
{
    public int CurrentState { get; set; }
}
```

Additionally, when the instance state is declared in the state machine, an ordered list of states must be specified, so that the integer values remain constant as the state machine evolves.

```csharp
class OrderStateMachine :
    AutomatonymousStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState[, state[, state]]);
    }
}
```

For instance, if the state machine had two additional states, `Submitted` and `Accepted`, those states would be declared using the following code (and assigned the values of 3 and 4 respectively).

```csharp
InstanceState(x => x.CurrentState, Submitted, Accepted);
```



