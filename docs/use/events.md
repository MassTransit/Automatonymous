# Events

Where states define a state machine at rest, _events_ are what put a state machine into action. As events are raised on a state machine, behaviors are triggered. But before a behavior can be triggered, the events must be declared.

## Declaring events

To declare events, properties are added to the state machine using one of two types: `Event` or `Event<T>`.

```csharp
class OrderStateMachine :
    AutomatonymousStateMachine<OrderState>
{
    public OrderStateMachine()
    {
    }

    public Event<OrderSubmitted> OrderSubmission { get; private set; }
    public Event OrderCancellationRequested { get; private set; }
}

class OrderSubmitted
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public DateTime ReceiveTimestamp { get; set; }
}
```


### Trigger events

A trigger event, declared using `Event`, is an event that can be raised on the state machine which does not contain any data. Trigger events are used when the event is purely a signal with no accompanying data.

The `OrderCancellationRequested` event is a trigger event.


### Data events

A data event, declared using `Event<T>`, is an event that can be raised on the state machine included the data contained in the generic argument type, `T`. The data in the event is passed to the behavior, allowing it to be consumed by the state machine.

The `OrderSubmission` event is a data event.


## Raising Events

To raise an event on a state machine, the state machine must be created and a state instance must be available. An example of raising both of the events above is shown below.

```csharp
var instance = new OrderState();
var machine = new OrderStateMachine();

var orderSubmitted = new OrderSubmitted
{
    OrderId = "123",
    CustomerId = "ABC",
    ReceiveTimestamp = DateTime.UtcNow
};

await machine.RaiseEvent(instance, machine.OrderSubmission, orderSubmitted);

await machine.RaiseEvent(instance, machine.OrderCancellationRequested);
```








