Automatonymous Quick Start
==========================

So you've got the chops and want to get started quickly using Automatonymous. Maybe
you are a bad ass and can't be bothered with reading documentation, or perhaps you
are already familiar with the Magnum StateMachine and want to see what things have
changed. Either way, here it is, your first state machine configured using Automatonymous.

.. sourcecode:: csharp
    :linenos:

    class Relationship :
        StateMachineInstance
    {
        public State CurrentState { get; set; }
        public string Name { get; set; }
    }

    class RelationshipStateMachine :
        AutomatonymousStateMachine<Relationship>
    {
        public RelationshipStateMachine()
        {
            Event(() => Hello);
            Event(() => PissOff);
            Event(() => Introduce);

            State(() => Friend);
            State(() => Enemy);

            Initially(
                When(Hello)
                    .TransitionTo(Friend),
                When(PissOff)
                    .TransitionTo(Enemy),
                When(Introduce)
                    .Then((instance,data) => instance.Name = data.Name)
                    .TransitionTo(Friend)                   
            );
        }

        public State Friend { get; private set; }
        public State Enemy { get; private set; }

        public Event Hello { get; private set; }
        public Event PissOff { get; private set; }
        public Event<Person> Introduce { get; private set; }
    }

	class Person
	{
		public string Name { get; set; }
	}


Seriously?
----------

Okay, so two classes are defined above, one that represents the state (``Relationship``)
and the other that defines the behavior of the state machine (``RelationshipStateMachine``).
For each state machine that is defined, it is expected that there will be at least one instance.
In Automatonymous, state is separate from behavior, allowing many instances to be managed using
a single state machine.

.. note:: 

    For some object-oriented purists, this may be causing the hair to raise on the back of your neck.
    Chill out, it's not the end of the world here. If you have a penchant for encapsulating 
    behavior with data (practices such as domain model, DDD, etc.), recognize that programming language
    constructs are the only thing in your way here.


Tracking State
--------------

State is managed in Automatonymous using a class, shown above as the ``Relationship``. Instances
must implement the ``StateMachineInstance`` interface so that the current state can be stored along
with any other properties used by your application.


Defining Behavior
-----------------

Behavior is defined using a class that inherits from ``AutomatonymousStateMachine``. The class is generic,
and the state type associated with the behavior must be specified. This allows the state machine configuration
to use the state for a better configuration experience.

.. note::

    It also makes Intellisense work better.
    

In a state machine, states must be defined along with the events that can be raised. In the constructor, 
each state and event must be explicitly defined. As each state or event is defined, the specified property
is initialized with the appropriate object type (either a State or an Event), which is why a lambda method
is used to specify the property.

.. note:

    Configuration of a state machine is done using an internal DSL, using an approach known as Object Scoping,
    and is explained in Martin Fowler's Domain Specific Languages book.
    

Creating Instances
------------------


Creating the State Machine
--------------------------


Raising Events
--------------

Once a state machine and an instance have been created, it is necessary to raise an event on the state
machine instance to invoke some behavior. There are three or four participants involved in raising an event: a
state machine, a state machine instance, and an event. If the event includes data, the data for the event is also
included.

The most explicit way to raise an event is shown below.

.. sourcecode:: csharp

    var relationship = new Relationship();
    var machine = new RelationshipStateMachine();
    
    machine.RaiseEvent(relationship, machine.Hello);

If the event has data, it is passed along with the event as shown.

.. sourcecode:: csharp

    var person = new Person { Name = "Joe" };
    
    machine.RaiseEvent(relationship, machine.Introduce, person);

Lifters
^^^^^^^

Lifters allow events to be raised without knowing explicit details about the state machine or the instance type,
making it easier to raise events from objects that do not have prior type knowledge about the state machine or the
instance. Using an approach known as *currying* (from functional programming), individual arguments of raising an event can
be removed.

For example, using an event lift, the state machine is removed.

.. sourcecode:: csharp

    var eventLift = machine.CreateEventLift(machine.Hello);

	// elsewhere in the code, the lift can be used    
    eventLift.Raise(relationship);

The instance can also be lifted, making it possible to raise an event without any instance type knowledge.

.. sourcecode:: csharp

    var instanceLift = machine.CreateInstanceLift(relationship);
	var helloEvent = machine.Hello;

	// elsewhere in the code, the lift can be used
    instanceLift.Raise(helloEvent);

Lifts are commonly used by plumbing code to avoid dynamic methods or delegates, making code
clean and fast.