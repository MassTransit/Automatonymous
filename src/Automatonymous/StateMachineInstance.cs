namespace Automatonymous
{
    public interface StateMachineInstance
    {
        State CurrentState { get; set; }
    }
}