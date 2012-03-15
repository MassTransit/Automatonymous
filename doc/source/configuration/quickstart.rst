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
    }

    class RelationshipStateMachine :
        AutomatonymousStateMachine<Relationship>
    {
        public RelationshipStateMachine()
        {
        	Event(() => Hello);
        	Event(() => PissOff);

			State(() => Friend);
			State(() => Enemy);

			Initially(
				When(Hello)
					.TransitionTo(Friend),
				When(PissOff)
					.TransitionTo(Enemy)
			);
        }

		public State Friend { get; private set; }
		public State Enemy { get; private set; }

        public Event Hello { get; private set; }
		public Event PissOff { get; private set; }
    }


Seriously?
""""""""""

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
""""""""""""""

State is managed in Automatonymous using a class, shown above as the ``Relationship``. Instances
must implement the ``StateMachineInstance`` interface so that the current state can be stored along
with any other properties used by your application.


Defining Behavior
"""""""""""""""""

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
""""""""""""""""""


Creating the State Machine
""""""""""""""""""""""""""


Raising Events
""""""""""""""




