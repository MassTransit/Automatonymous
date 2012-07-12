Getting Started with Automatonymous
"""""""""""""""""""""""""""""""""""

Once you have added Automatonymous to your project (using NuGet or otherwise), you're ready to create
your first state machine.

Creating Your First State Machine
---------------------------------

A state machine is declared with Automatonymous using an internal domain specific language (DSL). To
declare a state machine, add a class to your project that inherits from AutomatonymousStateMachine. For
our example, we'll use the concept of a vehicle pit stop on the road of life.

.. sourcecode:: csharp

  public class PitStop :
    AutomatonymousStateMachine<VehicleState>
  {
    public PitStop()
    {
      State(() => Running);
      Event(() => Start);

      Initially(
        When(Arrived)
          .TransitionTo(Waiting));
    }

    public Event<Vehicle> Arrived {get; private set;}

    public State Waiting {get; private set;}
  }

So far, only a minimum level of functionality has been declared. When a vehicle arrives, the PitStop
is in an initial state. When the Arrived event is raised, the vehicle information is passed to the handler,
which transitions to the Waiting state.

.. toctree::

    quickstart.rst
