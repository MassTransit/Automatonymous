namespace Stayt
{
    public interface StateMachineInstance
    {
        State CurrentState { get; set; }
    }
}